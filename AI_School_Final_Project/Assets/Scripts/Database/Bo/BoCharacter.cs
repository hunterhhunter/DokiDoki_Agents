using AI_Project.SD;
using System;
using System.Linq;

namespace AI_Project.DB
{
    [Serializable]
    public class BoCharacter : BoActor
    {
        public float currentExp;
        public float maxExp;

        /// <summary>
        /// 캐릭터 기획 데이터
        /// </summary>
        public SDCharacter sdCharacter;
        /// <summary>
        /// 캐릭터 레벨에 영향을 받는 스텟 기획 데이터
        /// </summary>
        public SDGrowthStat sdGrowthStat;

        public BoCharacter(DtoCharacter dtoCharacter)
        {
            level = dtoCharacter.level;
            // 서버에서 캐릭터 인덱스를 전달해주고,
            // 게임매니저 쪽에서 모든 기획데이터를 가지고 있으므로
            // 캐릭터 인덱스를 통해 캐릭터 테이블에서 해당 인덱스 값을 갖는
            // 캐릭터 데이터를 불러온다.
            sdCharacter = GameManager.SD.sdCharacters
                .Where(_ => _.index == dtoCharacter.index).SingleOrDefault();

            // 성장스텟 테이블에서 내 캐릭터가 참조하는 성장스텟 인덱스 값과 동일한 인덱스를 가진
            // 데이터가 있다면 가져옴
            sdGrowthStat = GameManager.SD.sdGrowthStats
                .Where(_ => _.index == sdCharacter.growthStatRef).SingleOrDefault();

            currentExp = dtoCharacter.currentExp;
        }
    }
}
