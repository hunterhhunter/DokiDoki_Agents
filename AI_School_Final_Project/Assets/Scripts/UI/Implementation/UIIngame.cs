using AI_Project.Object;
using AI_Project.Util;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    /// <summary>
    /// 인게임 내 잡다한 UI 요소들을 관리할 클래스 (ex: 몬스터 체력바, Hud ui 등)
    /// </summary>
    public class UIIngame : UIWindow
    {
        /// <summary>
        /// 인게임 ui 요소 중 플레이어 데이터를 기반으로 작동할 ui 요소가 많으므로
        /// 참조를 미리 받아둔 상태에서 사용함
        /// </summary>
        public PlayerController playerController;
        /// <summary>
        /// 모든 월드 UI 요소를 갖는 UI 캔버스
        /// </summary>
        public Transform worldUICanvas;

        public TextMeshProUGUI level;
        public Image expGauge;

        public List<BubbleGauge> hpBubbles;
        public List<BubbleGauge> manaBubbles;
        private List<HpBar> allHpBar = new List<HpBar>();

        private Coroutine expAnimCoroutine;

        private void Update()
        {
            BubbleGaugeUpdate();
            HpBarUpdate();
            BillboardUpdate();
        }

        /// <summary>
        /// 월드 UI 요소들에 대한 빌보드 처리
        /// </summary>
        private void BillboardUpdate()
        {
            // 월드 UI 요소들은 2d 이미지이지만, 월드 공간 상에 존재하므로
            // 카메라 회전이나 ui 요소들의 회전에 따라, 특정 각도에서는 원치 않는 방식으로 출력됨
            // 이를 해결하기 위해, ui 요소가 항상 카메라를 바라보게 만들어 ui 요소가 정상적으로 출력될 수
            // 있도록 한다. -> 빌보드 기법

            // 현재 씬의 카메라 컴포넌트 참조를 가져옴
            var camTrans = CameraController.Cam.transform;

            // 월드 ui 요소들의 부모는 월드 ui 캔버스
            // 따라서, 자식 수만큼 순회하며 모든 자식들이 카메라를 바라보도록 만든다.
            for (int i = 0; i < worldUICanvas.childCount; ++i)
            {
                var child = worldUICanvas.GetChild(i);

                // 카메라를 바라보게
                child.LookAt(camTrans, Vector3.up);
                // y축 회전만 하도록
                var newRot = child.eulerAngles;
                newRot.x = 0;
                newRot.z = 0;
                child.eulerAngles = newRot;
            }
        }

        /// <summary>
        /// 버블 게이지 UI 요소들을 업데이트 하는 기능
        /// </summary>
        private void BubbleGaugeUpdate()
        {
            var boActor = playerController?.PlayerCharacter?.boActor;

            // 플레이어 컨트롤러 참조가 없거나, 플레이어 캐릭터 참조가 없거나, boActor 데이터가
            // 아직 세팅되지 않은 경우에는 종료
            if (boActor == null)
                return;

            var hpGauge = boActor.currentHp / boActor.maxHp;
            var manaGauge = boActor.currentMana / boActor.maxMana;

            for (int i = 0; i < hpBubbles.Count; ++i)
            { 
                hpBubbles[i].SetGauge(hpGauge);
                manaBubbles[i].SetGauge(manaGauge);
            }
        }

        /// <summary>
        /// 전체 몬스터 hp 바를 업데이트 하는 기능
        /// </summary>
        private void HpBarUpdate()
        {
            for (int i = 0; i < allHpBar.Count; ++i)
                allHpBar[i]?.HpBarUpdate();
        }

        /// <summary>
        /// 매개변수로 받은 액터의 정보를 기준으로 체력바를 생성하여 
        /// 전체 체력바 리스트에 추가하는 기능
        /// </summary>
        /// <param name="target"></param>
        public void AddHpBar(Actor target)
        {
            var hpBar = ObjectPoolManager.Instance.GetPool<HpBar>().GetObj();

            // 풀에서 가져온 hp바의 부모를 월드 ui 캔버스로 설정
            hpBar.transform.SetParent(worldUICanvas);
            // 타겟의 데이터를 기준으로 hp바 초기화
            hpBar.Initialize(target);
            hpBar.gameObject.SetActive(true);

            allHpBar.Add(hpBar);
        }

        public void SetExp(int level, float expPer)
        {
            this.level.text = $"Lv : {level.ToString()}";
            this.expGauge.fillAmount = expPer;
        }

        public void SetExpAnim(int level, float expPer)
        {
            if (expAnimCoroutine != null)
            {
                StopCoroutine(expAnimCoroutine);
                expAnimCoroutine = null;
            }

            expAnimCoroutine = StartCoroutine(ExpProgress());

            IEnumerator ExpProgress()
            {
                // UI 상에 출력되고 있는 레벨을 받아옴 (미리 필드로 레벨을 받아놓는다면 더 좋음)
                int viewLevel = int.Parse(Regex.Replace(this.level.text, @"[^\d]", ""));

                // 보여지는 레벨과 실제 레벨이 다르다면 경험치 게이지 애니메이션 반복
                while (viewLevel != level)
                {
                    // 레벨이 다르므로 강제로 100퍼센트까지 게이지가 채워지도록 애님 코루틴 시작
                    yield return StartCoroutine(ExpAnim(1f));

                    expGauge.fillAmount = 0;
                    this.level.text = $"Lv : {++viewLevel}";
                }

                // 위의 과정을 통해 레벨을 동일한 값으로 맞춰짐
                // 나머지 퍼센테이지만 동일한 값으로 맞춰준다.
                yield return StartCoroutine(ExpAnim(expPer));
            }

            IEnumerator ExpAnim(float target)
            {
                // 실제 게이지 값과 채우고자 하는 게이지 값이 근사해질 때까지 게이지 애님 반복
                while (Mathf.Abs(expGauge.fillAmount - target) > .05f)
                {
                    expGauge.fillAmount = Mathf.MoveTowards(expGauge.fillAmount, target, Time.deltaTime);
                    yield return null;
                }

                // 근사해졌다면 반복을 끝내고 마지막에 타겟 값으로 fillAmount 값을 설정
                expGauge.fillAmount = target;
            }

            //this.level.text = $"Lv : {level.ToString()}";
            //this.expGauge.fillAmount = expPer;
        }

        /// <summary>
        /// 스테이지 전환 시, 현재 스테이지에서 사용하던 UI 요소들을 비우는 작업
        /// </summary>
        public void Clear()
        {
            var hpBarPool = ObjectPoolManager.Instance.GetPool<HpBar>();

            for (int i = 0; i < allHpBar.Count; ++i)
                hpBarPool.Return(allHpBar[i]);

            allHpBar.Clear();
        }
    }
}
