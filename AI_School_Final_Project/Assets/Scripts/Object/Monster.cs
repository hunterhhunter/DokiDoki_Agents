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
        /// �׺���̼� �ý����� ���� ������ ��θ� �̿��Ͽ� ��ã�⸦ �Ϸ���
        /// ��ã�⸦ �̿��ϴ� ��ü���� NavMeshAgent ������Ʈ�� �ʿ��� 
        ///  -> (�� �� ���Ͱ� ����� NavMeshAgent ������Ʈ ����)
        /// </summary>
        private NavMeshAgent agent;
        /// <summary>
        /// ��ã�� �ý����� �̿��Ͽ� Ư�� �������� ������Ʈ�� �̵���ų �� ����
        ///  -> ��ã�� �ý����� �̿��Ͽ� Ư�� ������������ ��θ� ������ �� �ִµ�
        ///     ��� ���꿡 ���� �����͸� ��Ƶ� �ʵ�
        /// </summary>
        private NavMeshPath path;

        public bool CanRecycle { get; set; } = true;

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);

            boMonster = boActor as BoMonster;

            SetStats();
            InitPatrolWaitTime();

            // �������� ������ ���� ��ġ�� �����Ͽ� �ٷ� ������ ���� ���·� �ν��ϰ� �Ͽ�
            // �ʱ⿡ ���Ͱ� �ٷ� ���ο� �������� �����Ͽ� �����ϰԲ� ����
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

            // ���Ͱ� ���� ��, �̵��� �� �� ������
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

                // �ӷ°� ������ ����
                agent.speed = boMonster.moveSpeed;
                agent.SetDestination(boMonster.destPos);
            }
            else
            {
                SetState(Define.Actor.State.Idle);
            }
        }

        /// <summary>
        /// ��Ȳ�� ���� �������� �����ϰ� �̵����θ� ��ȯ
        ///  -> �̵��̶�� true, �ƴ϶�� false
        /// </summary>
        /// <returns></returns>
        private bool GetMovement()
        {

            // ���°� ��� ���¶��
            if (State == Define.Actor.State.Idle)
            {
                // ��� �ð��� �������� Ȯ��
                if (Time.time - boMonster.patrolWaitStartTime >= boMonster.patrolWaitCheckTime)
                {
                    // �����ٸ� ���ð� �ʱ�ȭ �� �̵��� �� �ְ� true ��ȯ
                    InitPatrolWaitTime();
                    return true;
                }
                // ���ð��� ���� ������ �ʾҴٸ� �̵��� �� ������ false ��ȯ
                else
                    return false;
            }

            // �������� ���� ���� ��ġ�� ���̸� ���Ѵ�.
            var distance = (boMonster.destPos - transform.position).magnitude;

            // ���̰� ������Ʈ�� ������ �����Ÿ� ���϶��
            // ���� ��ġ�� �����ߴٴ� ��, ���� false�� ��ȯ���� ����ϵ���
            if (distance <= agent.stoppingDistance)
            {
                // �߰��� ������ ��������, ���� ��ġ�� ������ ������ �� �ֵ���
                // ���ο� �������� �������ش�.
                ChangeDestPos();

                return false;
            }

            return true;
        }

        /// <summary>
        /// ���� ��ġ�� �����ϴ� ���
        /// </summary>
        private void ChangeDestPos()
        {
            // ���͸��� ���������� �ٸ��Ƿ�, ���� �ε��� ���� �̿��Ͽ� �ش� ������
            // ���� ���� ������ ������ ��ġ�� �޴´�.
            boMonster.destPos = StageManager.Instance.GetRandPosInArea(boMonster.sdMonster.index);

            // �ش� ������������ ��� ���� (���� �̵��ϴ� ���� �ƴ�)
            var isExist = agent.CalculatePath(boMonster.destPos, path);

            // �ش� ��ġ�� �׺�޽� ��� �� �����ϴ� Ȯ��
            if (!isExist)
            {
                // �������� �ʴ´ٸ� �������� ���� �����ϰ� �ٽ� �˻��� �� �ֵ��� ��� ȣ��
                ChangeDestPos();
            }
            // �ش� ��ġ�� ��� �� ����������, �������� ������ �� ���� ���
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                ChangeDestPos();
            }
        }

        /// <summary>
        /// ���� ��� �ð� �ʱ�ȭ
        /// </summary>
        private void InitPatrolWaitTime()
        {
            boMonster.patrolWaitStartTime = Time.time;
            boMonster.patrolWaitCheckTime = Random.Range(Define.Monster.MinPatrolWaitTime, Define.Monster.MaxPatrolWaitTime);
        }

        /// <summary>
        /// ���� ���� �� ��(�÷��̾�)�� �ִ��� üũ
        /// </summary>
        private void CheckDetection()
        {
            // ���� ��ȹ ������ ��, ���� ���� ���� ������
            var extentsValue = boMonster.sdMonster.detectionRange;
            // ���� ���� ��(��Į��)�� �̿��Ͽ� 3������ �������� ����ŭ üũ ������ ����
            var halfExtents = new Vector3(extentsValue, extentsValue, extentsValue);

            // ���� ��ġ�� �������� halfExtents ��ŭ�� ������� �ڽ� ������ ���� (�ڽ��� ȸ���� ���� ȸ���� ����)
            // ���� ����� ĳ���� ���̾ ���� ��ü, �� �� �ڽ� �� ��ģ �ݶ��̴� ������ ��ȯ ��
            var colls = Physics.OverlapBox(transform.position, halfExtents, transform.rotation,
                1 << LayerMask.NameToLayer("Character"));

            attackController.hasTarget = colls.Length != 0;

            // ���� ���� �� �÷��̾ ���ٸ�
            if (!attackController.hasTarget)
                return;

            // �÷��̾ ������ �� �ֵ���, ������ �������� �÷��̾� ��ġ�� ����
            boMonster.destPos = colls[0].transform.position;
            // �÷��̾���� �Ÿ��� ���� ���� ���̶��, ���� ���� ���·� ����
            var disp = boMonster.destPos - transform.position;

            // �÷��̾���� �Ÿ��� ���ݹ��� ���϶��
            if (disp.magnitude <= boMonster.atkRange)
            {
                // ���� ���� ���·� ����
                attackController.canAtk = true;
                // ���Ͱ� ���� ���� ���°� �Ǹ�, �̵����� ������ �켱�� �Ǿ� �̵��̳� ȸ���� ���� �ʴ´�.
                // �׷� �� ��, ĳ���Ͱ� �̵� �� ���Ͱ� ĳ���͸� �ٶ� �� �ֵ��� ���Ͱ� �ٶ󺸴� ������ ����
                transform.rotation = Quaternion.LookRotation(disp.normalized);
            }
            else
            {
                attackController.canAtk = false;
            }
        }

        public override void OnDeadEnd()
        {
            // ����� ĳ���Ͱ� �ϳ��� �����Ƿ�, ���� �÷��̾� ��Ʈ�ѷ� ������ ã���ʰ�
            // �ΰ��ӸŴ����� �����Ͽ� �÷��̾� ĳ���� ������ ������..
            //  -> ĳ���Ͱ� ���� ����� �÷��̾� ��Ʈ�ѷ��� ���� ������ �� Ȯ���� ���� ���

            var playerCharacter = IngameManager.Instance.Characters[0] as Character;
            playerCharacter?.AddExp(boMonster.sdMonster.dropExp);

            //// ����� �����۵��� �ε����� ���� ����Ʈ
            //List<int> dropItemIndex = new List<int>(); 

            //// ���Ͱ� ����� �� �ִ� ������ ������ŭ �ݺ�
            //for (int i = 0; i < boMonster.sdMonster.dropItemRef.Length; ++i)
            //{
            //    // ������ ��� Ȯ�� ���
            //    var isDrop = boMonster.sdMonster.dropItemPer[i] <= Random.Range(0, 1f);

            //    // ����̶��
            //    if (isDrop)
            //        // ��� ������ ��Ͽ� ��´�.
            //        dropItemIndex.Add(boMonster.sdMonster.dropItemRef[i]);
            //}

            ServerManager.Server.DropItem(0, boMonster.sdMonster.index, transform.position, 
                IngameManager.Instance.IngameHandler.dropItemHandler);

            // ���� ��ü�� ������Ʈ Ǯ�� �ٽ� ��ȯ
            ObjectPoolManager.Instance.GetPool<Monster>().Return(this);

            //// ����ϴ� �������� ���ٸ� ����
            //if (dropItemIndex.Count == 0)
            //    return;

            //// ������ Ǯ ������ ����
            //var itemPool = ObjectPoolManager.Instance.GetPool<Item>();
            //// ���� UI ĵ������ ����
            //var worldUICanvas = UIWindowManager.Instance.GetWindow<UIIngame>().worldUICanvas;

            //for (int i = 0; i < dropItemIndex.Count; ++i)
            //{
            //    // ������ Ǯ���� ������ ��ü�� �ϳ� ������
            //    var itemObj = itemPool.GetObj();

            //    // ������ ��ü�� �θ� ���� ĵ������ ����
            //    itemObj.transform.SetParent(worldUICanvas);
            //    itemObj.transform.localScale = Vector3.one * .5f;
            //    // �������� ��ġ�� ������ ��ġ�� ����
            //    itemObj.transform.position = transform.position + Vector3.up * .5f;
            //    // ������ �ʱ�ȭ
            //    itemObj.Initialize(dropItemIndex[i]);

            //    // ������ ���� ������ ��ü�� Ȱ��ȭ
            //    itemObj.gameObject.SetActive(true);
            //}
        }
    }
}