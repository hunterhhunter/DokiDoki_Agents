using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.UI
{
    // RequireComponent - Ư�� ������Ʈ�� �������� �����ϴ� ���
    //  -> ��������� �ش� ��ũ��Ʈ�� �߰� ��, ������ ĵ���� �׷쵵 �߰��ȴ�.
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// ��� UI�� ���̽� Ŭ���� (���׸��� �˾�, ū UI ���� UI ��ҵ��� ������)
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        /// <summary>
        /// ĵ�����׷� ������ ���� �ʵ�
        /// </summary>
        private CanvasGroup cachedCanvasGroup;
        public CanvasGroup CachedCanvasGroup
        {
            get
            {
                if (cachedCanvasGroup == null)
                    cachedCanvasGroup = GetComponent<CanvasGroup>();

                return cachedCanvasGroup;
            }
        }

        /// <summary>
        /// �ش� UI�� esc Ű�� ���� �� �ְ� �����Ұ���?
        /// </summary>
        public bool canCloseESC;
        /// <summary>
        /// UI�� Ȱ��ȭ ����
        /// </summary>
        public bool isOpen;

        public virtual void Start()
        {
            InitWindow();
        }

        /// <summary>
        /// UI �ʱ�ȭ ���
        /// </summary>
        public virtual void InitWindow()
        {
            // UWM�� ���
            UIWindowManager.Instance.AddTotalWindow(this);

            // UIWindow�� ������� �Ļ��� ��� UI�� �ʱ⿡ ��Ȱ��ȭ�� ���·� 
            // ��ġ�� UI ��� �ϴ���, ���� Ȱ��ȭ�� ���·� ��ġ�� ���̴�.
            //  -> ����?, ��Ȱ��ȭ�� ���¿����� �ʱ��� Start �ݹ��� ���������� �������������Ƿ�
            // ����, Start �ݹ��� ����� �� �ְ� ���� Ȱ�����·� ���� ��ġ�ص� ��
            // �ݹ鿡�� InitWindow �޼��带 ���� �ʵ忡 �̸� �����ص� isOpen ���� ����
            // ��Ȱ��ȭ�Ǿ���� UI���� �ڵ������� ��Ȱ��ȭ�� ����
            if (isOpen)
                Open(true);
            else
                Close(true);
        }

        /// <summary>
        /// UI Ȱ��ȭ ���
        /// </summary>
        /// <param name="force">������ Ȱ��ȭ ��ų����?</param>
        public virtual void Open(bool force = false)
        {
            if (!isOpen || force)
            {
                isOpen = true;
                // UWM�� Ȱ�� UW ��Ͽ� ���
                UIWindowManager.Instance.AddOpenWindow(this);
                SetCanvasGroup(true);
            }
        }

        /// <summary>
        /// UI ��Ȱ��ȭ ���
        /// </summary>
        /// <param name="force">������ ��Ȱ��ȭ ��ų����?</param>
        public virtual void Close(bool force = false)
        {
            if (isOpen || force)
            {
                isOpen = false;
                // UWM�� Ȱ�� UW ��Ͽ��� ����
                UIWindowManager.Instance.RemoveOpenWindow(this);
                SetCanvasGroup(false);
            }
        }

        /// <summary>
        /// ĵ���� �׷� �� �ʵ带 UI Ȱ��/��Ȱ�� ���ο� ���� �����ϴ� ���
        /// </summary>
        /// <param name="isActive">UI Ȱ��/��Ȱ�� ����</param>
        private void SetCanvasGroup(bool isActive)
        {
            // �ش� UI ��� ��� ��, ���� ���� ��Ÿ�� (���� 0���� 1����)
            // 0 �̶�� ����, 1�̶�� ������, �� �� ���İ� 0�̵Ǹ� �ش� ��ü��
            // ���̶�Ű ���� �ڽ� UI�鵵 ��� �����ϰ� ����ȴ�.
            CachedCanvasGroup.alpha = Convert.ToInt32(isActive);
            // ĵ���� �׷��� �޸� ��ü�� �ڽ� UI������ ���ͷ����� �����Ѵ�.
            CachedCanvasGroup.interactable = isActive;
            // ĵ���� �׷��� �ٸ� ��ü�� �ڽ� UI������ UI ����ĳ���� ���
            // ����/������ ��ų �������� �����Ѵ�.
            CachedCanvasGroup.blocksRaycasts = isActive;
        }
    }
}