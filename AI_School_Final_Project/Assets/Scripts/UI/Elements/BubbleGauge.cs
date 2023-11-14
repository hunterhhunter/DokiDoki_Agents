using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class BubbleGauge : MonoBehaviour
    {
        private Image gauge;

        private void Start()
        {
            gauge = GetComponent<Image>();
        }

        /// <summary>
        /// �̹����� fillAmount �� �����ϴ� ���
        /// </summary>
        /// <param name="value"></param>
        public void SetGauge(float value)
        {
            gauge.fillAmount = value;            
        }
    }
}