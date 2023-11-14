using AI_Project.Object;
using AI_Project.UI;
using AI_Project.Util;
using System;
using UnityEngine;
using UnityEngine.U2D;

namespace AI_Project.Resource
{
    /// <summary>
    /// ��Ÿ��(����ð�)�� �ʿ��� ���ҽ��� �ҷ����� ����� ����� Ŭ����
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        public void Initialize()
        {
            LoadAllAtlas();
            LoadAllPrefabs();
        }

        /// <summary>
        /// Assets/Resources ���� ���� �������� �ҷ��� ��ȯ�ϴ� ���
        /// </summary>
        /// <param name="path">Resources ���� �� �ҷ��� ���� ���</param>
        /// <returns>�ҷ��� ���ӿ�����Ʈ</returns>
        public GameObject LoadObject(string path)
        {
            // Resources.Load -> Assets ���� �� Resources ��� �̸��� ������ �����Ѵٸ�
            // �ش� ��κ��� path�� ����, �ش� ��ο� ������ GameObject ���·� �θ� �� �ִٸ� �ҷ���
            return Resources.Load<GameObject>(path);
        }

        /// <summary>
        /// ������Ʈ Ǯ�� ����� ��ü�� �������� �ε� ��, ������Ʈ Ǯ �Ŵ����� �̿��Ͽ� Ǯ�� ����ϴ� ���
        /// </summary>
        /// <typeparam name="T">�ε��ϰ����ϴ� Ÿ��</typeparam>
        /// <param name="path">������ ���</param>
        /// <param name="poolCount">Ǯ ��� ��, �ʱ� �ν��Ͻ� ��</param>
        /// <param name="loadComplete">�ε� �� ����� �Ϸ� ��, �����ų �̺�Ʈ</param>
        public void LoadPoolableObject<T>(string path, int poolCount = 1, Action loadComplete = null)
            where T : MonoBehaviour, IPoolableObject
        {
            // �������� �ε��Ѵ�.
            var obj = LoadObject(path);
            // �������� ���� �ִ� TŸ�� ������Ʈ ������ �����´�.
            var tComponent = obj.GetComponent<T>();

            // tŸ���� Ǯ�� ���
            ObjectPoolManager.Instance.RegistPool<T>(tComponent, poolCount);

            // ���� �۾��� ��� ���� ��, �����ų ����� �ִٸ�? ����
            loadComplete?.Invoke();
        }

        /// <summary>
        /// Resources ���� ���� ��� ��Ʋ�󽺸� �ҷ��� ��������Ʈ �δ��� ��Ͻ�Ų��.
        /// </summary>
        private void LoadAllAtlas()
        {
            // ����� ��� ��Ʋ�󽺸� ó���� �ҷ��� ��������Ʈ �δ����� ��� ��� ������
            // ���� ������ ����, �ؽ��İ� ���ٸ� �� ���� ��, �ʿ� ���� ��Ʋ�󽺸� �����ϴ� ����� ��õ
            var atlases = Resources.LoadAll<SpriteAtlas>("Sprite");
            SpriteLoader.Initialize(atlases);
        }

        /// <summary>
        /// �ΰ��ӿ��� ���������� ���Ǵ� �����յ��� �θ��� ���
        ///   -> �ַ� �ΰ��� ������ ������Ʈ Ǯ�� ����Ͽ� ����� �����յ��� �ַ� �θ� ����
        /// </summary>
        private void LoadAllPrefabs()
        {
           /* LoadPoolableObject<DialogueButton>("Prefabs/UI/DialogueButton", 5);
            LoadPoolableObject<HpBar>($"Prefabs/UI/HpBar", 10);
            LoadPoolableObject<Item>($"Prefabs/Item", 10);
            LoadPoolableObject<QuestSlot>("Prefabs/UI/QuestSlot", 10);
            LoadPoolableObject<Projectile>("Prefabs/Projectile", 10);*/
        }
    }
}