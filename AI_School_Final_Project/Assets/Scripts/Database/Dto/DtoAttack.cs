using AI_Project.Define;
using AI_Project.Network;
using System;


namespace AI_Project.DB
{
    /// <summary>
    /// 공격에 관련된 통신 데이터
    /// 이전 dto와는 성질이 조금 다름
    /// 이전까지 정의한 dto는 통신 후 모두 db에 저장되는 데이터였다면]
    /// 해당 데이터는 서버에서 연산에만 사용되고, db에 저장되지는 않음
    /// </summary>
    [Serializable]
    public class DtoAttack : DtoBase
    {
        // 현재 클라이언트에서 몬스터를 생성하고 있으며
        // 서버에는 몬스터에 관한 데이터가 존재하지 않음
        // 따라서, 서버는 특정 몬스터를 분별해낼 수 없다.
        //  -> 동일한 종류의 몬스터 객체를 구별하기 위해 몬스터에게 임의의
        //     인스턴스 아이디를 부여한다.
        // 이후 해당 아이디와 몬스터의 체력,방어력 및 공격자가 가한 데미지에
        // 관한 데이터를 전달함으로써 서버에서는 데미지 연산을 할 수 있게됨
        // 연산 후, 서버는 해당 몬스터의 아이디와 변경된 체력을 클라에 다시
        // 반환함으로써 클라에서는 아이디를 기반으로 해당 몬스터를 찾고
        // 변경된 체력만 적용시키면 됨

        public Actor.Type targetType;
        public int targetId;
        public float targetHp;
        public float targetDef;
        public float damage;
    }

    [Serializable]
    public class DtoAttackResult : DtoBase
    {
        public Actor.Type targetType;
        public int targetId;
        public float targetHp;
    }
}
