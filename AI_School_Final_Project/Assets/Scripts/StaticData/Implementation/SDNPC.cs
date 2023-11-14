using System;

namespace AI_Project.SD
{
    [Serializable]
    public class SDNPC : StaticData
    {
        public string name;
        /// <summary>
        /// NPC 종류 (ex: 잡화상인, 강화, 장비판매 등)
        /// </summary>
        public Define.NPC.Type type;
        /// <summary>
        /// 해당 NPC가 유저에게 발행할 수 있는 퀘스트들의 인덱스 값
        /// </summary>
        public int[] questRef;
        /// <summary>
        /// 특정 퀘스트를 완료해야만, 출현하는 NPC가 존재한다면
        /// 이 때, 해당 NPC를 출현시키기 위해 플레이어가 완료해야만하는 퀘스트들의 인덱스 목록
        /// </summary>
        public int[] needQuestRef;
        /// <summary>
        /// 해당 NPC가 존재하는 스테이지 인덱스
        /// </summary>
        public int stageRef;
        /// <summary>
        /// 해당 NPC가 스테이지 내에 위치하고 있는 좌표 및 회전 값
        /// </summary>
        public float[] stagePos;
        /// <summary>
        /// 해당 NPC에게 대화를 걸었을 때, 출력되는 대사들의 스트링 테이블 인덱스
        /// </summary>
        public int[] speechRef;
        /// <summary>
        /// NPC 프리팹 경로
        /// </summary>
        public string resourcePath;
    }
}
