using UnityEngine;
using AI_Project.Define;
using AI_Project.Resource;
using AI_Project.Dummy;
using AI_Project.Network;
using AI_Project.UI;
using System.Collections;

namespace AI_Project
{
    /// <summary>
    /// 타이틀 씬에서 게임 시작 전에 필요한 전반적인 초기화 및
    /// 데이터 로드를 수행할 클래스
    /// </summary>
    public class TitleController : MonoBehaviour
    {
        /// <summary>
        /// 현재 페이즈의 완료 상태
        /// </summary>
        private bool loadComplete;

        /// <summary>
        /// 외부에서 loadComplete에 접근하기 위한 프로퍼티
        /// 추가로 현재 페이즈 완료 시 조건에 따라 다음 페이즈로 변경
        /// </summary>
        public bool LoadComplete
        {
            get { return loadComplete; }
            set
            {
                loadComplete = value;

                // 현재 페이즈가 완료되었고, 모든 페이즈가 완료되지 않았다면
                if (loadComplete && !AllLoaded)
                    // 다음 페이즈로 변경
                    NextPhase();
            }
        }

        /// <summary>
        /// 모든 페이즈의 완료 상태
        /// </summary>
        public bool AllLoaded { get; private set; }

        /// <summary>
        /// 현재 페이즈를 나타냄
        /// </summary>
        private IntroPhase introPhase = IntroPhase.Start;

        /// <summary>
        /// 로딩 게이지 애니메이션 처리에 사용될 코루틴 객체를 담아둘 필드
        ///  -> 페이즈가 빠르게 변경될 시, 이전에 발생된 코루틴이 존재하는 상태에서
        ///     동일한 코루틴을 다시 발생 시킬 시 문제가 발생할 수 있음.
        ///     따라서, 이 때 이전에 발생된 코루틴 객체를 담아두기 위해 사용
        /// </summary>
        private Coroutine loadGaugeUpdateCoroutine;
        public UITitle uiTitle;

        /// <summary>
        /// 타이틀 컨트롤러 초기화
        /// </summary>
        public void Initialize()
        {
            OnPhase(introPhase);
        }

        private void OnPhaseAnim(IntroPhase phase)
        {
            // 현재 페이즈를 문자열로 전달하여 로딩 상태 텍스트 갱신
            uiTitle.SetState(phase.ToString());

            // 이전에 로딩게이지 업데이트 코루틴을 발생시켰을 시에
            // 아직 로딩게이지 UI의 fillAmount가 실제 로딩 게이지 퍼센테이지만큼
            // 보간이 안됐다면 아직 코루틴은 실행중임.
            // 이미 실행중인 코루틴을, 또 발생시키면 오류가 발생하므로
            // 코루틴이 존재한다면 멈춘 후에, 새로 변경된 퍼센테이지 값을 넘겨 코루틴을 새로 시작한다.
            if (loadGaugeUpdateCoroutine != null)
            {
                StopCoroutine(loadGaugeUpdateCoroutine);
                loadGaugeUpdateCoroutine = null;
            }

            // 변경된 페이즈가 전체 페이즈 완료가 아니라면
            if (phase != IntroPhase.Complete)
            {
                // 현재 로드 퍼센테이지를 구한다.
                var loadPer = (float)phase / (float)IntroPhase.Complete;
                // 구한 퍼센테이지를 로딩바에 적용 
                loadGaugeUpdateCoroutine = StartCoroutine(uiTitle.LoadGaugeUpdate(loadPer));
            }
            // 완료라면 강제로 uiFillAmount를 1로 적용
            else
                uiTitle.loadGauge.fillAmount = 1f;       
        }


        /// <summary>
        /// 현재 페이즈에 대한 로직 실행
        /// </summary>
        /// <param name="phase">진행시키고자 하는 현재 페이즈</param>
        private void OnPhase(IntroPhase phase)
        {
            //OnPhaseAnim(phase);

            switch (phase)
            {
                case IntroPhase.Start:
                    break;
                case IntroPhase.AppSetting:
                    GameManager.Instance.OnAppSetting();
                    break;
                case IntroPhase.Server:
                    DummyServer.Instance.Initialize();
                    ServerManager.Instance.Initialize();
                    break;
                case IntroPhase.StaticData:
                    GameManager.SD.Intialize();
                    break;
                case IntroPhase.UserData:
                    // 게임 시작 시 필요한 유저 데이터를 서버에 요청해서 받아옴
                   // new LoginHandler().Connect();
                    break;
                case IntroPhase.Resource:
                    ResourceManager.Instance.Initialize();
                    break;
                case IntroPhase.UI:
                    UIWindowManager.Instance.Initialize();
                    break;
                case IntroPhase.Complete:
                    var stageManager = StageManager.Instance;

                    GameManager.Instance.LoadScene(SceneType.Ingame, stageManager.ChangeStage()
                        , stageManager.OnChangeStageComplete);
                    AllLoaded = true;
                    break;
            }

            LoadComplete = true;
        }

        /// <summary>
        /// 페이즈를 다음 페이즈로 변경
        /// </summary>
        private void NextPhase()
        {
            StartCoroutine(WaitForSeconds());

            IEnumerator WaitForSeconds()
            {
                yield return new WaitForSeconds(.8f);

                loadComplete = false;
                OnPhase(++introPhase);
            }
        }
    }
}
