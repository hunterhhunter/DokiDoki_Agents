using System;

namespace AI_Project.SD
{
    [Serializable]
    public class SDMonster : StaticData
    {
        /// <summary>
        /// 이름
        /// </summary>
        public string name;
        /// <summary>
        /// 공격타입
        /// </summary>
        public Define.Actor.AttackType atkType;
        /// <summary>
        /// 속력
        /// </summary>
        public float moveSpeed;
        /// <summary>
        /// 적을 감지할 수 있는 범위 (적은 플레이어)
        /// </summary>
        public float detectionRange;
        /// <summary>
        /// 공격 범위
        /// </summary>
        public float atkRange;
        /// <summary>
        /// 공격 간격(쿨타임)
        /// </summary>
        public float atkInterval;
        /// <summary>
        /// 최대 hp
        /// </summary>
        public float maxHp;
        /// <summary>
        /// 최대 mp
        /// </summary>
        public float maxMana;
        /// <summary>
        /// 공격력
        /// </summary>
        public float atk;
        /// <summary>
        /// 방어력
        /// </summary>
        public float def;
        /// <summary>
        /// 몬스터가 드롭할 수 있는 아이템들의 인덱스
        /// </summary>
        public int[] dropItemRef;
        /// <summary>
        /// 몬스터가 드롭할 수 있는 아이템들을 드롭할 확률
        /// </summary>
        public float[] dropItemPer;
        /// <summary>
        /// 몬스터를 잡았을 시, 드랍되는 경험치
        /// </summary>
        public float dropExp;
        /// <summary>
        /// 프리팹 경로
        /// </summary>
        public string resourcePath;
    }
}
