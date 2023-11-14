using AI_Project.SD;
using System;

namespace AI_Project.DB
{
    // 인게임 로직에 사용할 아이템 개별 데이터 (베이스)
    [Serializable]
    public class BoItem
    {
        public int slotIndex; // 아이템이 인벤토리 내 위치하고 있는 슬롯의 인덱스
        public int amount; // 아이템의 개수
        public SDItem sdItem; // 아이템의 기획 데이터

        // 파라미터가 SDItem인 이유? 왜 dto 아님?
        // -> 저희 프로젝트에서는 아이템 데이터 저장만을 통신을 통해서 진행하고
        //    아이템 생성은 클라이언트에서 처리할 예정이기 때문
        //    클라가 직접 아이템을 생성하므로 기획데이터를 이용해서 bo를 생성하는 생성자를 작성함
        // 추가로, 나중에 아이템 데이터를 db에서 불러오는 경우를 처리 시에는
        // 원한다면 dto를 이용한 생성자를 추가로 작성할 수 있겠지만, 번거로우므로 아래의 생성자로
        // 한꺼번에 처리하도록 작성할게요..
        public BoItem(SDItem sdItem)
        {
            slotIndex = -1;
            amount = 1;
            this.sdItem = sdItem;
        }

        public BoItem Copy()
        {
            // MemberwiseClone을 통해 값 타입의 필드를 전부 복사함.
            BoItem clone = (BoItem)this.MemberwiseClone();
            // 그 후 참조타입만 별도로 개별적으로 처리하면 깊은복사가 됨
            // 하지만, 현재 boItem의 참조타입을 별도로 복사할 필요 없음
            // 이유?
            // 현재 boItem의 참조타입은 SDItem만 존재
            // SDItem은 참조타입이지만, 현재 모든 기획데이터는 SD 모듈쪽에서 
            // 참조하여 사용하므로, 굳이 복사하여 기획데이터를 추가적으로 메모리에 들고
            // 있을 필요가 없음
            // 한마디로 같은 공간을 가리켜도 상관없음

            return clone;
        }
    }

    // 인게임 로직에 사용될 장비 개별 데이터 (BoItem에서 파생됨)
    [Serializable]
    public class BoEquipment : BoItem
    {
        public bool isEquip; // 장비를 착용중인지
        public int reinforceValue; // 강화 수치

        public BoEquipment(SDItem sdItem) : base(sdItem)
        {
            // 베이스에서 정의된 필드는 위임 생성을 통해 베이스에서
            // 알아서 처리됨
            isEquip = false;
            reinforceValue = 0;
        }
    }
}
