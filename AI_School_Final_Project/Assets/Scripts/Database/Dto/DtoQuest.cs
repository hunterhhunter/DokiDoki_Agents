using AI_Project.Network;
using System;

namespace AI_Project.DB
{
    [Serializable]
    public class DtoQuest : DtoBase
    {
        /// <summary>
        /// 진행중인 퀘스트에 대한 인덱스 목록
        /// </summary>
        public DtoQuestProgress[] progressQuests;
        /// <summary>
        /// 완료한 퀘스트에 대한 인덱스 목록
        /// </summary>
        public int[] completedQuests;
    }

    /// <summary>
    /// 진행 중인 퀘스트에 대한 상세 데이터를 갖는 Dto
    /// </summary>
    [Serializable]
    public class DtoQuestProgress : DtoBase
    {
        public int index; // 진행중인 퀘스트 인덱스
        public int[] details; // 진행중인 퀘스트에 대한 세부정보 (ex: 사냥이라면 현재 몇마리?)
    }

    [Serializable]
    public class DtoCheckQuest : DtoBase
    {
        public bool isCompleted;
        public DtoQuestProgress dtoQuestProgress;
    }
}
