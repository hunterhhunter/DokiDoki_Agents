using AI_Project.SD;
using System;
using UnityEngine;

namespace AI_Project.DB
{
    [Serializable]
    public class BoMonster : BoActor
    {
        public float patrolWaitStartTime; // 정찰 후 대기 시작 시간
        public float patrolWaitCheckTime; // 정찰 후 대기 체크 시간
        public Vector3 destPos; // 목적지 위치 (정찰 지점, 타겟 지점)

        public SDMonster sdMonster;

        /// <summary>
        /// 현재 생성자에 파라미터로 sd 데이터를 받는 형태만 존재함
        ///  -> dto를 받는 형태는 존재하지 않음
        ///     이게 의미하는 바는, 몬스터 데이터의 생성을 서버가 제어하지 않는다는 것
        ///     이 말은 클라이언트에서 몬스터를 생성하겠다는 뜻
        /// </summary>
        /// <param name="sdMonster"></param>
        public BoMonster(SDMonster sdMonster)
        {
            this.sdMonster = sdMonster;
        }
    }
}
