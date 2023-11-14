using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Project.DB
{
    /// <summary>
    /// BoUser는 dto 데이터를 받아 가공되는 데이터가 아닌,
    /// 여러 유저 데이터를 편하게 관리하기 위해 유저의 bo 데이터들을
    /// 전부 필드로 갖는 클래스
    ///   -> bo가 무조건 dto를 가공하여 쓰는 데이터는 아님
    ///      비즈니스 오브젝트 -> 비즈니스 로직 (인게임 로직)에 사용되는 데이터셋이
    ///      존재한다면 bo 쪽에서 정의하여 사용하면 됨 
    /// 
    /// </summary>
    [Serializable]
    public class BoUser
    {
        public BoAccount boAccount;
        public BoCharacter boCharacter;
        public BoStage boStage;
        public List<BoItem> boItems;
        public BoQuest boQuest;
    }
}
