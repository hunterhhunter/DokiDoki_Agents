using AI_Project.SD;
using System;
using System.Linq;
using UnityEngine;

namespace AI_Project.DB
{
    [Serializable]
    public class BoStage
    {
        /// <summary>
        /// 다른 스테이지로 워프 시, 이전 스테이지에 대한 인덱스를 받을 필드
        ///  -> 받는 이유? 어디서 왔는지 알아야 해당 워프와 연결된 새로운 스테이지의 워프로 배치해줄 수 있으므로
        /// </summary>
        public int prevStageIndex;
        /// <summary>
        /// 플레이어가 마지막으로 위치한 좌표
        /// </summary>
        public Vector3 lastPos;
        /// <summary>
        /// 플레이어가 마지막으로 위치한 스테이지의 기획 데이터
        /// </summary>
        public SDStage sdStage;

        public BoStage(DtoStage dtoStage)
        {
            sdStage = GameManager.SD.sdStages
                .Where(_ => _.index == dtoStage.index).SingleOrDefault();

            lastPos = new Vector3(dtoStage.posX, dtoStage.posY, dtoStage.posZ);
        }
    }
}
