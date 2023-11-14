
using AI_Project.Dummy;

namespace AI_Project.Network
{
    /// <summary>
    /// 특정 조건에 따라 서버모듈을 생성할 팩토리
    /// </summary>
    public static class ServerModuleFactory
    {
        /// <summary>
        /// 조건에 따라 상황에 맞는 서버모듈을 생성 후,
        /// 해당 모듈에 구현된 프로토콜을 갖는 객체를 반환한다.
        /// </summary>
        /// <returns></returns>
        public static INetworkClient NewNetworkClientModule()
        {
            // 더미서버를 사용하고, 더미서버 인스턴스가 존재한다면
            if (GameManager.Instance.useDummyServer && DummyServer.Instance != null)
            {
                // 더미서버 모듈을 반환
                return DummyServer.Instance.dummyModule;
            }
            else
            { 
                // 테스트 서버 or 라이브 서버 등..

            }

            return null;
        }
    }
}
