using AI_Project.Util;
using UnityEngine;

namespace AI_Project.Network
{
    /// <summary>
    /// 클라이언트 내 전체적인 서버 통신을 관리하는 매니저
    /// 상황에 따라 서버 모듈을 생성하여 통신을 처리한다.
    /// 여기서 말하는 상황에 따른 서버 모듈이란?
    ///  ex) 라이브 서버라면 라이브서버 모듈, 더미서버라면 더미서버 모듈 생성 
    /// </summary>
    public class ServerManager : Singleton<ServerManager>
    {
        /// <summary>
        /// 상황에 맞는 서버모듈을 갖는 필드
        /// </summary>
        private INetworkClient netClient;
        public static INetworkClient Server => Instance.netClient;

        public void Initialize()
        {
            // 서버 모듈 팩토리를 통해 현재 상황에 맞는 서버모듈을 반환받는다.
            netClient = ServerModuleFactory.NewNetworkClientModule();
        }
    }
}