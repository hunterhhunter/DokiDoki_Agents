using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    /// <summary>
    /// 로딩씬의 UI 요소 제어
    /// </summary>
    public class UILoading : UIWindow
    {
        private string dot = string.Empty;
        private const string stateDesc = "Load Next Scene";

        /// <summary>
        /// 로드 상태 텍스트 컴포넌트 참조
        /// </summary>
        public TextMeshProUGUI loadState;
        /// <summary>
        /// 로드 게이지 이미지 컴포넌트 참조
        /// </summary>
        public Image loadGauge;

        /// <summary>
        /// 로딩 씬의 카메라 컴포넌트 참조
        /// </summary>
        public Camera cam;

        private void Update()
        {
            loadGauge.fillAmount = GameManager.Instance.loadState;

            // 간단한 텍스트 애니메이션
            // 20프레임마다 . 이 추가 되고 최대 개수에 이르면 다시 . 하나부터 반복
            if (Time.frameCount % 20 == 0)
            {
                if (dot.Length >= 3)
                    dot = string.Empty;
                else
                    dot = string.Concat(dot, ".");

                loadState.text = $"{stateDesc}{dot}";
            }
        }
    }
}