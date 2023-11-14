using System;

namespace AI_Project.SD
{
    [Serializable]
    public class SDGrowthStat : StaticData
    {
        /// <summary>
        /// 레벨에 따른 최대 체력 계산 시 베이스값
        /// </summary>
        public float maxHp;
        /// <summary>
        /// 레벨에 따른 최대 체력 계산 시 계수
        ///   -> level * maxHp * maxHpFactor
        /// </summary>
        public float maxHpFactor;
        public float maxMana;
        public float maxManaFactor;
        public float atk;
        public float atkFactor;
        public float def;
        public float defFactor;
        public float behaviour;
        public float behaviourFactor;
        public float maxExp;
        public float maxExpFactor;
    }
}
