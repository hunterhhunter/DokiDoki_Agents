using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    /// <summary>
    /// �ε����� UI ��� ����
    /// </summary>
    public class UILoading : UIWindow
    {
        private string dot = string.Empty;
        private const string stateDesc = "Load Next Scene";

        /// <summary>
        /// �ε� ���� �ؽ�Ʈ ������Ʈ ����
        /// </summary>
        public TextMeshProUGUI loadState;
        /// <summary>
        /// �ε� ������ �̹��� ������Ʈ ����
        /// </summary>
        public Image loadGauge;

        /// <summary>
        /// �ε� ���� ī�޶� ������Ʈ ����
        /// </summary>
        public Camera cam;

        private void Update()
        {
            loadGauge.fillAmount = GameManager.Instance.loadState;

            // ������ �ؽ�Ʈ �ִϸ��̼�
            // 20�����Ӹ��� . �� �߰� �ǰ� �ִ� ������ �̸��� �ٽ� . �ϳ����� �ݺ�
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