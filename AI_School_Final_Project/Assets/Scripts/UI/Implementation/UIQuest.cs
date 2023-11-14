using AI_Project.DB;
using AI_Project.Network;
using AI_Project.SD;
using AI_Project.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AI_Project.Define.UI;

namespace AI_Project.UI
{
    public class UIQuest : UIWindow
    {
        /// <summary>
        /// 현재 UI 퀘스트 창의 모드를 나타내는 필드
        /// </summary>
        public QuestWindow currentWindow;
        /// <summary>
        /// 현재 창의 모드가 리스트일 때, 리스트 내에서 어떤 탭을
        /// 사용중인지를 나타내는 필드
        /// </summary>
        public QuestTab currentTab;

        // Quest List 창 관련 참조
        public Button progressTab; // 진행탭
        public Button completedTab; // 완료탭
        public Transform contentHolder; // 스크롤뷰의 콘텐츠 홀더
        public Transform listWindow; // QuestList 객체 트랜스폼 참조
        // 풀에서 가져온 슬롯을 담을 목록
        private List<QuestSlot> slots = new List<QuestSlot>();

        // Quest Order 창 관련 참조
        public Button refuse; // 거절 버튼
        public Button accept; // 수락 버튼
        public TextMeshProUGUI orderTitle; // 수주받은 퀘스트 이름
        public TextMeshProUGUI orderDesc; // 수주받은 퀘스트 내용
        public Transform orderWindow; // QuestOrder 객체 트랜스폼 참조

        public override void Start()
        {
            base.Start();

            // 수주창에서 거절 버튼 클릭 시 이벤트 바인딩
            refuse.onClick.AddListener(() => { Close(); });
            // 리스트창에서 진행 탭 클릭 시 이벤트 바인딩
            progressTab.onClick.AddListener(() => { OnClickTab(QuestTab.Progress); });
            // 리스트창에서 완료 탭 클릭 시 이벤트 바인딩
            completedTab.onClick.AddListener(() => { OnClickTab(QuestTab.Completed); });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                if (isOpen)
                    Close();
                else
                    Open(QuestWindow.List);
            }
        }

        /// <summary>
        /// 퀘스트 창을 오픈하는 기능
        /// </summary>
        /// <param name="questWindow">열고자하는 퀘스트 창의 모드</param>
        /// <param name="orderQuest">열고자 하는 퀘스트창이 오더창 일 때, 오더 퀘스트에 대한 기획데이터</param>
        public void Open(QuestWindow questWindow, SDQuest orderQuest = null)
        {
            // 이미 켜져있다면 종료
            if (isOpen)
                return;
            
            // 현재 창 모드를 파라미터로 전달받은 창 모드로 설정
            currentWindow = questWindow;

            var isListWindow = currentWindow == QuestWindow.List;

            // 창 모드가 리스트 창이라면?
            if (isListWindow)
            {
                // 창을 리스트 창에 맞게 설정
                SetListWindow();
            }
            // 창 모드가 오더 창이라면?
            else
            {
                // 창을 오더창에 맞게 설정
                SetOrderWindow(orderQuest);
            }

            // 창 모드에 따라 각종 창을 활성/비활성화
            listWindow.gameObject.SetActive(isListWindow);
            orderWindow.gameObject.SetActive(!isListWindow);

            // 베이스에 있던 오픈 기능을 호출
            Open();
        }

        /// <summary>
        /// 퀘스트 창을 리스트 창에 맞게 설정하는 기능
        /// </summary>
        private void SetListWindow()
        {
            // 퀘스트 슬롯이 담긴 풀을 가져옴
            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>();

            // 현재 탭 타입에 따라 처리
            switch (currentTab)
            {
                case QuestTab.Progress:
                    // 유저의 진행 퀘스트 정보를 가져옴
                    var boProgressQuest = GameManager.User.boQuest.progressQuests;

                    // 진행퀘스트 개수만큼 슬롯 세팅
                    for (int i = 0; i < boProgressQuest.Count; ++i)
                        SetSlots(boProgressQuest[i].sdQuest, boProgressQuest[i].details);
                    break;
                case QuestTab.Completed:
                    // 유저의 완료 퀘스트 정보를 가져옴
                    var boCompletedQuest = GameManager.User.boQuest.completedQuests;

                    for (int i = 0; i < boCompletedQuest.Count; ++i)
                        SetSlots(boCompletedQuest[i]);
                    break;
            }

            // 탭 타입에 따른 처리가 중복되므로 로컬 함수로 작성
            // 2번째 파라미터는 진행탭일 때만 사용
            void SetSlots(SDQuest sdQuest, params int[] details)
            {
                // 풀에서 퀘스트 슬롯을 하나 가져옴
                var questSlot = pool.GetObj();
                // 가져온 슬롯을 초기화
                questSlot.Initialize(sdQuest, details);

                // 퀘스트 슬롯을 스크롤 뷰 형식으로 출력할 것이므로
                // 퀘스트 슬롯의 부모를 스크롤뷰의 컨텐츠 홀더로 지정
                questSlot.transform.SetParent(contentHolder);
                questSlot.transform.localScale = Vector3.one;
                slots.Add(questSlot);
                questSlot.gameObject.SetActive(true);
            }

        }

