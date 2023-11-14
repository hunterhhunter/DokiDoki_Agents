using AI_Project.Network;
using AI_Project.Util;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace AI_Project.Dummy
{
    /// <summary>
    /// ���̼����� ������ ������ Ŭ����
    /// ���̼������� ����� db�� ���´�.
    /// </summary>
    public class DummyServer : Singleton<DummyServer>
    {
        /// <summary>
        /// ���̼������� ���� ���������� (���� DB)
        /// </summary>
        public UserDataSo userData;
        /// <summary>
        /// ���̼����� ��� ����� ���� ���
        /// </summary>
        public INetworkClient dummyModule;

        private Coroutine saveCoroutine;

        public void Initialize()
        {
            dummyModule = new ServerModuleDummy(this);
        }

        /// <summary>
        /// ���� ���� �����͸� �����ϴ� ���
        /// ��Ÿ�ӿ� db �����Ϳ� ���������� ������ ���, ��������� �����ϴ� ���
        /// </summary>
        public void Save()
        {
            // ���� db �����ʹ� ��ũ���ͺ� ������Ʈ�� ������� �ۼ��Ǿ��� ������
            // �������� �����Ϳ����� ����
            // ��, �ش� ����� ������ ���� ���� �ƴ�

            // �����ų ���� �����͸� ��Ƽ �÷��׷� ����
            //  ����Ƽ���� ��Ÿ�ӿ� ���Ǵ� (������ �Ǵ� ��ũ���ͺ� ������Ʈ) ����
            //  �Ϲ������� �ֹ߼� �������̰�, ���������� ������ �����ϴ� �������� ���Ǵ�
            //  �����Ͱ� �ƴ� 
            //  ������, ��Ÿ�� �� ���Ǵ� �����͸� �����ϰ� ���� ��, ��ũ�� �� �� �ְ�
            //  ��Ƽ �÷��׸� �����ϸ� ������ 

            if (saveCoroutine != null)
            {
                StopCoroutine(saveCoroutine);
                saveCoroutine = null;
            }

            saveCoroutine = StartCoroutine(SaveProgress());

            IEnumerator SaveProgress()
            {
                EditorUtility.SetDirty(userData);
                AssetDatabase.SaveAssets();

                yield return null;
            }
        }
    }
}