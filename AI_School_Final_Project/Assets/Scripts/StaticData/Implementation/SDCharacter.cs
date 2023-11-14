using System;
using static AI_Project.Define.Actor;

namespace AI_Project.SD
{ 
    [Serializable]
    public class SDCharacter : StaticData
    {
        /// <summary>
        /// 이름
        /// </summary>
        public string name;
        /// <summary>
        /// 일반 공격 타입
        /// </summary>
        public AttackType atkType;
        /// <summary>
        /// 이동 속력
        /// </summary>
        public float moveSpeed;
        /// <summary>
        /// 점프 시 캐릭터에 가해지는 힘
        /// </summary>
        public float jumpForce;
        /// <summary>
        /// 일반 공격 범위
        /// </summary>
        public float atkRange;
        /// <summary>
        /// 일반 공격 간격 (쿨타임)
        /// </summary>
        public float atkInterval;
        /// <summary>
        /// 성장스텟 테이블 인덱스 참조
        ///   -> 이쪽은 나중에 설명드림
        /// </summary>
        public int growthStatRef;
        /// <summary>
        /// 프리팹 경로
        /// </summary>
        public string resourcePath;
    }
}
