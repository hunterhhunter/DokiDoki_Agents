using System;
using System.Collections.Generic;
using System.Linq;
using AI_Project.SD;

namespace AI_Project.DB
{
    [Serializable]
    public class BoQuest
    {
        public List<BoQuestProgress> progressQuests;
        public List<SDQuest> completedQuests;

        public BoQuest(DtoQuest dtoQuest)
        {
            progressQuests = new List<BoQuestProgress>();
            completedQuests = new List<SDQuest>();

            // 서버에서 받은 진행퀘스트 정보를 기반으로 인게임 진행퀘스트 정보를 생성
            for (int i = 0; i < dtoQuest.progressQuests.Length; ++i)
            {
                progressQuests.Add(new BoQuestProgress(dtoQuest.progressQuests[i]));
            }

            // 서버에서 받은 완료퀘스트 인덱스를 기반으로 인게임 완료퀘스트 정보를 생성
            for (int i = 0; i < dtoQuest.completedQuests.Length; ++i)
            {
                completedQuests.Add(GameManager.SD.sdQuests.Where(_ => _.index == dtoQuest.completedQuests[i]).SingleOrDefault());
            }
        }
    }

    [Serializable]
    public class BoQuestProgress
    {
        public int[] details;
        public SDQuest sdQuest;

        public BoQuestProgress(DtoQuestProgress dtoQuestProgress)
        {
            details = (int[])dtoQuestProgress.details.Clone();
            sdQuest = GameManager.SD.sdQuests.Where(_ => _.index == dtoQuestProgress.index).SingleOrDefault();
        }
    }
}
