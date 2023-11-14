using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.UI
{
    // RequireComponent - 특정 컴포넌트를 가지도록 강제하는 기능
    //  -> 결과적으로 해당 스크립트를 추가 시, 강제로 캔버스 그룹도 추가된다.
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// 모든 UI의 베이스 클래스 (조그만한 팝업, 큰 UI 내의 UI 요소들을 제외한)
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        /// <summary>
        /// 캔버스그룹 참조를 담을 필드
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
        /// 해당 UI를 esc 키로 닫을 수 있게 설정할건지?
        /// </summary>
        public bool canCloseESC;
        /// <summary>
        /// UI의 활성화 상태
        /// </summary>
        public bool isOpen;

        public virtual void Start()
        {
            InitWindow();
        }

        /// <summary>
        /// UI 초기화 기능
        /// </summary>
        public virtual void InitWindow()
        {
            // UWM에 등록
            UIWindowManager.Instance.AddTotalWindow(this);

            // UIWindow를 기반으로 파생된 모든 UI는 초기에 비활성화된 상태로 
            // 배치될 UI 라고 하더라도, 전부 활성화된 상태로 배치할 것이다.
            //  -> 이유?, 비활성화된 상태에서는 초기의 Start 콜백이 정상적으로 동작하지않으므로
            // 따라서, Start 콜백이 실행될 수 있게 전부 활성상태로 씬에 배치해둔 후
            // 콜백에서 InitWindow 메서드를 통해 필드에 미리 설정해둔 isOpen 값에 따라
            // 비활성화되어야할 UI들은 자동적으로 비활성화될 예정
            if (isOpen)
                Open(true);
            else
                Close(true);
        }

        /// <summary>
        /// UI 활성화 기능
        /// </summary>
        /// <param name="force">강제로 활성화 시킬건지?</param>
        public virtual void Open(bool force = false)
        {
            if (!isOpen || force)
            {
                isOpen = true;
                // UWM에 활성 UW 목록에 등록
                UIWindowManager.Instance.AddOpenWindow(this);
                SetCanvasGroup(true);
            }
        }

        /// <summary>
        /// UI 비활성화 기능
        /// </summary>
        /// <param name="force">강제로 비활성화 시킬건지?</param>
        public virtual void Close(bool force = false)
        {
            if (isOpen || force)
            {
                isOpen = false;
                // UWM에 활성 UW 목록에서 제거
                UIWindowManager.Instance.RemoveOpenWindow(this);
                SetCanvasGroup(false);
            }
        }

        /// <summary>
        /// 캔버스 그룹 내 필드를 UI 활성/비활성 여부에 따라 설정하는 기능
        /// </summary>
        /// <param name="isActive">UI 활성/비활성 여부</param>
        private void SetCanvasGroup(bool isActive)
        {
            // 해당 UI 요소 출력 시, 알파 값을 나타냄 (값은 0부터 1까지)
            // 0 이라면 투명, 1이라면 불투명, 이 때 알파가 0이되면 해당 객체의
            // 하이라키 상의 자식 UI들도 모두 투명하게 변경된다.
            CachedCanvasGroup.alpha = Convert.ToInt32(isActive);
            // 캔버스 그룹이 달린 객체의 자식 UI까지의 인터렉션을 제어한다.
            CachedCanvasGroup.interactable = isActive;
            // 캔버스 그룹이 다린 객체의 자식 UI까지의 UI 레이캐스팅 대상에
            // 포함/미포함 시킬 것인지를 제어한다.
            CachedCanvasGroup.blocksRaycasts = isActive;
        }
    }
}