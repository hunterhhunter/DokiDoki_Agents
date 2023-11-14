using Newtonsoft.Json;
using AI_Project.DB;
using AI_Project.Network;
using AI_Project.SD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI_Project.Dummy
{
    /// <summary>
    /// 실제 더미서버에서의 통신 프로토콜 구현부를 갖는 클래스
    /// </summary>
    public class ServerModuleDummy : INetworkClient
    {
        private DummyServer serverData;
        private Coroutine addExpCoroutine;

        public ServerModuleDummy(DummyServer serverData)
        {
            // db가 더미서버에 존재하고, 더미서버모듈은 더미서버에서 생성되므로
            // 처음 생성 시, 생성 주체(더미서버)의 참조를 받아서 이후 데이터 처리 시에
            // 사용하게 한다.
            this.serverData = serverData;
        }

        public void AddExp(int uniqueId, float addExp, ResponseHandler<DtoCharacter> responseHandler)
        {
            var dtoCharacter = serverData.userData.dtoCharacter;

            dtoCharacter.currentExp += addExp;

            if (addExpCoroutine != null)
            {
                DummyServer.Instance.StopCoroutine(addExpCoroutine);
                addExpCoroutine = null;
            }

            // 서버는 bo 데이터를 갖지 않으므로 현재 최대경험치에 대한 데이터가 존재하지 않음
            // 따라서, dto에 있는 캐릭터 인덱스를 통해 기획 테이블에서 내 캐릭터와 캐릭터에 대한 성장스텟 정보를 가져온다.
            // 결과, 내 캐릭터의 현재 레벨에 맞는 최대경험치를 구할 수 있음
            var sdCharacter = GameManager.SD.sdCharacters.Where(_ => _.index == dtoCharacter.index).SingleOrDefault();
            var sdGrowthStat = GameManager.SD.sdGrowthStats.Where(_ => _.index == sdCharacter.growthStatRef).SingleOrDefault();

            var maxExp = dtoCharacter.level * sdGrowthStat.maxExp * sdGrowthStat.maxExpFactor;

            addExpCoroutine = DummyServer.Instance.StartCoroutine(GetLevelUpCount());

            // 경험치 추가 시, 추가된 경험치에 의해 몇 번의 레벨업이 이루어지는 구하는 기능
            IEnumerator GetLevelUpCount()
            {
                // 현재 레벨의 최대 경험치에서 현재 경험치를 빼서 잔여경험치를 구한다.
                var remainingExp = maxExp - dtoCharacter.currentExp;

                // 잔여 경험치가 0 이하라는 것은 레벨업이 가능하다는 뜻
                while (remainingExp <= 0)
                {
                    // 레벨 1 증가
                    ++dtoCharacter.level;
                    // 잔여 경험치를 현재 경험치에 적용
                    dtoCharacter.currentExp = Mathf.Abs(remainingExp);
                    // 증가한 레벨에 맞는 최대 경험치를 다시 구함
                    maxExp = dtoCharacter.level * sdGrowthStat.maxExp * sdGrowthStat.maxExpFactor;
                    // 갱신된 최대경험치와 현재경험치를 이용하여 다시 잔여 경험치를 구한다.
                    remainingExp = maxExp - dtoCharacter.currentExp;

                    yield return null;
                }

                // 변경된 사항을 저장
                DummyServer.Instance.Save();
                // 클라에 변경된 데이터를 보내준다.
                responseHandler.HandleSuccess(JsonUtility.ToJson(dtoCharacter));
            }
        }

        public void AddQuest(int uniqueId, int questIndex, ResponseHandler<DtoQuestProgress> responseHandler)
        {
            // 서버에서는 이런식으로 처리를 하고 있음

            // 퀘스트 인덱스를 통해 기획 퀘스트 테이블에서 해당 퀘스트 정보를 받아옴
            //  -> 서버도 기획 데이터를 가지고 있다는 뜻
            var sdQuest = GameManager.SD.sdQuests.Where(_ => _.index == questIndex).SingleOrDefault();

            // db에 저장하고, 클라이언트 측에 반환해줄 데이터를 생성
            var dtoQuestProgress = new DtoQuestProgress();

            dtoQuestProgress.index = sdQuest.index;

            // 진행 퀘스트 디테일 정보는 현재 퀘스트의 종류에 따라 달라짐
            switch (sdQuest.type)
            {
                // 사냥, 수집 종류의 퀘스트는
                //  사냥 -> 몇 종류의 몬스터를 몇 마리 잡아라
                //  수집 -> 몇 종류의 아이템을 몇 개 가져와라
                //  -> 결과적으로 종류만큼 디테일 배열의 길이를 설정
                case Define.Quest.QuestType.Hunt:
                case Define.Quest.QuestType.Collect:
                    Array.Resize(ref dtoQuestProgress.details, sdQuest.target.Length);
                    Array.ForEach(dtoQuestProgress.details, _ => _ = 0);
                    break;
                // 특정 지역을 탐사(특정 좌표)해라
                // 특정 지역 도착 판단 방법
                //  -> 특정 지역에 클라쪽에서 콜라이더 트리거가 존재해서 해당 트리거에 들어갔을 때, 판단
                //  ->  or 디테일 배열에서 특정 지역의 위치를 x,y,z로 갖고, 실제 유저 위치와 비교해서 근사하다면 완료

                // 퀘스트 기획 테이블
                // 타겟 값은 특정지역(스테이지 인덱스)
                // 디테일 값은 해당 스테이지에서 x,y,z 위치값

                // 유저 db에서의 디테일 값
                // 0이면 미도착, 1이면 도착
                case Define.Quest.QuestType.Adventure:
                    dtoQuestProgress.details = new int[] { 0 };
                    break;
            }

            // db에 dtoQuest.progressQuests에 위에 생성한 새로운 진행 퀘스트에 대한 정보를 추가
            // dtoQuest.progressQuests가 배열로 되어있으니까 배열을 리사이징해서
            // 방금 생성한 데이터를 배열의 마지막 인덱스 공간에 넣고 있음
            var length = serverData.userData.dtoQuest.progressQuests.Length + 1;
            Array.Resize(ref serverData.userData.dtoQuest.progressQuests, length);
            serverData.userData.dtoQuest.progressQuests[length - 1] = dtoQuestProgress;

            // db 데이터를 수정했으니까 저장
            DummyServer.Instance.Save();

            // 클라에도 생성한 데이터를 보내준다.
            responseHandler.HandleSuccess(JsonConvert.SerializeObject(serverData.userData.dtoQuest.progressQuests[length - 1]));
        }

        public void CalculateDamage(int uniqueId, DtoAttack dtoAttack, ResponseHandler<DtoAttackResult> responseHandler)
        {
            // 데미지 계산 (공격자의 데미지에서 피격자의 방어력을 뺄게요)
            var calDamage = Mathf.Max(dtoAttack.damage - dtoAttack.targetDef, 0);

            var dtoAttackResult = new DtoAttackResult();
            // 현재 받아온 타겟 아이디를 그대로 반환하고 있어 딱히 타겟 아이디를 가지고 하고 있는 작업은 없음
            //  -> 하지만 몬스터 생성을 서버에서 처리한다는 가정하에, 몬스터 생성을 서버에서 함으로써
            //     서버가 몬스터 데이터를 들고 있다면 해당 타겟 아이디를 기반으로 몬스터 데이터를 찾아서 연산을 처리할 수 있음
            dtoAttackResult.targetType = dtoAttack.targetType;
            dtoAttackResult.targetId = dtoAttack.targetId;
            // 계산된 데미지를 타겟의 현재 체력에 적용
            dtoAttackResult.targetHp = Mathf.Max(dtoAttack.targetHp - calDamage, 0);

            responseHandler.HandleSuccess(JsonConvert.SerializeObject(dtoAttackResult));
        }

        public void CheckHuntQuest(int uniqueId, int monsterIndex, ResponseHandler<DtoCheckQuest> responseHandler)
        {
            // db에서 진행중인 퀘스트의 인덱스를 전부 가져옴
            var progressQuestIndices = serverData.userData.dtoQuest.progressQuests.Select(_ => _.index).ToArray();

            // 퀘스트 인덱스를 기반으로 진행중인 사냥 퀘스트 기획데이터를 전부 가져옴
            var sdHuntQuests = GameManager.SD.sdQuests.Where(_ => _.type == Define.Quest.QuestType.Hunt).ToList();
            var progressSDQuests = new List<SDQuest>();
            for (int i = 0; i < progressQuestIndices.Length; ++i)
            {
                var progressSDQuest = sdHuntQuests.Where(_ => _.index == progressQuestIndices[i]).SingleOrDefault();
                progressSDQuests.Add(progressSDQuest);
            }

            // 진행중인 퀘스트 중, 파라미터로 받은 몬스터 인덱스와 연관된 퀘스트만을 추림
            List<KeyValuePair<SDQuest, int>> targetSDQuests = new List<KeyValuePair<SDQuest, int>>();

            for (int i = 0; i < progressSDQuests.Count; ++i)
            {
                var index = Array.IndexOf(progressSDQuests[i].target, monsterIndex);
                if (index >= 0)
                {
                    targetSDQuests.Add(new KeyValuePair<SDQuest, int>(progressSDQuests[i], index));
                }
            }


            var targetProgressQuests =
                from tagetSDQuest in targetSDQuests
                join progressQuest in serverData.userData.dtoQuest.progressQuests
                on tagetSDQuest.Key.index equals progressQuest.index
                let newDetail = 
                progressQuest.details[tagetSDQuest.Value] += 1
                let isCompleted = 
                 (progressQuest.details.Intersect(tagetSDQuest.Key.targetDetail).Count() 
                == tagetSDQuest.Key.targetDetail.Length ? true : false)
                select new DtoCheckQuest { isCompleted = isCompleted, dtoQuestProgress = progressQuest };
        }

        public void DropItem(int uniqueId, int monsterIndex, Vector3 dropPos, ResponseHandler<DtoDropItem> responseHandler)
        {
            // 전달받은 몬스터 인덱스를 기반으로 몬스터 기획 데이터를 찾음
            var sdMonster = GameManager.SD.sdMonsters.Where(_ => _.index == monsterIndex).SingleOrDefault();

            // 드롭할 아이템들의 인덱스를 갖는 리스트
            List<int> dropItemIndex = new List<int>();

            // 몬스터가 드랍할 수 있는 아이템 종류만큼 반복
            for (int i = 0; i < sdMonster.dropItemRef.Length; ++i)
            {
                // 아이템 드롭 확률 계산
                var isDrop = sdMonster.dropItemPer[i] <= Random.Range(0, 1f);

                // 드롭이라면
                if (isDrop)
                    // 드롭 아이템 목록에 담는다.
                    dropItemIndex.Add(sdMonster.dropItemRef[i]);
            }

            DtoDropItem dtoDropItem = new DtoDropItem()
            {
                dropPos = new float[] { dropPos.x, dropPos.y, dropPos.z },
                index = dropItemIndex.ToArray(),
                amount = new int[dropItemIndex.Count],
            };
            for (int i = 0; i < dtoDropItem.amount.Length; ++i)
                dtoDropItem.amount[i] = 1;

            responseHandler.HandleSuccess(JsonConvert.SerializeObject(dtoDropItem));
        }

        public void GetCharacter(int uniqueId, ResponseHandler<DtoCharacter> responseHandler)
        {
            responseHandler.HandleSuccess(JsonConvert.SerializeObject(serverData.userData.dtoCharacter));
        }

        public void GetItem(int uniqueId, ResponseHandler<DtoItem> responseHandler)
        {
            responseHandler.HandleSuccess(JsonConvert.SerializeObject(serverData.userData.dtoItem));
        }

        public void GetQuest(int uniqueId, ResponseHandler<DtoQuest> responseHandler)
        {
            responseHandler.HandleSuccess(JsonConvert.SerializeObject(serverData.userData.dtoQuest));
        }

        public void GetStage(int uniqueId, ResponseHandler<DtoStage> responseHandler)
        {
            responseHandler.HandleSuccess(JsonConvert.SerializeObject(serverData.userData.dtoStage));
        }

        public void Login(int uniqueId, ResponseHandler<DtoAccount> responseHandler)
        {
            // 더미서버에서는 계정정보를 요청해서 어떻게 처리할 것인가에 대해 작성
            // 더미서버이므로 클라이언트에서 클라이언트로의 요청을 하는 것 (원맨쇼)
            // 통신 요청에 대한 실패가 발생할 일이 없음
            //  -> 강제로 요청 성공 메서드를 실행시켜서 데이터(DB에 있는 데이터)를 넘겨줌
            responseHandler.HandleSuccess(JsonConvert.SerializeObject(serverData.userData.dtoAccount));
        }
    }
}
