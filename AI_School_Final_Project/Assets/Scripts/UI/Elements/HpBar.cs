using AI_Project.Object;
using AI_Project.Util;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class HpBar : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// hp 를 출력하고자 하는 타겟 액터 참조
        /// </summary>
        public Actor target;
        /// <summary>
        /// hp 게이지 이미지 참조
        /// </summary>
        public Image gauge;

        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// 초기화 시 hp를 출력하고자 하는 타겟의 참조를 넘겨받음
        /// </summary>
        /// <param name="target"></param>
        public void Initialize(Actor target)
        {
            this.target = target;

            // 오브젝트 풀로 사용하는 애들은 별도의 해당 풀의 홀더에 보관을 하고 있음
            // 근데 풀에서 꺼내서 사용할 때는 홀더에서 월드 캔버스로 하이라키 상의 부모가 변경이 됨
            // 이 때, 스케일링이 발생되므로, 부모를 기준으로 1,1,1의 크기로 다시 스케일링함
            transform.localScale = Vector3.one;
        }

        public void HpBarUpdate()
        {
            if (target == null || target.Coll == null)
                return;

            // 타겟이 죽었다면 
            if (target.State == Define.Actor.State.Dead)
            {
                // hpBar를 다시 풀에 반환
                ObjectPoolManager.Instance.GetPool<HpBar>().Return(this);
                return;
            }

            transform.position = target.transform.position + Vector3.up * target.Coll.bounds.size.y * 1.2f;
            gauge.fillAmount = target.boActor.currentHp / target.boActor.maxHp;
        }

    }
}