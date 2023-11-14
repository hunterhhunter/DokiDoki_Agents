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
        /// �ش� ������ ���� �ΰ��� ������ ������
        /// </summary>
        public BoItem BoItem { get; private set; }

        public void Initialize()
        {
            itemAmount ??= GetComponentInChildren<TextMeshProUGUI>();
            // ������ ���� ��ü�� ������ �̹���, ������ �̹���  2���� ������Ʈ�� �����ϹǷ�
            // ��������� ������ �̹����� ���� �ڽ� ��ü�� �����Ͽ� ������Ʈ�� ������
            itemImage ??= transform.GetChild(0).GetComponent<Image>();

            // �� ���Ե��� ������ �����ϰ� �ϱ� ���ؼ�
            SetSlot();
        }

        /// <summary>
        /// ���Կ� ������ �����͸� ������� UI�� �����ϴ� ���
        /// </summary>
        /// <param name="boItem"></param>
        public void SetSlot(BoItem boItem = null)
        {
            BoItem = boItem;

            // ���޵Ǵ� ������ ������ null�� ���?
            //  -> ���Կ� �������� �����ϰų�, �󽽷԰� �������� �����ϴ� ������ �����ϴ� ��� ��
            //     �Ҹ�ǰ�� ����ؼ� ������ 0�� �� ���
            if (boItem == null)
            {
                itemAmount.text = "";
                itemImage.sprite = null;
                // �󽽷��� ��� sprite�� �������� �����Ƿ�, ��� �̹����� ��µǹǷ�
                // ����Ⱦ ���ĸ� 0���� �༭ �����ϰ� ����
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