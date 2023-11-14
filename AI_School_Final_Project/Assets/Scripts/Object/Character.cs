using AI_Project.DB;
using AI_Project.Network;
using AI_Project.UI;
using System.Collections;
using UnityEngine;

namespace AI_Project.Object
{
    public class Character : Actor
    {
        public BoCharacter boCharacter;

        private Coroutine addExpCoroutine;

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);

            // ������ ���� ���� ��� �����͸� ���̽��� ���·� �����ϱ� ������
            // ������ ������ ������ ���� �Ļ� Ŭ���� ���·� ĳ�����Ͽ� ��Ƶд�
            boCharacter = boActor as BoCharacter;

            SetStats();
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// ĳ���� ���� ����
        ///  -> ��ȹ �����Ϳ� ����������(����)�� ������� ������ �����ȴ�.
        /// </summary>
        public override void SetStats()
        {
            boCharacter.type = Define.Actor.Type.Character;
            boCharacter.atkType = boCharacter.sdCharacter.atkType;
            boCharacter.moveSpeed = boCharacter.sdCharacter.moveSpeed;
            boCharacter.atkInterval = boCharacter.sdCharacter.atkInterval;
            boCharacter.atkRange = boCharacter.sdCharacter.atkRange;
            boCharacter.currentHp = boCharacter.maxHp 
                = boCharacter.level * boCharacter.sdGrowthStat.maxHp * boCharacter.sdGrowthStat.maxHpFactor;
            boCharacter.currentMana = boCharacter.maxMana
                = boCharacter.level * boCharacter.sdGrowthStat.maxMana * boCharacter.sdGrowthStat.maxManaFactor;
            boCharacter.atk = boCharacter.level * boCharacter.sdGrowthStat.atk * boCharacter.sdGrowthStat.atkFactor;
            boCharacter.def = boCharacter.level * boCharacter.sdGrowthStat.def * boCharacter.sdGrowthStat.defFactor;
            boCharacter.maxExp = boCharacter.level * boCharacter.sdGrowthStat.maxExp * boCharacter.sdGrowthStat.maxExpFactor;

            UIWindowManager.Instance.GetWindow<UIIngame>().SetExp((int)boCharacter.level, boCharacter.currentExp / boCharacter.maxExp);
        }

        public override void SetState(Define.Actor.State state)
        {
            // ���������� ���� ���� ó��
            base.SetState(state);

            // ĳ���͸� ���� ���� ó��
            switch (state)
            {
                case Define.Actor.State.Jump:
                    OnJump();
                    break;
            }
        }

        public override void Execute()
        {
            CheckGround();
            attackController.CheckCooltime();

            base.Execute();
        }

        public override void MoveUpdate()
        {
            // �ӷ°� ������ ���� �ӵ��� ����
            var velocity = boCharacter.moveSpeed * boCharacter.moveDir;
            // �ӵ��� �ٷ� ĳ������ ���������ǿ� ���� ��, ���带 ���������� ���� ������ ����
            // ���带 ���������� z������ ������
            // �׸��� ���� ĳ���ʹ� ���콺�� ���� ȸ���ϹǷ�, �� �� ĳ������ ���� ���� ����� ������
            // ���� �ش� ���͸� ĳ������ ���� ���� �������� �� ������ ��ȯ��Ų��.
            velocity = transform.TransformDirection(velocity);

            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate(boCharacter.rotDir * Define.Camera.RotSpeed);

            // ���� ���¿��� ������ ��, �̵� ����� �ƴ� ���� ����� �״�� �����Ű�� ����
            if (State == Define.Actor.State.Jump || State == Define.Actor.State.Attack)
                return;

            // �ӵ� ������ ���̰� 0�� �ٻ��ϴٸ� �� �����δٴ� ��
            //  ����, ���¸� ���� ����, �ƴ϶�� �ȱ�� ����
            if (Mathf.Approximately(velocity.magnitude, 0))
            {
                SetState(Define.Actor.State.Idle);
            }
            else
            {
                SetState(Define.Actor.State.Walk);
            }
        }

        /// <summary>
        /// ĳ���Ͱ� ���� �ִ��� üũ�ϴ� ���
        /// </summary>
        private void CheckGround()
        {
            // ����ĳ������ �̿��ؼ� ĳ���Ͱ� ���� ��Ҵ��� Ȯ��
            boActor.isGround = Physics.Raycast(transform.position, Vector3.down, .1f, 
                1 << LayerMask.NameToLayer("Floor"));
            // << ��Ʈ ����Ʈ ����, ���� Floor ���̾ �浹 üũ�ϵ���

            // ���� ���°� ���� ���°� �ƴ϶�� ����
            if (State != Define.Actor.State.Jump)
                return;

            // �� ���� ���Դٴ� ���� ���� ���� ���¶�� ��
            // �׸��� �� ��, ���� isGround�� true��� ĳ���Ͱ� ���� �����ߴٴ� ��
            // ����, ĳ������ ���¸� ��� ���·� �����Ѵ�.
            if (boActor.isGround)
                SetState(Define.Actor.State.Idle);
        }

        /// <summary>
        /// ���� ���� ���� (���� Ű�� ������ �� �� �� ȣ��)
        /// </summary>
        private void OnJump()
        {
            rig.AddForce(Vector3.up * boCharacter.sdCharacter.jumpForce, ForceMode.Impulse);
        }

        /// <summary>
        /// ĳ������ ���� ����ġ�� �߰��ϴ� ���
        /// </summary>
        /// <param name="exp">�߰��Ǵ� ����ġ��</param>
        public void AddExp(float exp)
        {
            ServerManager.Server.AddExp(0, exp,
                new ResponseHandler<DtoCharacter>(dtoCharacter =>
                {
                    boCharacter.level = dtoCharacter.level;
                    boCharacter.currentExp = dtoCharacter.currentExp;
                    boCharacter.maxExp = boCharacter.level * boCharacter.sdGrowthStat.maxExp * boCharacter.sdGrowthStat.maxExpFactor;

                    UIWindowManager.Instance.GetWindow<UIIngame>().SetExpAnim((int)boCharacter.level, boCharacter.currentExp / boCharacter.maxExp);
                }, 
                failed => { }));

        }
    }
}