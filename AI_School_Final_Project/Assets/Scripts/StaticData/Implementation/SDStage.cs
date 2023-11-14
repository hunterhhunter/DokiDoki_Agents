using System;

namespace AI_Project.SD
{
    [Serializable]
    public class SDStage : StaticData
    {
        /// <summary>
        /// 이름
        /// </summary>
        public string name;
        /// <summary>
        ///  해당 스테이지에서 생성될 수 있는 몬스터들의 기획 데이터 상의 인덱스
        ///   -> 나중에 설명
        /// </summary>
        public int[] genMonsters;
        /// <summary>
        /// 위의 genMonsters에 존재하는 각각의 몬스터들이 어떤 지역에 스폰될 것인지를
        /// 나타내는 값
        /// </summary>
        public int[] spawnArea;
        /// <summary>
        /// 해당 마을에서 이동할 수 있는 스테이지들의 기획 데이터상의 인덱스
        /// </summary>
        public int[] warpStageRef;
        /// <summary>
        /// 프리팹 경로
        /// </summary>
        public string resourcePath;
    }
}
