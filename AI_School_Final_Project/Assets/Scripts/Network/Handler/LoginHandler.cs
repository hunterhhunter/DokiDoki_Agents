using AI_Project.DB;
using System.Collections.Generic;
using System.Linq;

namespace AI_Project.Network
{
    /// <summary>
    /// 로그인 시 필요한 데이터들을 서버에 요청하는 기능을 수행할 클래스
    /// </summary>
    public class LoginHandler
    {
        /// <summary>
        /// 요청 후 응답처리에 사용할 핸들러 객체
        /// </summary>
        public ResponseHandler<DtoAccount> accountResponse;
        public ResponseHandler<DtoCharacter> characterResponse;
        public ResponseHandler<DtoStage> stageResponse;
        public ResponseHandler<DtoItem> itemResponse;
        public ResponseHandler<DtoQuest> questResponse;

        public LoginHandler()
        {
            accountResponse = new ResponseHandler<DtoAccount>(GetAccuontSuccess, OnFailed);
            characterResponse = new ResponseHandler<DtoCharacter>(GetCharacterSuccess, OnFailed);
            stageResponse = new ResponseHandler<DtoStage>(GetStageSuccess, OnFailed);
            itemResponse = new ResponseHandler<DtoItem>(GetItemSuccess, OnFailed);
            questResponse = new ResponseHandler<DtoQuest>(GetQuestSuccess, OnFailed);
        }

        /// <summary>
        /// 로그인 요청(유저 데이터 요청) 기능
        /// </summary>
        public void Connect()
        {
            ServerManager.Server.Login(0, accountResponse);
            ServerManager.Server.GetCharacter(0, characterResponse);
            ServerManager.Server.GetStage(0, stageResponse);
            ServerManager.Server.GetItem(0, itemResponse);
            ServerManager.Server.GetQuest(0, questResponse);
        }

        /// <summary>
        /// 스테이지 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoStage"></param>
        public void GetStageSuccess(DtoStage dtoStage)
        {
            GameManager.User.boStage = new BoStage(dtoStage);
        }

        /// <summary>
        /// 캐릭터 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoCharacter"></param>
        public void GetCharacterSuccess(DtoCharacter dtoCharacter)
        {
            GameManager.User.boCharacter = new BoCharacter(dtoCharacter);
        }

        /// <summary>
        /// 계정 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoAccount"></param>
        public void GetAccuontSuccess(DtoAccount dtoAccount)
        {
            GameManager.User.boAccount = new BoAccount(dtoAccount);
        }

        /// <summary>
        /// 아이템 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoItem">서버에서 보내준 유저의 전체 아이템 정보</param>
        public void GetItemSuccess(DtoItem dtoItem)
        {
            GameManager.User.boItems = new List<BoItem>();
            var boItems = GameManager.User.boItems;

            for (int i = 0; i < dtoItem.items.Count; ++i)
            {
                // dto 개별 아이템 정보로 접근
                var dtoItemElement = dtoItem.items[i];
                BoItem boItem = null;

                // 전체 아이템 테이블에서 개별 아이템의 인덱스와 동일한 아이템 기획데이터가
                // 있다면 가져옴
                var sdItem = GameManager.SD.sdItems
                    .Where(_ => _.index == dtoItemElement.index).SingleOrDefault();

                // 아이템이 장비 타입인지 확인하여 bo 객체 생성을 다르게함
                if (sdItem.itemType == Define.Item.Type.Equipment)
                {
                    boItem = new BoEquipment(sdItem);
                    
                    var boEquipment = boItem as BoEquipment;
                    boEquipment.reinforceValue = dtoItemElement.reinforceValue;
                    boEquipment.isEquip = dtoItemElement.isEquip;
                }
                else
                {
                    boItem = new BoItem(sdItem);
                }

                boItem.slotIndex = dtoItemElement.slotIndex;
                boItem.amount = dtoItemElement.amount;

                // dto -> bo 변환이 끝난 데이터를 유저의 인게임 아이템 정보에 추가
                boItems.Add(boItem);
            }
        }

        public void GetQuestSuccess(DtoQuest dtoQuest)
        {
            GameManager.User.boQuest = new BoQuest(dtoQuest);
        }

        /// <summary>
        /// 요청 실패 시 실행할 메서드
        /// </summary>
        /// <param name="dtoBase"></param>
        public void OnFailed(DtoBase dtoBase)
        { 
        
        }
    }
}
