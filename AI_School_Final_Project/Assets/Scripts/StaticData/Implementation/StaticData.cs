using System;

namespace AI_Project.SD
{
    /// <summary>
    /// 모든 기획데이터의 베이스
    ///  -> 모든 기획데이터가 공통으로 갖는 필드(컬럼)를 갖음
    /// </summary>
    [Serializable]
    public class StaticData
    {
        /// <summary>
        /// 기획 데이터의 인덱스
        /// (인덱스는 해당 테이블 내에서 유니크함)
        /// 모든 테이블이 인덱스를 사용하는 이유?
        ///   -> 데이터셋(엑셀로 치자면 각 행) 마다 데이터를 구분하기 위한 용도
        ///      추후, 특정 테이블에서 인덱스 값을 가지고 특정 데이터를 가져올 수 있음
        /// </summary>
        public int index;
    }
}
