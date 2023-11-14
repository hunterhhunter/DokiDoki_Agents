
namespace AI_Project.Util
{
    /// <summary>
    /// 오브젝트 풀링을 사용하는 클래스에서 구현해야하는 인터페이스
    /// 오브젝트 풀링을 사용할 객체는 해당 인터페이스를 상속받아야만
    /// 오브젝트 풀링을 사용할 수 있음
    /// </summary>
    public interface IPoolableObject
    {
        /// <summary>
        /// 오브젝트가 재사용될 수 있는 상태인지를 나타내는 프로퍼티
        /// </summary>
        bool CanRecycle { get; set; }
    }
}
