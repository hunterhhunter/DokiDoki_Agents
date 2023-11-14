using AI_Project.DB;
using AI_Project.Resource;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AI_Project.UI
{
    public class ItemSlot : MonoBehaviour
    {
        public Image itemImage;
        public TextMeshProUGUI itemAmount;

        /// <summary>
        /// 해당 슬롯이 갖는 인게임 아이템 데이터
        /// </summary>
        public BoItem BoItem { get; private set; }

        public void Initialize()
        {
            itemAmount ??= GetComponentInChildren<TextMeshProUGUI>();
            // 아이템 슬롯 객체에 프레임 이미지, 아이템 이미지  2개의 컴포넌트가 존재하므로
            // 명시적으로 아이템 이미지를 갖는 자식 객체로 접근하여 컴포넌트를 가져옴
            itemImage ??= transform.GetChild(0).GetComponent<Image>();

            // 빈 슬롯들의 색상을 투명하게 하기 위해서
            SetSlot();
        }

        /// <summary>
        /// 슬롯에 아이템 데이터를 기반으로 UI를 세팅하는 기능
        /// </summary>
        /// <param name="boItem"></param>
        public void SetSlot(BoItem boItem = null)
        {
            BoItem = boItem;

            // 전달되는 아이템 정보가 null인 경우?
            //  -> 슬롯에 아이템을 제거하거나, 빈슬롯과 아이템이 존재하는 슬롯을 스왑하는 경우 등
            //     소모품을 사용해서 수량이 0이 된 경우
            if (boItem == null)
            {
                itemAmount.text = "";
                itemImage.sprite = null;
                // 빈슬롯의 경우 sprite가 존재하지 않으므로, 흰색 이미지가 출력되므로
                // 보기싫어서 알파를 0으로 줘서 투명하게 만듬
                itemImage.color = new Color(1,1,1,0);
            }
            else
            {
                itemAmount.text = boItem.amount.ToString();
                itemImage.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlas,
                    boItem.sdItem.resourcePath);
                itemImage.color = Color.white;
            }
        }
    }
}