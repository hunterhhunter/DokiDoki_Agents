using AI_Project.Util;
using System.Linq;
using UnityEngine;

namespace AI_Project.Object
{
    public class Projectile : MonoBehaviour, IPoolableObject
    {
        public bool isEnd;
        private int targetLayer;
        private float lauchTime;
        private float duration = 4f;
        private float speed = 2f;
        private float radius;
        private AttackController ac;

        public bool CanRecycle { get; set; } = true;

        public void Initialize(AttackController ac)
        {
            this.ac = ac;

            isEnd = false;
            // 발사체의 방향을 공격자가 바라보고 있던 방향으로
            transform.forward = ac.attacker.transform.forward;
            // 발사체의 위치를 공격자의 위치로
            transform.position = ac.attacker.transform.position + Vector3.up;
            // 발사 시간을 현재 초기화 시간으로
            lauchTime = Time.time;
            // 공격자의 액터 타입에 따라 충돌을 체크할 타겟 레이어 설정
            targetLayer = ac.attacker.boActor.type == Define.Actor.Type.Character ?
                1 << LayerMask.NameToLayer("Monster") : 1 << LayerMask.NameToLayer("Character");
            // 발사체의 충돌 판정 체크 영역을 공격자의 공격범위로 설정
            radius = ac.attacker.boActor.atkRange;
        }

        public void Execute()
        {
            // 타겟과 충돌했거나 지속시간이 끝났다면 리턴
            if (isEnd)
                return;

            // 타겟 레이어와 겹침 체크
            var colls = Physics.OverlapSphere(transform.position, radius, targetLayer);

            // 타겟이 존재한다면
            if (colls.Length > 0)
            {
                isEnd = true;

                // 공격자의 타겟 목록을 새로 설정
                ac.SetTargets(colls.Select(_ => _.GetComponent<Actor>()));
                // 데미지 연산
                for (int i = 0; i < ac.targets.Count; ++i)
                {
                    ac.CalculateDamage(ac.attacker.boActor.atk, ac.targets[i]);   
                }
                return;
            }

            // 이동
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

            // 지속시간 체크
            if (Time.time - lauchTime >= duration)
            {
                isEnd = true;
            }
        }
    }
}