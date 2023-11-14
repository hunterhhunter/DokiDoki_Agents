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
        // ������ ��ȹ ������
        private SDItem sdItem;
        // ���� �������� ����� �̹��� ������Ʈ ����
        private Image icon; 

        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// ���Ͱ� ���� ��, �������� ����Ǹ� �ش� �������� �ε�����
        /// ������ ��ü �ʱ�ȭ �� ������
        /// </summary>
        /// <param name="itemIndex"></param>
        public void Initialize(int itemIndex)
        {
            // ���޹��� ������ �ε����� ������� ��ȹ �����͸� ������
            sdItem = GameManager.SD.sdItems.Where(_ => _.index == itemIndex).SingleOrDefault();
            // ������ ��������Ʈ �ҷ���
            var itemSprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlas, sdItem.resourcePath);
            // �ҷ��� ��������Ʈ�� �̹��� ������Ʈ�� ����
            icon ??= GetComponent<Image>();
            icon.sprite = itemSprite;
            // �̹��� ������Ʈ�� ��������Ʈ�� ����Ƽ�� ������� ����
            icon.SetNativeSize();
        }

        private void OnTriggerEnter(Collider other)
        {
            // �����۰� ��ģ �ݶ��̴��� ���� �±װ� �÷��̾ �ƴ϶�� ����
            if (!other.gameObject.CompareTag("Player"))
                return;

            // ���� ������ ��� �Դ���, ���� �ٸ� ������ ����� ����
            // ����� �������� ������� Ȯ��
            var isEquip = sdItem.itemType == Define.Item.Type.Equipment;

            // ���� ������ ���� ���� �޾ƿ�
            var userItems = GameManager.User.boItems;
            // UI �κ��丮 ���� �޾ƿ�
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // ��� �ƴ� �������̶��
            if (!isEquip)
            {
                // ������ �ش� �������� �̹� ������ �ִ��� Ȯ��
                var sameItem = userItems.Where(_ => _.sdItem.index == sdItem.index).SingleOrDefault();

                // �̹� ������ �ִ� �������̶�� ������ �÷��ְ�
                if (sameItem != null)
                {
                    ++sameItem.amount;
                    uiInventory.IncreaseItem(sameItem);
                }
                // ������ ���� �ʴٸ� �κ��丮�� ������ �߰�
                else
                {
                    AddItem(new BoItem(sdItem));
                }
            }
            // �����
            else
            {
                // ���� ������ �κ��丮 �� ĭ�� �����ϹǷ�, �ٷ� �κ��丮�� �߰��Ѵ�
                AddItem(new BoEquipment(sdItem));
            }

            // ����� �������� �κ��丮 �߰������Ƿ�,
            // ������ ��ü�� �ٽ� ������ Ǯ�� ��ȯ�Ѵ�.
            ObjectPoolManager.Instance.GetPool<Item>().Return(this);

            // ������ ��ü ������ ������ ������ �������Ƿ�,
            // ��ü ������ ������ ������ �����鼭, ��������� db�� �����ϵ��� �Ѵ�.
            //  -> ����Ʈ�� ������ ������ ������ ������, ���������� �����ϴ� ��
            //     ����Ʈ�� ���� ���߿� ������ ��� Ȯ�� ��� �� ������ �����ʿ��� ó���ϴ� �������
            //     ������ ������, �� �� ���� ������ ó���ϴ� ������� �غ��Կ�

            DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            DummyServer.Instance.Save();

            void AddItem(BoItem boItem)
            {
                // ui �� ������ �ݿ�
                uiInventory.AddItem(boItem);
                // ���� ������ ������ �ݿ�
                userItems.Add(boItem);
            }
        }
    }
}