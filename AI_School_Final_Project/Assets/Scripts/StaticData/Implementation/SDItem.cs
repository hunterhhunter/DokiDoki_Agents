using System;


namespace AI_Project.SD
{ 
    [Serializable]
    public class SDItem : StaticData
    {
        public string name;
        /// <summary>
        /// 아이템 종류 (ex: 장비, 소모품, 퀘스트, 기타 등)
        /// </summary>
        public Define.Item.Type itemType;
        /// <summary>
        /// 아이템 사용 또는 장착 시, 영향을 끼치는 스텟들
        /// </summary>
        public string[] affectingStats;
        /// <summary>
        /// 위의 필드에서 영향을 끼치는 스텟들에 적용되는 값
        /// </summary>
        public float[] affectingStatsValue;
        public string description;
        
        public string resourcePath;
    }
}
