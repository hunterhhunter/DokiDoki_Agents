using AI_Project.DB;
using AI_Project.Network;
using AI_Project.Util;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.Object
{
    using Type = Define.Actor.Type;
    using State = Define.Actor.State;
    using AttackType = Define.Actor.AttackType;

    /// <summary>
    /// 액터의 공격 기능을 수행할 컨트롤러
    /// </summary>
    public class AttackController : MonoBehaviour
    {
        /// <summary>
        /// 공격 대상이 있는지?
        /// </summary>
        public bool hasTarget;

        /// <summary>
        /// 공격 쿨타임을 체크할 수 있는지? 
        ///   -> (공격 모션의 특정 지점이 끝나기 전에는 쿨타임체크를 막음)
        /// </summary>
        public bool canCheckCooltime;

        /// <summary>
        /// 현재 공격 쿨타임인지?
        /// </summary>
        public bool isCooltime;

        /// <summary>
        /// 공격 가능 상태인지?
        /// </summary>
        public bool canAtk;

        /// <summary>
        /// 현재 공격 쿨타임을 체크하는 값
        /// </summary>
        private float currentCooltime;

        /// <summary>
        /// 공격자 (해당 어택 컨트롤러 인스턴스를 갖는 액터)
        /// </summary>
        public Actor attacker;

        /// <summary>
        /// 피격 대상 액터들을 갖는 리스트
        /// </summary>
        public List<Actor> targets = new List<Actor>();

        /// <summary>
        /// 초기화 기능
        /// </summary>
        /// <param name="attacker">공격자의 인스턴스 참조</param>
        public void Initialize(Actor attacker)
        {
            this.attacker = attacker;


            canAtk = true;

            InitCooltime();
        }

        /// <summary>
        /// 공격자가 현재 공격 가능한 상태인지를 반환해주는 기능
        /// </summary>
        /// <returns>가능이라면 true</returns>
        public bool CanAttack(bool force = false)
        {
            // 타겟이 없고 논타겟 실행이 아니라면 불가
            if (!hasTarget && !force)
                return false;

            // 공격 쿨타임이라면 불가
            if (isCooltime)
                return false;

            // 공격 불가능이라면 불가
            if (!canAtk)
                return false;

            return true;
        }

        // 현재 아래의 메서드들은 public virtual로 선언되는데
        // 추후, 공격 기능 확장에 따라 attackController를 상속받는
        // 공격 기능 확장 클래스를 작성할 수도 있기 때문에..

        /// <summary>
        /// 액터의 공격 모션이 타격점에 도달했을 때, 호출
        /// 근접 공격이라면 공격 범위 연산 후, 데미지 연산
        /// 발사체를 이용한 공격이라면 발사체를 생성
        /// </summary>
        public virtual void OnAttack()
        {
            // 현재 몬스터, 캐릭터의 게임 내 공격 메커니즘이 동일하다고 가정
            // 공격 타입에 따라 공격 기능을 다르게 실행
            switch (attacker.boActor.atkType)
            {
                case AttackType.Normal:
                    // 공격 범위 내 타겟 연산 (해당 기능 호출 후 타겟 목록이 채워짐)
                    CalculateAttackRange();

                    // 트루 데미지에 대한 연산 (현재 저희 게임은 트루뎀은 공격자의 공격력)
                    var damage = attacker.boActor.atk;

                    // 타겟 목록을 순회하며 데미지 적용
                    for (int i = 0; i < targets.Count; ++i)
                        CalculateDamage(damage, targets[i]);
                    break;
                case AttackType.Projectile:
                    OnFire();
                    break;
            }
        }

        /// <summary>
        /// 타겟 목록을 세팅하는 기능
        /// </summary>
        /// <param name="targets"></param>
        public void SetTargets(IEnumerable<Actor> actors)
        {
            // 기존의 타겟목록을 비움
            targets.Clear();

            // 파라미터로 받은 새로운 타겟들을 타겟목록에 추가
            targets.AddRange(actors);
        }

        /// <summary>
        /// 원거리 타입 발사체 생성
        /// </summary>
        public virtual void OnFire()
        {
            var projectile = ObjectPoolManager.Instance.GetPool<Projectile>().GetObj();
            projectile.Initialize(this);
            projectile.gameObject.SetActive(true);
            IngameManager.Instance.Projectiles.Add(projectile);
        }


        /// <summary>
        /// 공격 범위에 적이 있는지 연산
        /// </summary>
        public virtual void CalculateAttackRange()
        {
            // 공격자의 타입이 캐릭터라면, 타겟은 몬스터 아니라면 캐릭터
            var targetLayer = 
                LayerMask.NameToLayer(attacker.boActor.type == Type.Character ? "Monster" : "Character");

            // 공격자 위치를 기준으로 공격자가 바라보는 방향으로 atkRange 길이만큼 .5f 반지름을 갖는 구체 영역을
            // 반복해서 타겟 레이어에 충돌했는지 체크한다.
            var hits = Physics.SphereCastAll(attacker.transform.position, .5f, attacker.transform.forward,
                attacker.boActor.atkRange, 1 << targetLayer);

            // 위쪽에서 새로운 타겟에 대한 정보를 구했으니까, 이전에 사용하던 타겟 정보는 비운다.
            targets.Clear();

            // 새로운 타겟 정보를 타겟목록에 넣는다.
            for (int i = 0; i < hits.Length; ++i)
                targets.Add(hits[i].transform.GetComponent<Actor>());
        }

        /// <summary>
        /// 데미지를 공식에 따라 연산하여 타겟에 적용
        /// </summary>
        /// <param name="damage">공격자가 가한 트루 데미지(데미지 공식 미적용 상태)</param>
        /// <param name="target">피격 대상</param>
        public virtual void CalculateDamage(float damage, Actor target)
        {
            ServerManager.Server.CalculateDamage(0,
                new DtoAttack()
                {
                    targetType = target.boActor.type,
                    targetId = target.id,
                    targetDef = target.boActor.def,
                    targetHp = target.boActor.currentHp,
                    damage = damage,
                }, IngameManager.Instance.IngameHandler.attackHandler);

            //// 데미지 계산 (공격자의 데미지에서 피격자의 방어력을 뺄게요)
            //var calDamage = Mathf.Max(damage - target.boActor.def, 0);

            //// 계산된 데미지를 타겟의 현재 체력에 적용
            //target.boActor.currentHp = Mathf.Max(target.boActor.currentHp - calDamage, 0);

            // 타겟의 체력이 0 이하라면 죽은 상태로 변경
            if (target.boActor.currentHp <= 0)
                target.SetState(State.Dead);
        }

        /// <summary>
        /// 공격 쿨타임을 체크하는 기능
        ///  -> 이 메서드는 FixedUpdate에서 최종적으로 호출될 예정임
        /// </summary>
        public void CheckCooltime()
        {
            // 쿨타임을 체크할 수 없다면 리턴
            if (!canCheckCooltime)
                return;

            // 공격 쿨타임이 아니라면 리턴
            if (!isCooltime)
                return;

            // 이 아래에 왔다는 것은 쿨타임을 갱신할 수 있는 상태라는 뜻
            currentCooltime += Time.fixedDeltaTime;
            // 현재 공격 쿨타임의 공격자의 기본 공격 쿨타임 이상이라면
            if (currentCooltime >= attacker.boActor.atkInterval)
            {
                // 쿨타임이 지났다는 뜻, 따라서 쿨 초기화
                InitCooltime();
            }
        }

        /// <summary>
        /// 공격 쿨타임 초기화
        /// </summary>
        public void InitCooltime()
        {
            currentCooltime = 0;
            isCooltime = false;
        }
    }
}
