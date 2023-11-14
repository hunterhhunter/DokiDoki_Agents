using AI_Project.DB;
using AI_Project.Dummy;
using AI_Project.Resource;
using AI_Project.SD;
using AI_Project.UI;
using AI_Project.Util;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.Object
{
    public class Item : MonoBehaviour, IPoolableObject
    {
        // 아이템 기획 데이터
        private SDItem sdItem;
        // 현재 아이템을 출력할 이미지 컴포넌트 참조
        private Image icon; 

        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// 몬스터가 죽은 후, 아이템이 드랍되면 해당 아이템의 인덱스를
        /// 아이템 객체 초기화 시 전달함
        /// </summary>
        /// <param name="itemIndex"></param>
        public void Initialize(int itemIndex)
        {
            // 전달받은 아이템 인덱스를 기반으로 기획 데이터를 가져옴
            sdItem = GameManager.SD.sdItems.Where(_ => _.index == itemIndex).SingleOrDefault();
            // 아이템 스프라이트 불러옴
            var itemSprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlas, sdItem.resourcePath);
            // 불러온 스프라이트를 이미지 컴포넌트에 적용
            icon ??= GetComponent<Image>();
            icon.sprite = itemSprite;
            // 이미지 컴포넌트의 스프라이트를 네이티브 사이즈로 적용
            icon.SetNativeSize();
        }

        private void OnTriggerEnter(Collider other)
        {
            // 아이템과 겹친 콜라이더가 갖는 태그가 플레이어가 아니라면 리턴
            if (!other.gameObject.CompareTag("Player"))
                return;

            // 장비는 동일한 장비를 먹더라도, 서로 다른 슬롯을 사용할 예정
            // 드롭한 아이템이 장비인지 확인
            var isEquip = sdItem.itemType == Define.Item.Type.Equipment;

            // 유저 아이템 정보 참조 받아옴
            var userItems = GameManager.User.boItems;
            // UI 인벤토리 참조 받아옴
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // 장비가 아닌 아이템이라면
            if (!isEquip)
            {
                // 유저가 해당 아이템을 이미 가지고 있는지 확인
                var sameItem = userItems.Where(_ => _.sdItem.index == sdItem.index).SingleOrDefault();

                // 이미 가지고 있는 아이템이라면 개수를 올려주고
                if (sameItem != null)
                {
                    ++sameItem.amount;
                    uiInventory.IncreaseItem(sameItem);
                }
                // 가지고 있지 않다면 인벤토리에 아이템 추가
                else
                {
                    AddItem(new BoItem(sdItem));
                }
            }
            // 장비라면
            else
            {
                // 장비는 무조건 인벤토리 한 칸을 차지하므로, 바로 인벤토리에 추가한다
                AddItem(new BoEquipment(sdItem));
            }

            // 드롭한 아이템을 인벤토리 추가했으므로,
            // 아이템 객체를 다시 아이템 풀에 반환한다.
            ObjectPoolManager.Instance.GetPool<Item>().Return(this);

            // 유저의 전체 아이템 정보에 변동이 생겼으므로,
            // 전체 아이템 정보를 서버에 보내면서, 변경사항을 db에 저장하도록 한다.
            //  -> 베스트는 변동된 아이템 정보만 보내고, 변동사항을 적용하는 것
            //     베스트의 경우는 나중에 아이템 드롭 확률 계산 및 생성을 서버쪽에서 처리하는 방식으로
            //     수정할 예정임, 그 때 변동 정보만 처리하는 방식으로 해볼게요

            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();

            void AddItem(BoItem boItem)
            {
                // ui 상에 데이터 반영
                uiInventory.AddItem(boItem);
                // 유저 아이템 정보에 반영
                userItems.Add(boItem);
            }
        }
    }
}