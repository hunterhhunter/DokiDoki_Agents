using AI_Project.DB;
using UnityEngine;

namespace AI_Project.Dummy
{
    // ScriptableObject
    // 기획 데이터 또는 정적 메서드(ex: 툴 같은 기능)만을 갖는 클래스를 작성할 때 주로 사용
    // 게임에서 유저 데이터를 저장하는 용도로 사용되는 DB는 데이터의 변경이 잦음
    // 반면에 기획데이터는 변경이 잦지 않음.
    //  ScriptableObject 가 갖는 데이터 필드를 Editor에서만 변경할 수 있게 만들었음
    //  실제 빌드 후에는 ScritableObject 형태로 파생된 클래스의 필드는 데이터를 수정할 수 없음

    // 더미서버를 사용하는 이유가 프로젝트 초반에 서버의 부재 상태에서 통신에 관한 처리를
    // 미리 일반화하여 작성하는 용도로 더미서버를 사용하는 것이므로
    // 빌드 단계에서는 더미서버를 사용할 일이 없다.
    // 따라서, ScriptableObject를 더미서버 DB로 사용해도 무방함.

    /// <summary>
    /// 더미서버에서의 DB의 역할을 할 클래스
    /// </summary>
    [CreateAssetMenu(menuName = "ProjectW/UserData")]
    public class UserDataSo : ScriptableObject
    {
        public DtoAccount dtoAccount;
        public DtoCharacter dtoCharacter;
        public DtoStage dtoStage;
        public DtoItem dtoItem;
        public DtoQuest dtoQuest;
    }
}
