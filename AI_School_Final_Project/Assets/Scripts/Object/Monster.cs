using AI_Project.DB;
using AI_Project.Network;
using AI_Project.UI;
using AI_Project.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI_Project.Object
{
    public class Monster : Actor, IPoolableObject
    {
        public BoMonster boMonster;

        /// <summary>
        /// 네비게이션 시스템을 통해 구워둔 경로를 이용하여 길찾기를 하려면
        /// 길찾기를 이용하는 객체에는 NavMeshAgent 컴포넌트가 필요함 
        ///  -> (이 때 몬스터가 사용할 NavMeshAgent 컴포넌트 참조)
        /// </summary>
        private NavMeshAgent agent;
        /// <summary>
        /// 길찾기 시스템을 이용하여 특정 목적지로 에이전트를 이동시킬 수 있음
        ///  -> 길찾기 시스템을 이용하여 특정 목적지까지의 경로를 연산할 수 있는데
        ///     경로 연산에 관한 데이터를 담아둘 필드
        /// </summary>
        private NavMeshPath path;

        public bool CanRecycle { get; set; } = true;

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);

            boMonster = boActor as BoMonster;

            SetStats();
            InitPatrolWaitTime();

            // 목적지를 몬스터의 현재 위치로 설정하여 바로 목적지 도착 상태로 인식하게 하여
            // 초기에 몬스터가 바로 새로운 목적지를 설정하여 정찰하게끔 유도
            boMonster.destPos = transform.position;
        }

        protected override void Start()
        {
            base.Start();

            agent = GetComponent<NavMeshAgent>();
            path = new NavMeshPath();
        }


        public override void SetStats()
        {
            if (boMonster == null)
                return;

            boMonster.level = 1;
            boMonster.type = Define.Actor.Type.Monster;
            boMonster.atkType = boMonster.sdMonster.atkType;
            boMonster.moveSpeed = boMonster.sdMonster.moveSpeed;
            boMonster.currentHp = boMonster.maxHp = boMonster.sdMonster.maxHp;
            boMonster.currentMana = boMonster.maxMana = boMonster.sdMonster.maxMana;
            boMonster.atkRange = boMonster.sdMonster.atkRange;
            boMonster.atkInterval = boMonster.sdMonster.atkInterval;
            boMonster.atk = boMonster.sdMonster.atk;
            boMonster.def = boMonster.sdMonster.def;
        }

        public override void Execute()
        {
            CheckDetection();
            attackController.CheckCooltime();
            if (attackController.CanAttack())
                SetState(Define.Actor.State.Attack);

            // 몬스터가 공격 중, 이동을 할 수 없도록
            if (State == Define.Actor.State.Attack)
                return;

            base.Execute();
        }

        public override void MoveUpdate()
        {
            var isMove = GetMovement();

            if (isMove)
            {
                SetState(Define.Actor.State.Walk);

                // 속력과 목적지 설정
                agent.speed = boMonster.moveSpeed;
                agent.SetDestination(boMonster.destPos);
            }
            else
            {
                SetState(Define.Actor.State.Idle);
            }
        }

        /// <summary>
        /// 상황에 따라 움직임을 설정하고 이동여부를 반환
        ///  -> 이동이라면 true, 아니라면 false
        /// </summary>
        /// <returns></returns>
        private bool GetMovement()
        {

            // 상태가 대기 상태라면
            if (State == Define.Actor.State.Idle)
            {
                // 대기 시간이 끝났는지 확인
                if (Time.time - boMonster.patrolWaitStartTime >= boMonster.patrolWaitCheckTime)
                {
                    // 끝났다면 대기시간 초기화 및 이동할 수 있게 true 반환
                    InitPatrolWaitTime();
                    return true;
                }
                // 대기시간이 아직 지나지 않았다면 이동할 수 없도록 false 반환
                else
                    return false;
            }

            // 목적지와 현재 몬스터 위치의 길이를 구한다.
            var distance = (boMonster.destPos - transform.position).magnitude;

            // 길이가 에이전트에 설정된 정지거리 이하라면
            // 정찰 위치에 도착했다는 뜻, 따라서 false를 반환시켜 대기하도록
            if (distance <= agent.stoppingDistance)
            {
                // 추가로 정찰이 끝났으니, 다음 위치로 정찰을 진행할 수 있도록
                // 새로운 목적지를 설정해준다.
                ChangeDestPos();

                return false;
            }

            return true;
        }

        /// <summary>
        /// 정찰 위치를 변경하는 기능
        /// </summary>
        private void ChangeDestPos()
        {
            // 몬스터마다 스폰구역이 다르므로, 몬스터 인덱스 값을 이용하여 해당 몬스터의
            // 스폰 구역 내에서 랜덤한 위치를 받는다.
            boMonster.destPos = StageManager.Instance.GetRandPosInArea(boMonster.sdMonster.index);

            // 해당 목적지까지의 경로 연산 (실제 이동하는 것은 아님)
            var isExist = agent.CalculatePath(boMonster.destPos, path);

            // 해당 위치가 네비메쉬 경로 상에 존재하는 확인
            if (!isExist)
            {
                // 존재하지 않는다면 목적지를 새로 지정하고 다시 검사할 수 있도록 재귀 호출
                ChangeDestPos();
            }
            // 해당 위치가 경로 상에 존재하지만, 목적지에 도착할 수 없는 경우
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                ChangeDestPos();
            }
        }

        /// <summary>
        /// 정찰 대기 시간 초기화
        /// </summary>
        private void InitPatrolWaitTime()
        {
            boMonster.patrolWaitStartTime = Time.time;
            boMonster.patrolWaitCheckTime = Random.Range(Define.Monster.MinPatrolWaitTime, Define.Monster.MaxPatrolWaitTime);
        }

        /// <summary>
        /// 감지 범위 내 적(플레이어)가 있는지 체크
        /// </summary>
        private void CheckDetection()
        {
            // 몬스터 기획 데이터 중, 감지 범위 값을 가져옴
            var extentsValue = boMonster.sdMonster.detectionRange;
            // 감지 범위 값(스칼라)를 이용하여 3축으로 감지범위 값만큼 체크 영역을 만듬
            var halfExtents = new Vector3(extentsValue, extentsValue, extentsValue);

            // 몬스터 위치를 기준으로 halfExtents 만큼의 사이즈로 박스 영역을 생성 (박스의 회전은 몬스터 회전과 동일)
            // 감지 대상은 캐릭터 레이어를 갖는 객체, 이 때 박스 내 겹친 콜라이더 정보가 반환 됨
            var colls = Physics.OverlapBox(transform.position, halfExtents, transform.rotation,
                1 << LayerMask.NameToLayer("Character"));

            attackController.hasTarget = colls.Length != 0;

            // 감지 범위 내 플레이어가 없다면
            if (!attackController.hasTarget)
                return;

            // 플레이어를 추적할 수 있도록, 몬스터의 목적지를 플레이어 위치로 변경
            boMonster.destPos = colls[0].transform.position;
            // 플레이어와의 거리가 공격 범위 안이라면, 공격 가능 상태로 변경
            var disp = boMonster.destPos - transform.position;

            // 플레이어와의 거리가 공격범위 이하라면
            if (disp.magnitude <= boMonster.atkRange)
            {
                // 공격 가능 상태로 변경
                attackController.canAtk = true;
                // 몬스터가 공격 가능 상태가 되면, 이동보다 공격이 우선시 되어 이동이나 회전을 하지 않는다.
                // 그럼 이 때, 캐릭터가 이동 시 몬스터가 캐릭터를 바라볼 수 있도록 몬스터가 바라보는 방향을 변경
                transform.rotation = Quaternion.LookRotation(disp.normalized);
            }
            else
            {
                attackController.canAtk = false;
            }
        }

        public override void OnDeadEnd()
        {
            // 현재는 캐릭터가 하나만 있으므로, 굳이 플레이어 컨트롤러 참조를 찾지않고
            // 인게임매니저로 접근하여 플레이어 캐릭터 참조를 가져옴..
            //  -> 캐릭터가 여러 개라면 플레이어 컨트롤러를 통한 참조가 더 확실한 접근 방법

            var playerCharacter = IngameManager.Instance.Characters[0] as Character;
            playerCharacter?.AddExp(boMonster.sdMonster.dropExp);

            //// 드롭할 아이템들의 인덱스를 갖는 리스트
            //List<int> dropItemIndex = new List<int>(); 

            //// 몬스터가 드랍할 수 있는 아이템 종류만큼 반복
            //for (int i = 0; i < boMonster.sdMonster.dropItemRef.Length; ++i)
            //{
            //    // 아이템 드롭 확률 계산
            //    var isDrop = boMonster.sdMonster.dropItemPer[i] <= Random.Range(0, 1f);

            //    // 드롭이라면
            //    if (isDrop)
            //        // 드롭 아이템 목록에 담는다.
            //        dropItemIndex.Add(boMonster.sdMonster.dropItemRef[i]);
            //}

            ServerManager.Server.DropItem(0, boMonster.sdMonster.index, transform.position, 
                IngameManager.Instance.IngameHandler.dropItemHandler);

            // 몬스터 객체를 오브젝트 풀에 다시 반환
            ObjectPoolManager.Instance.GetPool<Monster>().Return(this);

            //// 드롭하는 아이템이 없다면 종료
            //if (dropItemIndex.Count == 0)
            //    return;

            //// 아이템 풀 참조를 받음
            //var itemPool = ObjectPoolManager.Instance.GetPool<Item>();
            //// 월드 UI 캔버스도 받음
            //var worldUICanvas = UIWindowManager.Instance.GetWindow<UIIngame>().worldUICanvas;

            //for (int i = 0; i < dropItemIndex.Count; ++i)
            //{
            //    // 아이템 풀에서 아이템 객체를 하나 가져옴
            //    var itemObj = itemPool.GetObj();

            //    // 아이템 객체의 부모를 월드 캔버스로 설정
            //    itemObj.transform.SetParent(worldUICanvas);
            //    itemObj.transform.localScale = Vector3.one * .5f;
            //    // 아이템의 위치를 몬스터의 위치로 설정
            //    itemObj.transform.position = transform.position + Vector3.up * .5f;
            //    // 아이템 초기화
            //    itemObj.Initialize(dropItemIndex[i]);

            //    // 설정이 끝난 아이템 객체를 활성화
            //    itemObj.gameObject.SetActive(true);
            //}
        }
    }
}