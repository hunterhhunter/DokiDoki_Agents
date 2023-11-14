using AI_Project.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.UI
{
    /// <summary>
    /// UIWindow�� ������� �Ļ��� ��� ��ü���� ������ �Ŵ���
    /// </summary>
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        // UIWindowManager == UWM, UIWindow == UW

        /// <summary>
        /// UWM�� ��ϵ� Ȱ��ȭ �Ǿ��ִ� ��� UW�� ���� ����Ʈ
        /// </summary>
        private List<UIWindow> totalOpenWindows = new List<UIWindow>();
        /// <summary>
        /// UWM�� ��ϵ� ��� UW�� ���� ����Ʈ
        ///  -> �ݺ����� ����� �� (��ü ���Ҹ� ��ȸ�� ��)
        /// </summary>
        private List<UIWindow> totalUIWindows = new List<UIWindow>();
        /// <summary>
        /// UWM�� ��ϵ� ��� UW�� ���� ��ųʸ�
        ///  -> Ư�� ������ Ž��
        /// </summary>
        private Dictionary<string, UIWindow> cachedTotalUIWindows = new Dictionary<string, UIWindow>();
        /// <summary>
        /// UWM�� ��ϵ� UW�� �ν��Ͻ� ���� ��, �ش� �ν��Ͻ����� ���� �Ļ����� Ÿ������ ĳ���Ͽ� ��Ƶ� ��ųʸ�
        ///  -> ��������� ���� �ڵ�� ���� �����Ͽ� ã�����ϴ� �Ļ������� UW ��ü�鸸 ���
        /// </summary>
        private Dictionary<string, object> cachedInstances = new Dictionary<string, object>();

        public void Initialize()
        {
            InitAllWindow();
        }

        private void Update()
        {
            // UW�� esc�� â�� ������ �� �ִ����� ���� �������� ���� ����
            // ����, esc Ű �Է��� üũ�ϰ�, �Է� �ÿ� ���� �ֱٿ� Ȱ��ȭ�� UW�� �ݵ���
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var target = GetTopWindow();

                if (target != null && target.canCloseESC)
                    target.Close();
            }
        }

        /// <summary>
        /// UWM�� UW �ν��Ͻ��� ����ϴ� ���
        /// </summary>
        /// <param name="uiWindow">����ϰ����ϴ� UW �ν��Ͻ�</param>
        public void AddTotalWindow(UIWindow uiWindow)
        {
            // ��ųʸ��� ��� �� or ���� �� ����� Ű ��
            var key = uiWindow.GetType().Name;
            // ��ųʸ��� �ش� Ű�� ��ϵǾ��ִ����� ��Ÿ���� ����
            var hasKey = false;

            // ��ü UW ����Ʈ�� ����ϰ��� �ϴ� �ν��Ͻ��� �ִ���?
            // �Ǵ� ��ü UW ��ųʸ��� �ش� �ν��Ͻ� Ÿ���̸��� Ű ���� �����ϴ���?
            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindows.ContainsKey(key))
            {
                // ��ųʸ��� Ű ������ ����� ���� ��, �ش� ����� null�� �ƴ϶�� ����
                if (cachedTotalUIWindows[key] != null)
                    // �̹� �ش� �ν��Ͻ��� �����ϹǷ� ����� ������ �ʿ� ���� ����
                    return;
                // Ű ���� �ִµ� ����� null�̶�� �����ϰ� �ִ� UW�� �ν��Ͻ��� ���ٴ� ��
                else
                {
                    // ex) Ÿ��Ʋ������ ����ϴ� UI�� UWM�� ��ϵǾ��ִµ�
                    //     �ΰ������� ����ȯ�ϸ鼭 �ش� UI�� �ν��Ͻ��� �ı��Ǽ� ����� null�� �Ǿ��ִ� ���..

                    hasKey = true;

                    // ��ü UW ����Ʈ���� �׷� �����ϰ� null�� ���� �� ���� �� �����Ƿ�
                    // ��ȸ�ϸ鼭 null�� ���Ҹ� ����
                    for (int i = 0; i < totalUIWindows.Count; ++i)
                    {
                        if (totalUIWindows[i] == null)
                            totalUIWindows.RemoveAt(i);
                    }
                }
            }

            // �������� ���Դٴ� ����, UW �ν��Ͻ��� ����� �ʿ��ϴٴ� �̾߱�
            // ����, ��ü UW ����Ʈ, ��ųʸ��� ����Ѵ�.

            totalUIWindows.Add(uiWindow);

            if (hasKey)
                cachedTotalUIWindows[key] = uiWindow;
            else
                cachedTotalUIWindows.Add(key, uiWindow);
        }

        /// <summary>
        /// ��ü Ȱ��ȭ�� UW ��Ͽ� Ȱ��ȭ�� UW�� ����ϴ� ���
        /// </summary>
        /// <param name="uiWindow">����ϰ����ϴ� UW �ν��Ͻ�</param>
        public void AddOpenWindow(UIWindow uiWindow)
        {
            // totalOpenWindows ����Ʈ�� �̹� �����ϴ��� Ȯ�� ��, ���ٸ� ���
            if (!totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Add(uiWindow);
        }

        /// <summary>
        /// ��ü Ȱ��ȭ�� UW ��Ͽ� ��Ȱ��ȭ�� UW�� �����ϴ� ���
        /// </summary>
        /// <param name="uiWindow">�����ϰ����ϴ� UW �ν��Ͻ�</param>
        public void RemoveOpenWindow(UIWindow uiWindow)
        {
            // totalOpenWindows ����Ʈ�� �̹� �����ϴ��� Ȯ�� ��, �����Ѵٸ� ����
            if (totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Remove(uiWindow);
        }

        /// <summary>
        /// UWM�� ��ϵ� TŸ�� ������ UW�� ��ȯ�ϴ� ���
        ///  -> ���������, ���� �Ļ����·� UW �ν��Ͻ��� ������ �� �ְ� �ȴ�.
        /// </summary>
        /// <typeparam name="T">��ȯ�ް��� �ϴ� UW�� Ÿ��</typeparam>
        /// <returns>TŸ���� UW �ν��Ͻ�</returns>
        public T GetWindow<T>() where T : UIWindow
        {
            string key = typeof(T).Name;

            // ��ü UW ��ųʸ��� ������ null ��ȯ
            if (!cachedTotalUIWindows.ContainsKey(key))
                // ��ϵ��� ���� Ÿ���� �ν��Ͻ� ��û�̹Ƿ� ����
                return null;

            // ���������� TŸ���� UW �ν��Ͻ��� �ν��Ͻ� ��ųʸ��� ���� ��ȯ�ȴ�.
            // �׷� �� ��, ã���� �ϴ� �ν��Ͻ��� �ν��Ͻ� ��ųʸ��� �����ϴ��� �˻��Ѵ�.
            if (!cachedInstances.ContainsKey(key))
            {
                // �� ���� ���Դٴ� ����, ��ϵ��� �ʾҴٴ� ��
                // ���� ��ü UW ��ųʸ����� �ش� �ν��Ͻ��� ������ TŸ������ ĳ���� ��
                // �ν��Ͻ� ��ųʸ��� ����Ѵ�.
                cachedInstances.Add(key, (T)Convert.ChangeType(cachedTotalUIWindows[key], typeof(T)));
            }
            // �ν��Ͻ� ��ųʸ��� Ű�� ������, �� �� ����� �����ϴ��� �˻�
            // ���� ����� null�̶�� ���� �����ϰ� ��ü UW ��ųʸ����� �ν��Ͻ��� ������ ���
            else if (cachedInstances[key].Equals(null))
            {
                cachedInstances[key] = (T)Convert.ChangeType(cachedTotalUIWindows[key], typeof(T));
            }

            // ���� ������ ����, ���������� �ν��Ͻ� ��ųʸ����� TŸ�� ������ UW �ν��Ͻ��� �����
            // ����, Ư�� TŸ���� �ν��Ͻ��� �������� ����� ȣ�� ��, �Ź� ĳ�����Ͽ� ��ȯ�� �ʿ� ����
            // ĳ�̵� �����͸� ����ϹǷ� ������� ������ ������ �� �ִ�.
            //  -> ���� ���ɻ� ū �ǹ̴� ����, ���� ���� ������ TŸ���� �ν��Ͻ��� ���ٴ� ������
            //     �ν��Ͻ� ���ٿ� ���� ȣ�� ���� ��, ���� ���� ����..
            return (T)cachedInstances[key];
        }

        /// <summary>
        /// UWM�� ��ϵ� ��� UW�� �ݴ� ���
        /// </summary>
        public void CloseAll()
        {
            for (int i = 0; i < totalUIWindows.Count; ++i)
            {
                totalUIWindows[i]?.Close();
            }
        }

        /// <summary>
        /// UWM�� ��ϵ� ��� UW�� �ʱ�ȭ�ϴ� ���
        /// </summary>
        public void InitAllWindow()
        {
            for (int i = 0; i < totalUIWindows.Count; ++i)
            {
                totalUIWindows[i]?.InitWindow();
            }
        }

        /// <summary>
        /// ���� �����ִ� UW �� ���� �ֻ���(���� �������� ����) UW �ν��Ͻ��� ��ȯ�ϴ� ���
        ///  -> ����� ����̽� ��� �����ϰ� ��� ���� 
        ///  (����� ��⿡�� �ڷΰ��� ��ư�� ���� ��, �Ϲ������� ���� �ֱٿ� Ȱ��ȭ�� UI�� ��Ȱ��ȭ �ǹǷ�
        ///   �̷��� ���, ���� �ֱٿ� Ȱ��ȭ�� UI ��ü�� ���ϰ� ���� �� �ִ�)
        /// </summary>
        /// <returns></returns>
        public UIWindow GetTopWindow()
        {
            // ��ü Ȱ�� UW ��Ͽ��� Ȱ��ȭ�� ������� UW ��ü�� �߰��ǹǷ�
            // ����� ���� ���ʿ������� �ν��Ͻ��� �����ϴ��� Ȯ�� ��, �����Ѵٸ� ��ȯ �Ѵٸ�
            // ��������� ���� �ֱٿ� Ȱ��ȭ�� UW ��ü�� ��ȯ��
            for (int i = totalOpenWindows.Count - 1; i > 0; --i)
            {
                if (totalOpenWindows[i] != null)
                    return totalOpenWindows[i];
            }

            return null;
        }

    } 
}