using AI_Project.DB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.Object
{
    using State = Define.Actor.State;

    /// <summary>
    /// 인게임 내에서 다이나믹하게 행동하는 객체들의 추상화된 베이스 클래스
    /// 캐릭터, 몬스터 등
    /// Actor의 파생클래스에서 공통되는 기능은 최대한 Actor에 정의
    /// 파생클래스에 따라 다른 기능은 해당 파생클래스에서 정의
    /// </summary>
    public abstract class Actor : MonoBehaviour
    {
        public int id;
        /// <summary>
        /// 애니메이터 상태 제어에 사용되는 state 변수의 hash 값
        /// </summary>
        protected int animStateHash;
        /// <summary>
        /// 액터의 현재 상태
        /// </summary>
        public State State;

        /// <summary>
        /// 액터의 bo 데이터 참조
        /// </summary>
        public BoActor boActor;

        // 이전에는 객체를 나타내는 클래스에 객체의 기능을 포함하여 작성했음
        // 하지만, 객체와 객체의 기능을 분리하여 작성할 수 있다면 최대한 분리하여 작성하는 것이
        // 프로그램의 확장에 도움이 됨, 대신 생산성은 조금 떨어질 수 있음
        //  -> 구조적인 부분만 봤을 때는 가장 옳은 판단
        /// <summary>
        /// 액터의 공격 기능을 수행할 컨트롤러 객체 참조
        /// </summary>
        public AttackController attackController;

        /// <summary>
        /// 액터가 공통적으로 사용하는 컴포넌트들의 참조
        /// </summary>
        public Collider Coll { get; private set; }
        protected Rigidbody rig;
        protected Animator anim;

        /// <summary>
        /// 초기화 시, 외부에서 boActor 데이터를 주입받는다.
        /// </summary>
        /// <param name="boActor"></param>
        public virtual void Initialize(BoActor boActor)
        {
            State = State.Idle;
            this.boActor = boActor;

            // attackController 참조가 존재하는지 확인 후 없다면 생성
            attackController ??= gameObject.AddComponent<AttackController>();
            // 어택 컨트롤러에 액터의 참조를 전달(공격자 등록)하여 초기화 
            attackController.Initialize(this);
        }

        protected virtual void Start()
        {
            // 액터들이 사용하는 컴포넌트들의 참조를 받는다
            Coll = GetComponent<Collider>();
            rig = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            animStateHash = Animator.StringToHash("state");
        }

        /// <summary>
        /// 액터 스텟 설정 메서드
        /// </summary>
        public virtual void SetStats() { }

        /// <summary>
        /// 액터 업데이트
        /// </summary>
        public virtual void Execute()
        {
            MoveUpdate();
        }

        /// <summary>
        /// 이동 업데이트
        /// </summary>
        public virtual void MoveUpdate() { }

        /// <summary>
        /// 현재 상태 변경 기능
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetState(State state)
        {


            State = state;
            anim.SetInteger(animStateHash, (int)state);

            // 액터의 파생 객체들의 공통적인 상태만을 베이스에서 처리
            // 그 후 파생 객체에 따라, 개별적으로 갖는 상태는 해당 파생클래스에서 처리
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
        /// 공격 상태로 변경 시, 한 번 호출
        /// </summary>
        protected virtual void OnAttack()
        {
            attackController.canCheckCooltime = false;
            attackController.isCooltime = true;
        }

        // 애니메이션의 특정 프레임에 내가 호출하고자 하는 메서드를 등록하여 해당 프레임에
        // 도달했을 때, 등록한 메서드가 실행되도록 하는 기능 (접근제어자 public만 가능)
        #region 애니메이션 이벤트
        /// <summary>
        /// 공격 모션 중에 타점 또는 발사체를 발사하는 시점에 호출될 메서드
        /// </summary>
        public virtual void OnAttackHit()
        {
            attackController.OnAttack();
        }

        /// <summary>
        /// 공격 모션 중에 모션의 마지막에 호출될 이벤트 
        ///  -> 모션의 마지막 부근에 공격 쿨타임을 다시 체크할 수 있도록 할 시점
        /// </summary>
        public virtual void OnAttackEnd()
        {
            if (State == State.Dead)
                return;

            attackController.canCheckCooltime = true;
            SetState(State.Idle);
        }

        /// <summary>
        /// 죽는 모션 중에 모션의 마지막에 호출될 이벤트
        /// </summary>
        public virtual void OnDeadEnd()
        { 
        
        }
        #endregion

    }
}