using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.Util
{
    /// <summary>
    /// 오브젝트 풀링을 수행할 클래스
    /// </summary>
    /// <typeparam name="T">모노를 상속받고, IPoolableObject를 구현하는 타입만</typeparam>
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// T타입 객체들을 갖는 풀
        /// </summary>
        public List<T> Pool { get; private set; } = new List<T>();

        /// <summary>
        /// 해당 풀의 객체들의 인스턴스를 하이라키 상에서 들고 있을 부모 객체의 트랜스폼
        /// (하이라키 상에서 관리하기 좋게 정리용으로 사용할 홀더)
        /// </summary>
        public Transform holder;

        /// <summary>
        /// 풀에서 재사용한 가능한 객체가 존재하는지를 나타내는 프로퍼티
        ///  -> Pool.Find(_ => _.CanRecycle)
        ///     풀 안에 있는 객체 중 CanReycycle 값이 true인 객체가 있는지 찾는 작업 
        ///     풀 안에 있는 T타입 객체는 IPoolableObject를 상속받으므로 CanRecycle 프로퍼티를 갖음
        /// </summary>
        public bool CanRecycle => Pool.Find(_ => _.CanRecycle) != null;

        /// <summary>
        /// 풀링할 새로운 T타입 인스턴스를 등록
        /// </summary>
        /// <param name="obj">풀에 넣고자하는 T타입 인스턴스</param>
        public void Regist(T obj)
        {
            Pool.Add(obj);
        }

        /// <summary>
        /// 풀에서 꺼내서 사용하던 객체를 풀에 다시 반환하는 기능
        /// (정확히는 객체는 항상 풀에 존재하고, 풀에서 객체를 꺼낸다고 표현하고 있지만
        ///  실제로 이루어지는 작업은, 재사용 가능한 객체를 찾아서 활성/비활성화 && 객체의 부모 변경)
        ///  풀에 있을 경우 -> 객체의 부모는 오브젝트 풀의 홀더 필드
        ///  풀에서 꺼냈을 경우 -> 상황에 따라 내가 지정한 다른 부모를 가짐
        /// </summary>
        /// <param name="obj">반환하고자하는 T타입 인스턴스</param>
        public void Return(T obj)
        {
            // 반환하였으므로 객체의 부모를 다시 홀더로 지정
            obj.transform.SetParent(holder);
            obj.gameObject.SetActive(false);
            obj.CanRecycle = true;
        }

        /// <summary>
        /// 풀 내의 재사용 가능하고 특정 조건을 만족하는 객체를 반환
        /// </summary>
        /// <param name="condition">내가 찾고자 하는 특정 조건</param>
        /// <returns>재사용 가능하고 특정 조건을 만족하는 T타입 인스턴스</returns>
        public T GetObj(Func<T, bool> condition = null)
        {
            // 풀 내에서 재사용 가능한 객체가 없다면
            if (!CanRecycle)
            {
                // 조건이 있다면 조건과 동일한 객체를 하나 가져오도록
                // 조건이 없다면 풀에서 아무 객체나 하나 가져오도록
                var protoObj = condition != null ? Pool.Find(_ => condition(_)) : Pool[0];

                // 재사용이 현재 불가능하지만, 조건을 만족하는 객체가 있다면
                if (protoObj != null)
                {
                    // 해당 원형 객체를 통해 새로운 객체를 생성
                    var clone = GameObject.Instantiate(protoObj.gameObject, holder);
                    clone.name = protoObj.name;
                    clone.SetActive(false);
                    // 새로 생성한 객체를 풀에 등록
                    Regist(clone.GetComponent<T>());
                }
                // 없다면
                else
                {
                    Debug.Log($"{typeof(T).Name } GetObj from pool failed !!!");
                    return null;
                }
            }

            // 풀에 재사용 가능한 객체가 존재하거나, 존재하지 않지만 조건을 만족하는 객체를 찾아 복사했다면
            // 이 곳으로 들어옴

            T recycleObj = condition != null ? 
                // 조건이 있다면 조건에 만족하고, 재사용 가능한 객체가 있다면 하나 가져옴
                Pool.Find(_ => condition(_) && _.CanRecycle)
                // 조건이 없다면 재사용 가능한 객체가 있다면 하나 가져옴
                : Pool.Find(_ => _.CanRecycle);

            if (recycleObj == null)
            {
                Debug.Log($"{typeof(T).Name } GetObj from pool failed !!!");
                return null;
            }

            // 이 곳으로 왔다면, 재사용할 객체를 찾았다는 뜻
            // 따라서, 해당 객체를 이제 사용할 것이므로 재사용 플래그를 더 이상 재사용할 수 없게 변경
            recycleObj.CanRecycle = false;

            return recycleObj;
        }
    }
}
