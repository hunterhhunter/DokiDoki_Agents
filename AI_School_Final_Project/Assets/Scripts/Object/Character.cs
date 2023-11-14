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

            // 제어의 편리를 위해 모든 데이터를 베이스의 형태로 관리하기 때문에
            // 데이터 접근의 편리함을 위해 파생 클래스 형태로 캐스팅하여 담아둔다
            boCharacter = boActor as BoCharacter;

            SetStats();
        }

        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// 캐릭터 스텟 설정
        ///  -> 기획 데이터와 유저데이터(레벨)를 기반으로 스텟이 설정된다.
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
            // 공통적으로 갖는 상태 처리
            base.SetState(state);

            // 캐릭터만 갖는 상태 처리
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
            // 속력과 방향을 통해 속도를 구함
            var velocity = boCharacter.moveSpeed * boCharacter.moveDir;
            // 속도를 바로 캐릭터의 로컬포지션에 더할 시, 월드를 기준으로한 방향 값으로 인해
            // 월드를 기준으로한 z축으로 움직임
            // 그리고 현재 캐릭터는 마우스를 통해 회전하므로, 이 때 캐릭터의 로컬 축은 변경된 상태임
            // 따라서 해당 벡터를 캐릭터의 로컬 축을 기준으로 한 값으로 변환시킨다.
            velocity = transform.TransformDirection(velocity);

            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate(boCharacter.rotDir * Define.Camera.RotSpeed);

            // 점프 상태에서 움직일 시, 이동 모션이 아닌 점프 모션을 그대로 실행시키기 위해
            if (State == Define.Actor.State.Jump || State == Define.Actor.State.Attack)
                return;

            // 속도 벡터의 길이가 0과 근사하다면 안 움직인다는 뜻
            //  따라서, 상태를 대기로 변경, 아니라면 걷기로 변경
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
        /// 캐릭터가 땅에 있는지 체크하는 기능
        /// </summary>
        private void CheckGround()
        {
            // 레이캐스팅을 이용해서 캐릭터가 땅에 닿았는지 확인
            boActor.isGround = Physics.Raycast(transform.position, Vector3.down, .1f, 
                1 << LayerMask.NameToLayer("Floor"));
            // << 비트 시프트 연산, 현재 Floor 레이어만 충돌 체크하도록

            // 현재 상태가 점프 상태가 아니라면 리턴
            if (State != Define.Actor.State.Jump)
                return;

            // 이 곳에 들어왔다는 것은 현재 점프 상태라는 뜻
            // 그리고 이 때, 만약 isGround가 true라면 캐릭터가 땅에 착지했다는 뜻
            // 따라서, 캐릭터의 상태를 대기 상태로 변경한다.
            if (boActor.isGround)
                SetState(Define.Actor.State.Idle);
        }

        /// <summary>
        /// 점프 연산 실행 (점프 키를 눌렀을 때 한 번 호출)
        /// </summary>
        private void OnJump()
        {
            rig.AddForce(Vector3.up * boCharacter.sdCharacter.jumpForce, ForceMode.Impulse);
        }

        /// <summary>
        /// 캐릭터의 현재 경험치를 추가하는 기능
        /// </summary>
        /// <param name="exp">추가되는 경험치량</param>
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