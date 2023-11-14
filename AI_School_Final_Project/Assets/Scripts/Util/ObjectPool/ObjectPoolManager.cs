using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.Util
{
    /// <summary>
    /// 여러 종류의 오브젝트 풀을 관리할 클래스
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// 모든 오브젝트 풀을 보관할 딕셔너리
        ///  ex) ObjectPool<Monster>, ObjectPool<Effect> 등의 서로 다른 타입의 풀을 보관하기 위해
        ///      object 타입으로 박싱하여 관리한다.
        ///      대신 성능면에서 좋은 것은 아님.. 명시적으로 서로 다른 타입 풀을 필드로 선언하는 것이 베스트
        /// </summary>
        private Dictionary<string, object> poolDic = new Dictionary<string, object>();

        /// <summary>
        /// 풀 딕셔너리에 새로운 풀을 생성하여 등록하는 기능
        /// </summary>
        /// <typeparam name="T">생성할 풀의 타입</typeparam>
        /// <param name="obj">풀의 초기 생성을 위해 필요한 원형객체(프리팹)</param>
        /// <param name="poolCount">초기에 생성할 인스턴스 수</param>
        public void RegistPool<T>(T obj, int poolCount = 1) where T : MonoBehaviour, IPoolableObject
        {
            ObjectPool<T> pool = null;

            // T타입 이름을 딕셔너리의 키 값으로 활용
            var key = typeof(T).Name;

            // 딕셔너리에 이미 키가 존재한다면?
            if (poolDic.ContainsKey(key))
            {
                // 기존에 존재하던 풀을 사용
                //  -> 이미 풀이 존재한다는 뜻이고, 해당 기능은 풀을 새로 등록하는 기능인데
                //     등록을 중단하지 않고, 기존 풀을 그대로 받아오는지?
                //  -> 현재 제가 만들고 있는 오브젝트 풀은 동일한 T 타입이라도
                //     실제 인스턴스 시에 모델이나 세부 로직이 다른 인스턴스도 담을 수 있게 하려고함
                //     ex) 몬스터 풀이 존재 시, 처음 등록 시에 a라는 몬스터를 등록했다고 가정
                //         이후 b라는 몬스터를 풀링하여 사용하고자 할 때, 굳이 새로운 풀을 만들 필요 없이
                //         기존에 있던 동일한 몬스터풀을 이용할 수 있게하려고 함
                pool = poolDic[key] as ObjectPool<T>;
            }
            // 존재하지 않는다면 
            else
            {
                // 해당 키 값의 풀이 등록되어있지 않으므로, 새로 생성하여 딕셔너리에 추가
                pool = new ObjectPool<T>();
                poolDic.Add(key, pool);
            }

            // 풀에 홀더가 존재하는지 체크하여, 홀더가 없다면 생성
            if (pool.holder == null)
            {
                // 홀더의 이름을 T 타입 Holder로 지정하여 생성
                pool.holder = new GameObject($"{key}Holder").transform;
                // 홀더의 부모 객체를 오브젝트 풀 매니저로 지정
                //  -> 결과, 오브젝트 풀 매니저는 싱글톤 객체이고 현재 싱글톤 객체는
                //     DonDestroyOnLoad로 설정되어 있어 씬이 변경되도 파괴되지 않음
                //     따라서, 오브젝트 풀에 있는 객체들도 파괴되지 않음
                //     이 말은 상황에 따라 적절히 풀이 필요 없어질 때 본인이 알아서 해제해야함
                pool.holder.parent = transform;
                pool.holder.position = Vector3.zero;
            }

            // 파라미터로 받은 풀에 초기 생성할 인스턴스 수 만큼 반복
            for (int i = 0; i < poolCount; ++i)
            {
                // 인스턴스를 생성하여 풀에 등록
                var poolableObj = Instantiate(obj);
                poolableObj.name = obj.name;
                // 생성한 인스턴스의 부모를 홀더로 설정
                poolableObj.transform.SetParent(pool.holder);
                poolableObj.gameObject.SetActive(false);

                pool.Regist(poolableObj);
            }
        }

        /// <summary>
        /// poolDic에 등록된 특정 풀을 찾아서 반환
        /// </summary>
        /// <typeparam name="T">찾고자하는 풀의 타입</typeparam>
        /// <returns>T타입의 풀</returns>
        public ObjectPool<T> GetPool<T>() where T : MonoBehaviour, IPoolableObject
        {
            var key = typeof(T).Name;

            // 딕셔너리에 해당 타입의 키 값이 존재하는지 검사
            if (!poolDic.ContainsKey(key))
            {
                return null;
            }

            return poolDic[key] as ObjectPool<T>;
        }

        /// <summary>
        /// 특정 풀을 클리어하는 기능
        /// </summary>
        /// <typeparam name="T">클리어하고자하는 풀의 타입</typeparam>
        public void ClearPool<T>() where T : MonoBehaviour, IPoolableObject
        {
            // 이 기능은 특정 씬에서 들고있을 필요가 없는 풀이 발생했을 때
            // 해당 풀을 비우기 위한 용도로 사용

            var pool = GetPool<T>()?.Pool;

            // 풀이 없다면 리턴
            if (pool == null)
                return;

            // 있다면 풀 안에 있는 풀러블 객체를 전부 파괴한다
            for (int i = 0; i < pool.Count; ++i)
            {
                if (pool[i] != null)
                    Destroy(pool[i].gameObject);
            }

            pool.Clear();
        }

    }
}
