using AI_Project.Network;
using System;

namespace AI_Project.DB
{
    [Serializable]
    public class DtoStage : DtoBase
    {
        /// <summary>
        /// 플레이어가 마지막으로 위치한 스테이지의 기획데이터 인덱스
        /// </summary>
        public int index;

        /// <summary>
        /// 플레이어가 마지막으로 위치한 스테이지 상에서의 좌표
        ///  -> 좌표는 유니티 상에서 벡터3로 표현하므로 벡터3 형태로
        ///     전달하면 될텐데, 개별 스칼라값으로 전달하는 이유?
        ///     서버에는 유니티 엔진 라이브러리를 사용하지 않으므로 Vector3
        ///     구조체에 대한 정보가 없음, 따라서 기본 데이터 형태로 보냄
        ///     이 때 만약 서버에서 별도로 특정 데이터타입을 따로 구현했다면 
        ///     클라에서도 동일한 형태로 정의하여 통신에 사용가능
        ///     ex) CustomVector3 
        /// </summary>
        public float posX;
        public float posY;
        public float posZ;
    }
}
