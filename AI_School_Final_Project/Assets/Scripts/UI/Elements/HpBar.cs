using AI_Project.Object;
using AI_Project.Util;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class HpBar : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// hp �� ����ϰ��� �ϴ� Ÿ�� ���� ����
        /// </summary>
        public Actor target;
        /// <summary>
        /// hp ������ �̹��� ����
        /// </summary>
        public Image gauge;

        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// �ʱ�ȭ �� hp�� ����ϰ��� �ϴ� Ÿ���� ������ �Ѱܹ���
        /// </summary>
        /// <param name="target"></param>
        public void Initialize(Actor target)
        {
            this.target = target;

            // ������Ʈ Ǯ�� ����ϴ� �ֵ��� ������ �ش� Ǯ�� Ȧ���� ������ �ϰ� ����
            // �ٵ� Ǯ���� ������ ����� ���� Ȧ������ ���� ĵ������ ���̶�Ű ���� �θ� ������ ��
            // �� ��, �����ϸ��� �߻��ǹǷ�, �θ� �������� 1,1,1�� ũ��� �ٽ� �����ϸ���
            transform.localScale = Vector3.one;
        }

        public void HpBarUpdate()
        {
            if (target == null || target.Coll == null)
                return;

            // Ÿ���� �׾��ٸ� 
            if (target.State == Define.Actor.State.Dead)
            {
                // hpBar�� �ٽ� Ǯ�� ��ȯ
                ObjectPoolManager.Instance.GetPool<HpBar>().Return(this);
                return;
            }

            transform.position = target.transform.position + Vector3.up * target.Coll.bounds.size.y * 1.2f;
            gauge.fillAmount = target.boActor.currentHp / target.boActor.maxHp;
        }

    }
}