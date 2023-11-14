using System;

namespace AI_Project.DB
{
    [Serializable]
    public class BoDialogue
    {
        /// <summary>
        /// NPC가 출력할 대사 중 현재 대사의 인덱스
        /// </summary>
        public int currentSpeech;
        /// <summary>
        /// 다이얼로그 버튼 생성에 사용될 퀘스트 인덱스
        ///  -> 해당 NPC가 발행할 수 있는 퀘스트 수만큼 버튼 생성
        /// </summary>
        public int[] quests;
        public string speaker;
        /// <summary>
        /// NPC가 출력할 대사들
        /// </summary>
        public string[] speeches;
    }

    public class BoDialogueBtnBase { }
    public class BoDialogueShop : BoDialogueBtnBase
    {
        
    }
    
    public class BoDialogueQuest : BoDialogueBtnBase
    {
        public int questIndex;

        public BoDialogueQuest(int questIndex)
        {
            this.questIndex = questIndex;
        }
    }

}
