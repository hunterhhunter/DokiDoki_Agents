using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class UITitle : MonoBehaviour
    {
        public TextMeshProUGUI loadState;
        public Image loadGauge;

        /// <summary>
        /// 로딩 상태 텍스트 설정
        /// </summary>
        /// <param name="state"></param>
        public void SetState(string state)
        {
            loadState.text = $"Load {state}...";
        }

        /// <summary>
        /// 로딩 바 애니메이션 처리
        /// </summary>
        /// <param name="loadPer">현재 로드 퍼센테이지</param>
        /// <returns></returns>
        public IEnumerator LoadGaugeUpdate(float loadPer)
        {
            // ui 의 fillAmount 값이랑 파라미터로 전달받은 퍼센테이지 값이랑
            // 근사하지 않다면 반복
            while (!Mathf.Approximately(loadGauge.fillAmount, loadPer))
            {
                loadGauge.fillAmount = Mathf.Lerp(loadGauge.fillAmount, loadPer, Time.deltaTime * 2f);

                yield return null;
            }
        }
    }
}
