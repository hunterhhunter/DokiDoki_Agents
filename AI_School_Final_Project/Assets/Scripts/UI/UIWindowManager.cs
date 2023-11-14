using AI_Project.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.UI
{
    /// <summary>
    /// UIWindow를 기반으로 파생된 모든 객체들을 관리할 매니저
    /// </summary>
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        // UIWindowManager == UWM, UIWindow == UW

        /// <summary>
        /// UWM에 등록된 활성화 되어있는 모든 UW를 갖는 리스트
        /// </summary>
        private List<UIWindow> totalOpenWindows = new List<UIWindow>();
        /// <summary>
        /// UWM에 등록된 모든 UW를 갖는 리스트
        ///  -> 반복문에 사용할 때 (전체 원소를 순회할 때)
        /// </summary>
        private List<UIWindow> totalUIWindows = new List<UIWindow>();
        /// <summary>
        /// UWM에 등록된 모든 UW를 갖는 딕셔너리
        ///  -> 특정 데이터 탐색
        /// </summary>
        private Dictionary<string, UIWindow> cachedTotalUIWindows = new Dictionary<string, UIWindow>();
        /// <summary>
        /// UWM에 등록된 UW의 인스턴스 접근 시, 해당 인스턴스들을 최종 파생형태 타입으로 캐싱하여 담아둘 딕셔너리
        ///  -> 결과적으로 내가 코드로 직접 접근하여 찾고자하는 파생형태의 UW 객체들만 담김
        /// </summary>
        private Dictionary<string, object> cachedInstances = new Dictionary<string, object>();

        public void Initialize()
        {
            InitAllWindow();
        }

        private void Update()
        {
            // UW는 esc로 창을 종료할 수 있는지에 대한 설정값을 갖고 있음
            // 따라서, esc 키 입력을 체크하고, 입력 시에 가장 최근에 활성화된 UW를 닫도록
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var target = GetTopWindow();

                if (target != null && target.canCloseESC)
                    target.Close();
            }
        }

        /// <summary>
        /// UWM에 UW 인스턴스를 등록하는 기능
        /// </summary>
        /// <param name="uiWindow">등록하고자하는 UW 인스턴스</param>
        public void AddTotalWindow(UIWindow uiWindow)
        {
            // 딕셔너리에 등록 시 or 접근 시 사용할 키 값
            var key = uiWindow.GetType().Name;
            // 딕셔너리에 해당 키가 등록되어있는지를 나타내는 변수
            var hasKey = false;

            // 전체 UW 리스트에 등록하고자 하는 인스턴스가 있는지?
            // 또는 전체 UW 딕셔너리에 해당 인스턴스 타입이름의 키 값이 존재하는지?
            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindows.ContainsKey(key))
            {
                // 딕셔너리에 키 값으로 밸류에 접근 시, 해당 밸류가 null이 아니라면 리턴
                if (cachedTotalUIWindows[key] != null)
                    // 이미 해당 인스턴스가 존재하므로 등록을 진행할 필요 없이 종료
                    return;
                // 키 값은 있는데 밸류가 null이라면 참조하고 있는 UW의 인스턴스가 없다는 것
                else
                {
                    // ex) 타이틀씬에서 사용하던 UI가 UWM에 등록되어있는데
                    //     인게임으로 씬전환하면서 해당 UI의 인스턴스가 파괴되서 밸류가 null로 되어있는 경우..

                    hasKey = true;

                    // 전체 UW 리스트에도 그럼 동일하게 null인 값이 들어가 있을 수 있으므로
                    // 순회하면서 null인 원소를 제거
                    for (int i = 0; i < totalUIWindows.Count; ++i)
                    {
                        if (totalUIWindows[i] == null)
                            totalUIWindows.RemoveAt(i);
                    }
                }
            }

            // 이쪽으로 들어왔다는 것은, UW 인스턴스의 등록이 필요하다는 이야기
            // 따라서, 전체 UW 리스트, 딕셔너리에 등록한다.

            totalUIWindows.Add(uiWindow);

            if (hasKey)
                cachedTotalUIWindows[key] = uiWindow;
            else
                cachedTotalUIWindows.Add(key, uiWindow);
        }

        /// <summary>
        /// 전체 활성화된 UW 목록에 활성화된 UW를 등록하는 기능
        /// </summary>
        /// <param name="uiWindow">등록하고자하는 UW 인스턴스</param>
        public void AddOpenWindow(UIWindow uiWindow)
        {
            // totalOpenWindows 리스트에 이미 존재하는지 확인 후, 없다면 등록
            if (!totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Add(uiWindow);
        }

        /// <summary>
        /// 전체 활성화된 UW 목록에 비활성화된 UW를 제거하는 기능
        /// </summary>
        /// <param name="uiWindow">제거하고자하는 UW 인스턴스</param>
        public void RemoveOpenWindow(UIWindow uiWindow)
        {
            // totalOpenWindows 리스트에 이미 존재하는지 확인 후, 존재한다면 제거
            if (totalOpenWindows.Contains(uiWindow))
                totalOpenWindows.Remove(uiWindow);
        }

        /// <summary>
        /// UWM에 등록된 T타입 유형의 UW를 반환하는 기능
        ///  -> 결과적으로, 최종 파생형태로 UW 인스턴스를 가져올 수 있게 된다.
        /// </summary>
        /// <typeparam name="T">반환받고자 하는 UW의 타입</typeparam>
        /// <returns>T타입의 UW 인스턴스</returns>
        public T GetWindow<T>() where T : UIWindow
        {
            string key = typeof(T).Name;

            // 전체 UW 딕셔너리에 없으면 null 반환
            if (!cachedTotalUIWindows.ContainsKey(key))
                // 등록되지 않은 타입의 인스턴스 요청이므로 종료
                return null;

            // 최종적으로 T타입의 UW 인스턴스는 인스턴스 딕셔너리를 통해 반환된다.
            // 그럼 이 때, 찾고자 하는 인스턴스가 인스턴스 딕셔너리에 존재하는지 검사한다.
            if (!cachedInstances.ContainsKey(key))
            {
                // 이 곳에 들어왔다는 것은, 등록되지 않았다는 뜻
                // 따라서 전체 UW 딕셔너리에서 해당 인스턴스를 가져와 T타입으로 캐스팅 후
                // 인스턴스 딕셔너리에 등록한다.
                cachedInstances.Add(key, (T)Convert.ChangeType(cachedTotalUIWindows[key], typeof(T)));
            }
            // 인스턴스 딕셔너리에 키는 존재함, 이 때 밸류도 존재하는지 검사
            // 만약 밸류가 null이라면 위와 동일하게 전체 UW 딕셔너리에서 인스턴스를 가져와 등록
            else if (cachedInstances[key].Equals(null))
            {
                cachedInstances[key] = (T)Convert.ChangeType(cachedTotalUIWindows[key], typeof(T));
            }

            // 위의 과정을 통해, 최종적으로 인스턴스 딕셔너리에는 T타입 형태의 UW 인스턴스가 저장됨
            // 따라서, 특정 T타입의 인스턴스를 가져오는 기능을 호출 시, 매번 캐스팅하여 반환할 필요 없이
            // 캐싱된 데이터를 사용하므로 어느정도 성능을 절약할 수 있다.
            //  -> 실제 성능상 큰 의미는 없고, 내가 직접 접근한 T타입의 인스턴스만 담긴다는 점에서
            //     인스턴스 접근에 대한 호출 추적 시, 편리한 점이 있음..
            return (T)cachedInstances[key];
        }

        /// <summary>
        /// UWM에 등록된 모든 UW를 닫는 기능
        /// </summary>
        public void CloseAll()
        {
            for (int i = 0; i < totalUIWindows.Count; ++i)
            {
                totalUIWindows[i]?.Close();
            }
        }

        /// <summary>
        /// UWM에 등록된 모든 UW를 초기화하는 기능
        /// </summary>
        public void InitAllWindow()
        {
            for (int i = 0; i < totalUIWindows.Count; ++i)
            {
                totalUIWindows[i]?.InitWindow();
            }
        }

        /// <summary>
        /// 현재 열려있는 UW 중 가장 최상위(가장 마지막에 열린) UW 인스턴스를 반환하는 기능
        ///  -> 모바일 디바이스 등에서 유용하게 사용 가능 
        ///  (모바일 기기에선 뒤로가기 버튼을 누를 시, 일반적으로 가장 최근에 활성화된 UI가 비활성화 되므로
        ///   이러한 경우, 가장 최근에 활성화된 UI 객체를 편리하게 얻을 수 있다)
        /// </summary>
        /// <returns></returns>
        public UIWindow GetTopWindow()
        {
            // 전체 활성 UW 목록에는 활성화된 순서대로 UW 객체가 추가되므로
            // 목록의 가장 뒤쪽에서부터 인스턴스가 존재하는지 확인 후, 존재한다면 반환 한다면
            // 결과적으로 가장 최근에 활성화한 UW 객체가 반환됨
            for (int i = totalOpenWindows.Count - 1; i > 0; --i)
            {
                if (totalOpenWindows[i] != null)
                    return totalOpenWindows[i];
            }

            return null;
        }

    } 
}