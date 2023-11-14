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
            // �߻�ü�� ������ �����ڰ� �ٶ󺸰� �ִ� ��������
            transform.forward = ac.attacker.transform.forward;
            // �߻�ü�� ��ġ�� �������� ��ġ��
            transform.position = ac.attacker.transform.position + Vector3.up;
            // �߻� �ð��� ���� �ʱ�ȭ �ð�����
            lauchTime = Time.time;
            // �������� ���� Ÿ�Կ� ���� �浹�� üũ�� Ÿ�� ���̾� ����
            targetLayer = ac.attacker.boActor.type == Define.Actor.Type.Character ?
                1 << LayerMask.NameToLayer("Monster") : 1 << LayerMask.NameToLayer("Character");
            // �߻�ü�� �浹 ���� üũ ������ �������� ���ݹ����� ����
            radius = ac.attacker.boActor.atkRange;
        }

        public void Execute()
        {
            // Ÿ�ٰ� �浹�߰ų� ���ӽð��� �����ٸ� ����
            if (isEnd)
                return;

            // Ÿ�� ���̾�� ��ħ üũ
            var colls = Physics.OverlapSphere(transform.position, radius, targetLayer);

            // Ÿ���� �����Ѵٸ�
            if (colls.Length > 0)
            {
                isEnd = true;

                // �������� Ÿ�� ����� ���� ����
                ac.SetTargets(colls.Select(_ => _.GetComponent<Actor>()));
                // ������ ����
                for (int i = 0; i < ac.targets.Count; ++i)
                {
                    ac.CalculateDamage(ac.attacker.boActor.atk, ac.targets[i]);   
                }
                return;
            }

            // �̵�
            transform.position += transform.forward * speed * Time.fixedDeltaTime;

            // ���ӽð� üũ
            if (Time.time - lauchTime >= duration)
            {
                isEnd = true;
            }
        }
    }
}