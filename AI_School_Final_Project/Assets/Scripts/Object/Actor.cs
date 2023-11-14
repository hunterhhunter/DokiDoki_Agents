using AI_Project.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.Object
{
    using State = Define.Actor.State;

    /// <summary>
    /// �ΰ��� ������ ���̳����ϰ� �ൿ�ϴ� ��ü���� �߻�ȭ�� ���̽� Ŭ����
    /// ĳ����, ���� ��
    /// Actor�� �Ļ�Ŭ�������� ����Ǵ� ����� �ִ��� Actor�� ����
    /// �Ļ�Ŭ������ ���� �ٸ� ����� �ش� �Ļ�Ŭ�������� ����
    /// </summary>
    public abstract class Actor : MonoBehaviour
    {
        public int id;
        /// <summary>
        /// �ִϸ����� ���� ��� ���Ǵ� state ������ hash ��
        /// </summary>
        protected int animStateHash;
        /// <summary>
        /// ������ ���� ����
        /// </summary>
        public State State;

        /// <summary>
        /// ������ bo ������ ����
        /// </summary>
        public BoActor boActor;

        // �������� ��ü�� ��Ÿ���� Ŭ������ ��ü�� ����� �����Ͽ� �ۼ�����
        // ������, ��ü�� ��ü�� ����� �и��Ͽ� �ۼ��� �� �ִٸ� �ִ��� �и��Ͽ� �ۼ��ϴ� ����
        // ���α׷��� Ȯ�忡 ������ ��, ��� ���꼺�� ���� ������ �� ����
        //  -> �������� �κи� ���� ���� ���� ���� �Ǵ�
        /// <summary>
        /// ������ ���� ����� ������ ��Ʈ�ѷ� ��ü ����
        /// </summary>
        public AttackController attackController;

        /// <summary>
        /// ���Ͱ� ���������� ����ϴ� ������Ʈ���� ����
        /// </summary>
        public Collider Coll { get; private set; }
        protected Rigidbody rig;
        protected Animator anim;

        /// <summary>
        /// �ʱ�ȭ ��, �ܺο��� boActor �����͸� ���Թ޴´�.
        /// </summary>
        /// <param name="boActor"></param>
        public virtual void Initialize(BoActor boActor)
        {
            State = State.Idle;
            this.boActor = boActor;

            // attackController ������ �����ϴ��� Ȯ�� �� ���ٸ� ����
            attackController ??= gameObject.AddComponent<AttackController>();
            // ���� ��Ʈ�ѷ��� ������ ������ ����(������ ���)�Ͽ� �ʱ�ȭ 
            attackController.Initialize(this);
        }

        protected virtual void Start()
        {
            // ���͵��� ����ϴ� ������Ʈ���� ������ �޴´�
            Coll = GetComponent<Collider>();
            rig = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            animStateHash = Animator.StringToHash("state");
        }

        /// <summary>
        /// ���� ���� ���� �޼���
        /// </summary>
        public virtual void SetStats() { }

        /// <summary>
        /// ���� ������Ʈ
        /// </summary>
        public virtual void Execute()
        {
            MoveUpdate();
        }

        /// <summary>
        /// �̵� ������Ʈ
        /// </summary>
        public virtual void MoveUpdate() { }

        /// <summary>
        /// ���� ���� ���� ���
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetState(State state)
        {


            State = state;
            anim.SetInteger(animStateHash, (int)state);

            // ������ �Ļ� ��ü���� �������� ���¸��� ���̽����� ó��
            // �� �� �Ļ� ��ü�� ����, ���������� ���� ���´� �ش� �Ļ�Ŭ�������� ó��
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Walk:
                    break;
                case State.Attack:
                    OnAttack();
                    break;
                case State.Dead:
                    break;
            }
        }

        /// <summary>
        /// ���� ���·� ���� ��, �� �� ȣ��
        /// </summary>
        protected virtual void OnAttack()
        {
            attackController.canCheckCooltime = false;
            attackController.isCooltime = true;
        }

        // �ִϸ��̼��� Ư�� �����ӿ� ���� ȣ���ϰ��� �ϴ� �޼��带 ����Ͽ� �ش� �����ӿ�
        // �������� ��, ����� �޼��尡 ����ǵ��� �ϴ� ��� (���������� public�� ����)
        #region �ִϸ��̼� �̺�Ʈ
        /// <summary>
        /// ���� ��� �߿� Ÿ�� �Ǵ� �߻�ü�� �߻��ϴ� ������ ȣ��� �޼���
        /// </summary>
        public virtual void OnAttackHit()
        {
            attackController.OnAttack();
        }

        /// <summary>
        /// ���� ��� �߿� ����� �������� ȣ��� �̺�Ʈ 
        ///  -> ����� ������ �αٿ� ���� ��Ÿ���� �ٽ� üũ�� �� �ֵ��� �� ����
        /// </summary>
        public virtual void OnAttackEnd()
        {
            if (State == State.Dead)
                return;

            attackController.canCheckCooltime = true;
            SetState(State.Idle);
        }

        /// <summary>
        /// �״� ��� �߿� ����� �������� ȣ��� �̺�Ʈ
        /// </summary>
        public virtual void OnDeadEnd()
        { 
        
        }
        #endregion

    }
}