using AI_Project.DB;
using AI_Project.Util;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AI_Project.UI
{
    public class UIDialouge : UIWindow
    {
        public TextMeshProUGUI speakerName;
        public TextMeshProUGUI dialogue;
        public Transform btnHolder;

        [SerializeField]
        private BoDialogue boDialogue;
        /// <summary>
        /// 현재 활성화된 다이얼로그 버튼들의 참조를 갖는 리스트
        /// </summary>
        private List<DialogueButton> dialogueBtns = new List<DialogueButton>();

        public void SetDialogue(BoDialogue boDialogue)
        {
            this.boDialogue = boDialogue;

            // NPC 이름 적용
            speakerName.text = boDialogue.speaker;
            // 대화창에 출력할 대화 중 맨 첫번째 대화를 적용
            dialogue.text = boDialogue.speeches[0];

            OnDialogueButtons();

            Open();
        }

        /// <summary>
        /// 플레이어가 상호작용 키를 눌러 대화를 다음으로 진행시켰을 경우
        /// 실행될 기능
        /// </summary>
        public bool NextDialogue()
        {
            if (boDialogue == null)
                return false;

            // 현재 대화를 나타내는 인덱스에 +1을 한 값이
            // 전체 대화 배열의 길이 이상이라면, 더 이상 출력할 대화가 없다는 뜻
            if (boDialogue.currentSpeech >= boDialogue.speeches.Length - 1)
            {
                // 따라서 대화창을 종료
                Close();
                return false;
            }
            // 아니라면
            else
            {
                // 아직 출력할 대화가 남았으므로 다음 대화로 변경될 수 있도록
                ++boDialogue.currentSpeech;
                dialogue.text = boDialogue.speeches[boDialogue.currentSpeech];
            }

            return true;
        }

        /// <summary>
        /// 전달받은 다이얼로그 데이터에 맞게 버튼을 배치하는 기능
        /// </summary>
        private void OnDialogueButtons()
        {
            // 다이얼로그 버튼 오브젝트 풀을 가져옴
            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>();

            // NPC가 발행할 수 있는 퀘스트 수만큼 다이얼로그 버튼 활성화
            for (int i = 0; i < boDialogue.quests.Length; ++i)
            {
                // 풀에서 버튼을 하나 가져옴
                var button = pool.GetObj();
                // 가져온 버튼의 부모를 UI 다이얼로그의 버튼홀더로 지정
                //  -> 버튼 홀더의 수직 레이아웃 그룹에 의해 버튼의 위치가 자동정렬
                button.transform.SetParent(btnHolder);
                // 버튼을 초기화
                button.Initialize(new BoDialogueQuest(boDialogue.quests[i]));
                button.gameObject.SetActive(true);

                // 활성 다이얼로그 버튼 목록에 추가
                dialogueBtns.Add(button);
            }

            // NPC에 따라 버튼의 종류가 달라짐
            //  -> 다이얼로그 UI를 설정할 때, BoDialogue에 추가적으로 NPC 타입을 받아와야 함
            // NPC 종류에 따라 추가적인 다이얼로그 버튼을 활성화할 수 있게 됨
        }

        public override void Close(bool force = false)
        {
            base.Close(force);

            boDialogue = null;

            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>();

            for (int i = 0; i < dialogueBtns.Count; ++i)
            {
                pool.Return(dialogueBtns[i]);
            }
            dialogueBtns.Clear();
        }
    }
}
