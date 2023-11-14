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
        /// 이미지의 fillAmount 를 제어하는 기능
        /// </summary>
        /// <param name="value"></param>
        public void SetGauge(float value)
        {
            gauge.fillAmount = value;            
        }
    }
}