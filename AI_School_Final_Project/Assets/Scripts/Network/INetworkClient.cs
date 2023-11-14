
using AI_Project.DB;
using UnityEngine;

namespace AI_Project.Network
{
    /// <summary>
    /// 모든 서버 모듈은 해당 인터페이스를 상속받는다.
    /// 해당 인터페이스는 서버와 통신하는 프로토콜 메서드를 갖는다.
    /// 프로토콜이란?
    /// 서버와 클라이언트 간에 통신에 사용되는 API
    /// </summary>
    public interface INetworkClient
    {
        /// <summary>
        /// 서버에 계정 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId">
        /// 서버에 계정 정보 요청 시, 어떤 유저에 대한 계정 정보를 원하는지?
        ///  각 계정마다 부여된 고유한 키 값을 통해 유저를 식별
        /// </param>
        /// <param name="responseHandler">
        ///  서버에 요청한 데이터를 받았을 때, 어떻게 처리할 것인지에 대한 정보를 갖는 핸들러
        /// </param>
        void Login(int uniqueId, ResponseHandler<DtoAccount> responseHandler);

        /// <summary>
        /// 서버에 캐릭터 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responseHandler"></param>
        void GetCharacter(int uniqueId, ResponseHandler<DtoCharacter> responseHandler);

        /// <summary>
        /// 서버에 스테이지 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responseHandler"></param>
        void GetStage(int uniqueId, ResponseHandler<DtoStage> responseHandler);

        /// <summary>
        /// 서버에 유저의 아이템 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responseHandler"></param>
        void GetItem(int uniqueId, ResponseHandler<DtoItem> responseHandler);

        /// <summary>
        /// 서버에 유저의 퀘스트 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="responseHandler"></param>
        void GetQuest(int uniqueId, ResponseHandler<DtoQuest> responseHandler);

        /// <summary>
        /// 서버에 유저 db에 새로운 퀘스트 정보 추가를 요청하는 메서드 
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="questIndex">추가할 진행 퀘스트의 인덱스</param>
        /// <param name="responseHandler">요청 성공 시, 진행퀘스트 정보를 반환</param>
        void AddQuest(int uniqueId, int questIndex, ResponseHandler<DtoQuestProgress> responseHandler);

        /// <summary>
        /// 서버에 유저의 캐릭터 경험치 추가를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="addExp"></param>
        /// <param name="responseHandler"></param>
        void AddExp(int uniqueId, float addExp, ResponseHandler<DtoCharacter> responseHandler);

        /// <summary>
        /// 서버에 특정 몬스터에 대한 데미지 연산을 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name=""></param>
        void CalculateDamage(int uniqueId, DtoAttack dtoAttack, ResponseHandler<DtoAttackResult> responseHandler);

        /// <summary>
        /// 특정 몬스터 사망 시, 서버에 특정 몬스터에 대한 아이템 드랍 확률 연산을 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="monsterIndex"></param>
        /// <param name="responseHandler"></param>
        void DropItem(int uniqueId, int monsterIndex, Vector3 dropPos, ResponseHandler<DtoDropItem> responseHandler);

        /// <summary>
        /// 몬스터 사냥 시, 해당 몬스터와 연관된 퀘스트 진행여부를 체크하고, 연관된 퀘스트를 진행 중이라면 진행사항을 갱신 요청
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="monsterIndex"></param>
        /// <param name="responseHandler"></param>
        void CheckHuntQuest(int uniqueId, int monsterIndex, ResponseHandler<DtoCheckQuest> responseHandler);
    }
}
