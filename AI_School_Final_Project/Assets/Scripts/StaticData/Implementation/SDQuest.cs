using System;

namespace AI_Project.SD
{
    [Serializable]
    public class SDQuest : StaticData
    {
        public string name;
        public Define.Quest.QuestType type;
        /// <summary>
        /// 해당 퀘스트를 수주받기 위해 필요한 선행 퀘스트 목록
        /// </summary>
        public int[] antecedentQuest;
        /// <summary>
        /// 현재 퀘스트 클리어 목표에 관한 인덱스들
        ///  -> 퀘스트 종류에 따라 달라짐
        ///     사냥: 몬스터 인덱스, 수집: 아이템 인덱스, 탐험: 스테이지 인덱스
        /// </summary>
        public int[] target;
        /// <summary>
        /// 현재 퀘스트 클리어 목표에 관한 세부 데이터들
        ///  -> 퀘스트 종류에 따라 달라짐
        ///     사냥: 몇마리?, 수집: 몇개?, 탐험: 좌표
        /// </summary>
        public int[] targetDetail;
        /// <summary>
        /// 퀘스트 클리어 시 보상 아이템들의 인덱스
        /// </summary>
        public int[] rewardItems;
        /// <summary>
        /// 퀘스트 클리어 시 보상 아이템들을 몇 개씩 주는지?
        /// </summary>
        public int[] rewardItemsCount;
        /// <summary>
        /// NPC에게서 퀘스트 수주 시, NPC가 출력할 대사들
        /// </summary>
        public int[] speechRef;
        /// <summary>
        /// 퀘스트 수주 후에, 퀘스트 창에서 확인할 수 있는 퀘스트 상세 내용
        /// </summary>
        public int description;
    }
}
