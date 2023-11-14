using AI_Project.DB;
using AI_Project.Dummy;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class UIInventory : UIWindow, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 모든 아이템 슬롯 객체를 갖는 하이라키 상의 부모 
        /// </summary>
        private Transform itemSlotHolder;
        /// <summary>
        /// 인벤토리 내 모든 아이템 슬롯을 갖는 리스트
        /// </summary>
        private List<ItemSlot> itemSlots = new List<ItemSlot>();

        /// <summary>
        /// 아이템을 드래그 시, 드래그를 끝낸 위치가 잘못된 위치라면 아이템이
        /// 제자리로 돌아갈 수 있도록 원래 위치하고 있던 곳의 좌표를 담아둔다.
        /// </summary>
        private Vector3 dragSlotOriginPos;
        /// <summary>
        /// 현재 드래그중인 슬롯의 참조
        /// </summary>
        private ItemSlot dragSlot;

        /// <summary>
        /// 인벤토리 정렬 버튼 참조
        /// </summary>
        public Button sortButton;
        /// <summary>
        /// UI 요소 레이캐스팅을 위한 gr
        /// </summary>
        public GraphicRaycaster gr;

        public override void Start()
        {
            base.Start();

            // 아이템 슬롯 홀더 참조 바인딩
            itemSlotHolder = transform.Find("Frame/ItemSlotHolder");

            // 모든 아이템 슬롯의 참조를 찾아서 리스트에 넣음
            for (int i = 0; i < itemSlotHolder.childCount; ++i)
            {
                itemSlots.Add(itemSlotHolder.GetChild(i).GetComponent<ItemSlot>());   
            }

            // 전체 슬롯 초기화
            InitAllSlot();

            // 모든 슬롯을 유저 아이템 정보를 기반으로 갱신
            InventoryUpdate();

            // 인벤토리 정렬 버튼 이벤트 바인딩
            sortButton.onClick.AddListener(OnSort);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (isOpen)
                    Close();
                else
                    Open();
            }
        }

        /// <summary>
        /// 기존에 가지고 있는 아이템의 수량 변동이 발생했을 때
        /// 뷰에도 해당 변동사항을 적용시키는 기능
        /// </summary>
        /// <param name="boItem"></param>
        public void IncreaseItem(BoItem boItem)
        {
            itemSlots[boItem.slotIndex].itemAmount.text = boItem.amount.ToString();
        }

        /// <summary>
        /// 인벤토리에 아이템을 추가하는 기능
        /// 1. 서버에서 아이템 정보를 받아서 추가할 때
        /// 2. 몬스터를 잡아서 드랍된 아이템을 루팅해서 추가할 때
        /// </summary>
        /// <param name="boItem"></param>
        public void AddItem(BoItem boItem)
        {
            // 추가된 아이템에 슬롯 인덱스가 정상적으로 존재하는지 검사
            //  -> 슬롯 인덱스가 정상적인 값이라면 서버에서 보내준 아이템 데이터
            //     아니라면, 몬스터를 잡아서 드랍된 아이템 (클라에서 만든 아이템 데이터)
            
            // 서버에서 준 거 (이전에 가지고 있던 아이템)
            if (boItem.slotIndex >= 0)
            {
                itemSlots[boItem.slotIndex].SetSlot(boItem);
                return;
            }

            // 클라에서 만든거 (새로 드롭한 아이템)
            for (int i = 0; i < itemSlots.Count; ++i)
            {
                // 슬롯이 비어있는지 확인
                if (itemSlots[i].BoItem == null)
                {
                    // 비었다면 해당 슬롯에 세팅 후 종료
                    boItem.slotIndex = i;
                    itemSlots[i].SetSlot(boItem);
                    return;
                }
            }
        }

        /// <summary>
        /// 유저의 아이템 정보를 받아 아이템 슬롯 UI를 갱신하는 기능
        /// </summary>
        private void InventoryUpdate()
        {
            var userItems = GameManager.User.boItems;
            for (int i = 0; i < userItems.Count; ++i)
            {
                AddItem(userItems[i]);
            }
        }

        /// <summary>
        /// 모든 아이템 슬롯 초기화 기능
        /// </summary>
        private void InitAllSlot()
        {
            for (int i = 0; i < itemSlots.Count; ++i)
                itemSlots[i].Initialize();
        }

        /// <summary>
        /// 드래그 시작 시, 한 번 호출 됨
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // gr을 이용하여 레이캐스팅을 실행하여 현재 마우스로
            // 클릭한 객체의 정보를 받아옴

            // 굳이 gr을 사용하지않고 eventData로 접근하여
            // 현재 선택한 객체의 정보를 얻을 수 있음 (단, 하나의 객체)
            // 하지만, 일반적으로 UI는 겹치는 요소들이 많고
            // UI의 이미지에 레이캐스트 타겟 설정을 전부 하지 않은 이상
            // 특정 상황에서 의도치 않게 내가 타겟으로 하는 UI 요소를 정상적으로
            // 캐치하지 못하는 경우가 발생하므로
            // eventData 대신 gr을 이용하여 직접 레이캐스팅을 실행
            // 결과 -> evenData.selectedObject (단 하나의 대상)
            //         gr을 이용한 직접적인 레이캐스팅 (여러개의 대상)

            var result = new List<RaycastResult>();
            gr.Raycast(eventData, result);

            // 드래그하려고 하는 객체가 아이템슬롯의 아이템인지?
            for (int i = 0; i < result.Count; ++i)
            {
                // 이름에 아이템슬롯이 들어가 있으면? 
                //  -> 추후 이름보다는 태그를 이용하여 비교하게 수정하면 좋을듯
                if (result[i].gameObject.name.Contains("ItemSlot"))
                {
                    // 클릭한 슬롯의 참조를 가져온 후, dragSlot으로 설정
                    dragSlot = result[i].gameObject.GetComponent<ItemSlot>();
                    // 드래그를 시작하기 전, 원래의 위치를 담아둠
                    dragSlotOriginPos = dragSlot.itemImage.transform.position;
                    break;
                }
            }
        }

        /// <summary>
        /// 드래그 도중 계속해서 호출됨
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            // dragSlot이 없다면 종료
            if (dragSlot == null)
                return;

            // dragSlot의 ItemImage 객체를 마우스를 따라 이동하도록
            dragSlot.itemImage.transform.position = Input.mousePosition;
        }
        
        /// <summary>
        /// 드래그 끝낼 시, 한 번 호출 됨
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // dragSlot이 없다면 종료
            if (dragSlot == null)
                return;

            var result = new List<RaycastResult>();
            gr.Raycast(eventData, result);

            // 드래그하던 아이템 이미지를 무조건 원래 위치로 되돌린다.
            // 아이템의 위치가 바뀔건데 이미지를 원래대로 되돌려놓는 이유?
            // 아이템의 데이터를 변경된 슬롯 위치로 세팅하고
            // 데이터가 변경되었으므로, 아이템 슬롯의 SetSlot을 통해 변경된
            // 데이터에 알맞는 이미지,수량 등을 갱신시킨다.
            // -> 결과적으로, 아이템을 옮긴 것과 동일함

            // -> 눈에 보이는 뷰에 신경을 쓰는게 아니라, 데이터에 초점을 맞춰서
            //    작업을 하겠다는 것
            dragSlot.itemImage.transform.position = dragSlotOriginPos;

            // 드래그를 끝낸 시점, 마우스를 뗐을 때 마우스가 위치하고 있는 아이템 슬롯을 찾는다.
            // 만약 마우스 위치에 아이템 슬롯이 존재하지 않는다면, 데이터를 넘기지 않고 메서드 종료
            ItemSlot destSlot = null;

            for (int i = 0; i < result.Count; ++i)
            {
                // 마우스 위치의 슬롯이 드래그하던 슬롯이라면 넘긴다. 
                if (result[i].gameObject == dragSlot.gameObject)
                    continue;

                if (result[i].gameObject.name.Contains("ItemSlot"))
                {
                    destSlot = result[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // destSlot이 없다는 것은 슬롯이 아닌 다른 위치에서 드래그를 끝냈다는 의미이므로 종료
            if (destSlot == null)
                return;

            var boItems = GameManager.User.boItems;

            // 드래고 하고 있던 아이템의 정보를 복사함
            var tempBoItem = dragSlot.BoItem.Copy();

            // 기존 드래그하던 아이템의 정보를 유저의 아이템 정보에서 지우고,
            // 복사한 아이템 정보를 추가함
            boItems.Remove(dragSlot.BoItem);
            boItems.Add(tempBoItem);

            // 드래그 하고 있던 슬롯에는 방금 마우스를 뗀 위치에 존재하는 슬롯의
            // 아이템 정보를 세팅
            dragSlot.SetSlot(destSlot.BoItem);
            // 이후, dragSlot의 위치에 맞는 슬롯인덱스를 변경
            SetSlotIndex(dragSlot);

            // 마우스를 뗀 위치에 존재하는 슬롯에는 기존 드래그하던 슬롯의 정보를 세팅
            destSlot.SetSlot(tempBoItem);
            // 이후, destSlot의 위치에 맞는 슬롯인덱스로 변경
            SetSlotIndex(destSlot);

            // db에도 데이터를 업데이트
            DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
            DummyServer.Instance.Save();

            // 해당 슬롯이 인벤토리 내 배치된 위치에 맞는 슬롯인덱스 값을
            // 설정하느 기능
            void SetSlotIndex(ItemSlot slot)
            {
                // 빈슬롯이라면 아이템정보가 존재하지 않으므로
                if (slot.BoItem == null)
                    return;

                // 현재 슬롯의 이름에는 해당 슬롯인덱스를 나타내는 정수가 포함되어 있음
                // 그럼 슬롯의 이름으로 접근하여 정수값만 가져온다면, 슬롯 인덱스를 나타내게 됨
                string index = Regex.Replace(slot.gameObject.name, @"[^\d]", "");
                slot.BoItem.slotIndex = int.Parse(index);
            }
        }

        /// <summary>
        /// 인벤토리 정렬 기능
        ///  -> UI 정렬버튼을 하나 배치하여, 정렬버튼을 누를 시 해당 메서드가 호출됨
        /// </summary>
        private void OnSort()
        {
            // 정렬 기준
            // 아이템 이름(sdItem.name), 오름차순으로

            // Linq
            // OrderBy 함수 쿼리를 이용하여 아이템 데이터를 이름순으로 정렬

            // 슬롯으로 접근할 필요없이, 유저 데이터에 접근해서 아이템 데이터를 받아옴
            var boItems = GameManager.User.boItems;

            // 아이템 데이터를 이름을 기준으로 오름차순으로 정렬
            var sortedItems = boItems.OrderBy(_ => _.sdItem?.name).ToList();

            // 정렬된 데이터를 적용 전에, 전체 슬롯 초기화
            // 슬롯에 바인딩 되었던 아이템 정보 참조가 전부 날아감
            InitAllSlot();

            // 정렬시켰던 아이템 정보를 순서대로 슬롯에 적용
            // 위와 같은 방식으로 작업할 시, 빈 슬롯에 대해 고려하지않아도 됨
            for (int i = 0; i < sortedItems.Count; ++i)
            {
                itemSlots[i].SetSlot(sortedItems[i]);
                itemSlots[i].BoItem.slotIndex = i;
            }

            // 아이템 정보가 변경되었으므로 db 갱신
            DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
            DummyServer.Instance.Save();
        }
    }
}
