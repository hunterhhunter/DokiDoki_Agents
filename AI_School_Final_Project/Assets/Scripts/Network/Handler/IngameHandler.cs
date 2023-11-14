using AI_Project.DB;
using AI_Project.Object;
using AI_Project.UI;
using AI_Project.Util;
using System.Linq;
using UnityEngine;

namespace AI_Project.Network
{

    public class IngameHandler
    {
        public ResponseHandler<DtoAttackResult> attackHandler;
        public ResponseHandler<DtoDropItem> dropItemHandler;
        public ResponseHandler<DtoCheckQuest> checkHuntQuestHandler;

        public IngameHandler()
        {
            attackHandler = new ResponseHandler<DtoAttackResult>(OnAttackSuccess, OnFailed);
            dropItemHandler = new ResponseHandler<DtoDropItem>(OnDropItemSuccess, OnFailed);
            checkHuntQuestHandler = new ResponseHandler<DtoCheckQuest>(OnCheckHuntQuestSuccess, OnFailed);
        }

        private void OnCheckHuntQuestSuccess(DtoCheckQuest dtoCheckQuest)
        { 
            
        }

        private void OnDropItemSuccess(DtoDropItem dtoDropItem)
        {
            // 드롭하는 아이템이 없다면 종료
            if (dtoDropItem.index.Length == 0)
                return;

            // 아이템 풀 참조를 받음
            var itemPool = ObjectPoolManager.Instance.GetPool<Item>();
            // 월드 UI 캔버스도 받음
            var worldUICanvas = UIWindowManager.Instance.GetWindow<UIIngame>().worldUICanvas;

            for (int i = 0; i < dtoDropItem.index.Length; ++i)
            {
                // 아이템 풀에서 아이템 객체를 하나 가져옴
                var itemObj = itemPool.GetObj();

                // 아이템 객체의 부모를 월드 캔버스로 설정
                itemObj.transform.SetParent(worldUICanvas);
                itemObj.transform.localScale = Vector3.one * .5f;
                // 아이템의 위치를 몬스터의 위치로 설정
                itemObj.transform.position = new Vector3(dtoDropItem.dropPos[0], dtoDropItem.dropPos[1], dtoDropItem.dropPos[2])
                    + Vector3.up * .5f;
                // 아이템 초기화
                itemObj.Initialize(dtoDropItem.index[i]);

                // 설정이 끝난 아이템 객체를 활성화
                itemObj.gameObject.SetActive(true);
            }
        }

        private void OnAttackSuccess(DtoAttackResult dtoAttackResult)
        {
            var inGameManager = IngameManager.Instance;

            // 피격 대상에 따라 피격대상 타입에 맞는 목록을 참조
            var targets = dtoAttackResult.targetType == Define.Actor.Type.Character ? inGameManager.Characters : inGameManager.Monsters;

            // 피격 대상 목록에서 동일한 아이디를 갖는 객체를 찾음
            var target = targets.Where(_ => _.id == dtoAttackResult.targetId).SingleOrDefault();

            // 타겟이 존재한다면, 연산된 체력을 적용            
            if (target != null)
                target.boActor.currentHp = dtoAttackResult.targetHp;
        }

        private void OnFailed(DtoBase dtoError)
        { 
        
        }
    }
}
