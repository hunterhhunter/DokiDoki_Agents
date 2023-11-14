using AI_Project.SD;
using AI_Project.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class QuestSlot : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; } = true;

        public Button btn;
        public TextMeshProUGUI title;

        /// <summary>
        /// 퀘스트 슬롯 초기화 기능
        /// </summary>
        /// <param name="sdQuest">퀘스트 기획 데이터</param>
        /// <param name="details">해당 슬롯이 진행퀘스트일 시 진행 상세정보, 완료퀘스트일 시 null</param>
        public void Initialize(SDQuest sdQuest, params int[] details)
        {
            title.text = sdQuest.name;

            // 퀘스트 슬롯은 풀러블 오브젝트로 재사용되므로,
            // 버튼에 이전에 바인딩된 이벤트를 제거한다.
            btn.onClick.RemoveAllListeners();

            // 버튼 클릭 시, 해당 슬롯에 전달된 퀘스트 상세정보 UI 띄우기 
        }
    }
}
