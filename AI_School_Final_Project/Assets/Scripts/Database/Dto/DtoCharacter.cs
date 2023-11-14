using AI_Project.Network;
using System;

namespace AI_Project.DB
{
    [Serializable]
    public class DtoCharacter : DtoBase
    {
        /// <summary>
        /// 유저의 캐릭터 인덱스
        /// </summary>
        public int index;
        /// <summary>
        /// 유저의 캐릭터 레벨
        /// </summary>
        public int level;
        /// <summary>
        /// 유저의 현재 경험치
        /// </summary>
        public float currentExp;
    }
}
