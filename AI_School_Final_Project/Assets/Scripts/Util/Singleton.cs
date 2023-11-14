using UnityEngine;

namespace AI_Project.Util
{
    /// <summary>
    /// 디자인 패턴 중 하나, 싱글턴 패턴
    ///  -> 객체의 인스턴스를 단 하나로 유지하는 방법
    ///  
    /// 씬이 변경되어도 데이터가 유지되어야 하고, 외부에서 호출이 잦음
    /// 여러 객체를 사용하는 것이 아닌 단일 객체를 사용
    /// 위의 경우에 싱글턴 패턴을 적용
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        /// <summary>
        /// 싱글턴에서 파생된 클래스의 인스턴스
        /// </summary>
        private static T instance = null;

        /// <summary>
        /// 외부에서 싱글턴 인스턴스에 접근하기 위한 읽기 전용 프로퍼티
        ///  -> get에서 instance가 존재하는지 확인
        /// </summary>
        public static T Instance
        {
            get
            {
                // 인스턴스가 존재하는지 체크
                if (instance == null)
                {
                    // 인스턴스를 씬에서 찾음
                    instance = FindObjectOfType<T>();
                    // 인스턴스를 찾았는지 체크
                    if (instance == null)
                    {
                        // 없다면 게임 오브젝트를 생성
                        GameObject obj = new GameObject(typeof(T).Name);
                        // 생성한 빈 객체에 T타입 컴포넌트를 붙인다
                        instance = obj.AddComponent<T>();
                    }
                }

                // 결과적으로, 처음에 존재한다면 바로 반환
                // 존재하지 않는다면 찾거나, 생성하여 반환
                //  -> 이후에는 인스턴스가 null이 아니므로 결과적으로
                //     계속해서 동일한 단 하나의 인스턴스만을 반환하게 된다.
                return instance;
            }
        }

        // Awake : 객체가 초기화되는 시점에 호출되는 콜백, Start 콜백보다 먼저 호출됌
        //  -> Awake가 호출되었다는 것은 씬의 하이라키상에 해당 싱글턴 객체가 이미 존재한다는 뜻
        protected virtual void Awake()
        {
            // 인스턴스가 없다면
            if (instance == null)
            {
                // 인스턴스를 미리 넣어주는 작업
                //  -> Instance 프로퍼티를 통해 접근 시 객체를 찾거나 생성하는 과정을 생략 
                instance = this as T;
                // 씬이 변경되어도 게임 오브젝트가 파괴되지 않도록
                // gameObject : MonoBehaviour를 상속받았다면, 게임 오브젝트(씬에서 하이라키상에서 객체)로 사용 가능
                //              -> 자기 자신의 게임오브젝트를 가리키는 프로퍼티
                DontDestroyOnLoad(gameObject);
            }
            // 인스턴스가 있다면
            else
            {
                // 이 시점에 인스턴스가 존재하는 것은 잘못된 사용으로 인한 복수의 인스턴스 생성
                // Destory : 파라미터로 넘긴 게임오브젝트를 파괴하는 기능
                Destroy(gameObject);
            }
        }
    }
}