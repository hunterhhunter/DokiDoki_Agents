using AI_Project.SD;
using System;

namespace AI_Project.DB
{
    //1. NPC 스크립트 생성(부모는 모노)
    //2. NPC 초기화 시, BoNPC 를 받아
    //   NPC의 위치 및 회전값 설정
    //3. 플레이어와의 상호작용을 위해
    //   Physics.OverlapBox를 통해 NPC의 위치를 기점으로
    //   NPC에게 설정된 콜라이더 영역 * 1.25배만큼 충돌체크를 함
    //   이 영역 내에 플레이어가 들어와있고, 이 때 e키를 눌렀다면
    //   isInteraction을 true로 체크
    //4. 이미 상호작용 중인 상태에서도 영역을 벗어났다면, isInteraction을
    //   false로   
    //5. NPC 생성은 씬/스테이지 전환 완료 후,
    //   몬스터/플레이어와 동일하게 활성화된 NPC만을 들고있는 리스트가
    //   IngameManager에 존재함
    //6. IngameManager에서는 활성화된 NPC들의 업데이트를 수행함
    //   (이 때 NPC 업데이트를 통해 위의 3,4의 기능들이 계속해서 호출됨)

    [Serializable]
    public class BoNPC 
    {
        /// <summary>
        /// 현재 NPC가 플레이어와 상호작용 중인지를 나타내는 값
        /// </summary>
        public bool isInteraction;

        public SDNPC sdNPC;

        public BoNPC(SDNPC sdNPC)
        {
            this.sdNPC = sdNPC;
        }
    }
}
