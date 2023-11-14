using System;

namespace AI_Project.DB
{
    /// <summary>
    /// 일반적으로 통신에 사용하는 Dto를 인게임에서 사용 시
    /// Bo로 변환하여 사용
    ///  -> 보안 및 Dto는 통신에 알맞는 형태로 데이터를 최적화하기 때문에
    ///     인게임에서 바로 사용할 수 없는 경우 등이 있어서..
    /// 
    /// Bo는 인게임 로직에서만 사용되므로, 직렬화할 필요가 없음
    /// 하지만, 작업과정에서 데이터를 편하게 확인하기 위해 인스펙터에 노출
    ///  -> 인스펙터 노출을 위해 직렬화
    /// </summary>
#if UNITY_EDITOR
    [Serializable]
    // 유니티 에디터에서만 해당 코드가 동작하도록
#endif
    public class BoAccount
    {
        public string nickname;
        public int gold;

        public BoAccount(DtoAccount dtoAccount)
        {
            nickname = dtoAccount.nickname;
            gold = dtoAccount.gold;
        }
    }
}