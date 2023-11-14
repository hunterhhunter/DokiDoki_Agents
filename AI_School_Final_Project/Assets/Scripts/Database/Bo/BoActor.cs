using AI_Project.Define;
using System;
using UnityEngine;

namespace AI_Project.DB
{
    /// <summary>
    /// Actor 타입들의 공통된 데이터를 정의
    ///  ex) 캐릭터, 몬스터
    /// 이 곳에는 기획 데이터(고정) 및 인게임에서만 사용하는 휘발성(Volatile) 데이터 등을 혼재하여 갖는다.
    /// </summary>
    [Serializable]
    public class BoActor
    {
        /// <summary>
        /// 레벨
        /// </summary>
        public float level;
        /// <summary>
        /// 액터 종류
        /// </summary>
        public Actor.Type type;
        /// <summary>
        /// 공격 타입 (ex: 근접, 발사체(원거리) 등)
        /// </summary>
        public Actor.AttackType atkType;
        /// <summary>
        /// 속력
        /// </summary>
        public float moveSpeed;
        /// <summary>
        /// 이동방향
        /// </summary>
        public Vector3 moveDir;
        /// <summary>
        /// 회전방향
        /// </summary>
        public Vector3 rotDir;
        /// <summary>
        /// 현재 체력
        /// </summary>
        public float currentHp;
        /// <summary>
        /// 최대 체력
        /// </summary>
        public float maxHp;
        /// <summary>
        /// 현재 마나
        /// </summary>
        public float currentMana;
        /// <summary>
        /// 최대 마나
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
        /// 공격 범위
        /// </summary>
        public float atkRange;
        /// <summary>
        /// 공격 간격(쿨타임)
        /// </summary>
        public float atkInterval;
        /// <summary>
        /// 현재 땅에 있는지?
        /// </summary>
        public bool isGround;
    }
}
