using AI_Project.DB;
using AI_Project.Resource;
using AI_Project.Util;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AI_Project.Define.UI;

namespace AI_Project.UI
{
    public class DialogueButton : MonoBehaviour, IPoolableObject
    {
        public TextMeshProUGUI title;
        public Image icon;
        public Button btn;

        public bool CanRecycle { get; set; } = true;

        public void Initialize(BoDialogueBtnBase boDialogueBtnBase)
        {
            // 버튼을 초기화 시, 변경되어야할 데이터가 무엇인가?
            //  -> 버튼의 종류에 따라 아이콘 이미지가 변경되어야 함
            //  -> 버튼의 종류 및 데이터에 따라 텍스트가 변경되어야 함
            //  -> 버튼의 종류에 따라 버튼에 바인딩되는 이벤트도 변경되어야 함

            //if (boDialogueBtnBase is BoDialogueShop)
            //{
            //    var boDialogueShop = boDialogueBtnBase as BoDialogueShop;
            //}
            //else if (boDialogueBtnBase is BoDialogueQuest)
            //{ 
            //}

            // 버튼에 이전에 바인딩된 이벤트 제거
            btn.onClick.RemoveAllListeners();

            // 아이콘에 적용시킬 스프라이트의 이름
            var spriteKey = string.Empty;

            switch (boDialogueBtnBase)
            {
                case var boDialogueShop when boDialogueBtnBase is BoDialogueShop:
                    spriteKey = "";

                    break;
                case BoDialogueQuest boDialogueQuest when boDialogueBtnBase is BoDialogueQuest:
                    spriteKey = "exclamation_mark";

                    // BoDialogueQuest에 있는 퀘스트 인덱스를 기반으로 퀘스트 기획 데이터를 가져옴
                    var sdQuest = GameManager.SD.sdQuests.Where(_ => _.index == boDialogueQuest.questIndex).SingleOrDefault();
                    // 버튼의 타이틀에 퀘스트 이름이 출력되도록
                    title.text = sdQuest.name;

                    var uiWindowManager = UIWindowManager.Instance;
                    // 버튼에 해당 버튼을 클릭 시, 퀘스트 데이터를 기반으로 퀘스트 수주 UI가 팝업되도록
                    btn.onClick.AddListener(() =>
                    {
                        uiWindowManager.GetWindow<UIDialouge>().Close();
                        uiWindowManager.GetWindow<UIQuest>().Open(QuestWindow.Order, sdQuest);
                    });
                    break;
            }

            icon.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.IconAtlas, spriteKey);
        }
    }
}