        /// <summary>
        /// 퀘스트 창을 오더 창에 맞게 설정하는 기능
        /// </summary>
        /// <param name="sdQuest">오더 퀘스트에 대한 기획데이터</param>
        private void SetOrderWindow(SDQuest sdQuest)
        {
            // 수주 퀘스트 이름 설정
            orderTitle.text = sdQuest.name;
            // 수주 퀘스트 내용 설정
            orderDesc.text = GameManager.SD.sdStrings.Where(_ => _.index == sdQuest.description).SingleOrDefault().kr;

            // 수락 버튼 이벤트 바인딩
            // 수락 버튼을 누를 때 마다, 실행 시킬 기능 자체는 똑같은데
            // 전달되는 데이터가 달라짐
            // 수락버튼을 눌렀을 때, 수락한 퀘스트를 유저 DB의 퀘스트에 추가하는 행위는
            // 같은데, 어떤 퀘스트인지? 데이터가 달라짐

            // 수락 버튼에 이전에 바인딩된 이벤트를 전부 지운다.
            accept.onClick.RemoveAllListeners();

            // 수주 퀘스트에 대한 정보를 서버에 넘기면서 db에 추가해달라는 요청을 하는 기능
            // 을 수락 버튼에 바인딩한다.
            accept.onClick.AddListener(() => {

                ServerManager.Server.AddQuest(0, sdQuest.index, 
                    new ResponseHandler<DB.DtoQuestProgress>(dtoQuestProgress => {

                        var boQuestProgress = new BoQuestProgress(dtoQuestProgress);
                        GameManager.User.boQuest.progressQuests.Add(boQuestProgress);
                        Close();
                    }, failed => { }));
            });

            //accept.onClick.AddListener(AddQuest);

            //void AddQuest()
            //{
            //    ServerManager.Server.AddQuest(0, sdQuest.index, 
            //        new ResponseHandler<DtoQuestProgress>(AddQuestSuccess, AddQuestFailed));
            //}

            //void AddQuestSuccess(DtoQuestProgress dtoQuestProgress)
            //{
            //    var boQuestProgress = new BoQuestProgress(dtoQuestProgress);
            //    GameManager.User.boQuest.progressQuests.Add(boQuestProgress);
            //    Close();
            //}

            //void AddQuestFailed(DtoBase dtoError)
            //{ 
            
            //}
        }

        /// <summary>
        /// 리스트 창에서 탭 버튼을 클릭 시 실행시킬 기능
        /// </summary>
        /// <param name="tab">누른 탭</param>
        private void OnClickTab(QuestTab tab)
        {
            // 탭이 변경되었는지 확인
            var isOtherTab = currentTab != tab;

            currentTab = tab;

            if (isOtherTab)
            {
                // 기존 퀘스트 슬롯 비우고
                ClearSlot();
                // 바뀐 탭에 맞게 슬롯을 새로 세팅
                SetListWindow();
            }
        }

        public override void Close(bool force = false)
        {
            base.Close(force);

            ClearSlot();
        }

        /// <summary>
        /// 풀에서 가져온 슬롯들이 담긴 슬롯 목록을 비우는 기능
        /// </summary>
        private void ClearSlot()
        {
            if (slots.Count == 0)
                return;

            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>();
            for (int i = 0; i < slots.Count; ++i)
            {
                pool.Return(slots[i]);
            }
            slots.Clear();
        }

    }
}
