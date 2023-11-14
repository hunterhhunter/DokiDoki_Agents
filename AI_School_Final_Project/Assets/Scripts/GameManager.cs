using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI_Project.Util;
using AI_Project.SD;
using AI_Project.Define;
using System;
using UnityEngine.SceneManagement;
using AI_Project.DB;
using AI_Project.UI;

namespace AI_Project
{
    /// <summary>
    /// 게임에 사용하는 모든 데이터를 관리하는 클래스
    /// 추가로 게임의 씬 변경 등과 같은 큰 흐름을 제어함
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// 더미서버를 사용할 것인지에 대한 플래그
        /// </summary>
        public bool useDummyServer;

        /// <summary>
        /// 씬 전환 시, 현재 로딩 상태를 나타내는 필드
        /// 0~1 로 사용
        /// </summary>
        public float loadState;

        // 유저 데이터
        [SerializeField]
        private BoUser boUser = new BoUser();
        public static BoUser User => Instance.boUser;

        // 기획 데이터
        [SerializeField]
        private StaticDataModule sd = new StaticDataModule();
        public static StaticDataModule SD => Instance.sd;

        protected override void Awake()
        {
            base.Awake();

            // 타이틀 씬에서 페이즈 로드를 실행하기 위해
            // 타이틀 컨트롤러 참조를 찾는다.
            var titleController = FindObjectOfType<TitleController>();
            // 참조가 존재한다면? 초기화
            titleController?.Initialize();
        }

        /// <summary>
        /// 앱 기본 설정
        /// </summary>
        public void OnAppSetting()
        {
            // 수직동기화 끄기
            QualitySettings.vSyncCount = 0;
            // 렌더 프레임 60 설정
            Application.targetFrameRate = 60;
            // 앱 실행 중 장시간 대기 시에도 화면이 꺼지지 않도록
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// 씬을 비동기로 로드하는 기능
        /// 다른 씬 간의 전환에 사용 (ex Title -> Ingame)
        /// </summary>
        /// <param name="type">로드할 씬의 이름을 갖는 열거형</param>
        /// <param name="loadProgress">씬 전환 시 로딩 씬에서 미리 처리할 작업</param>
        /// <param name="loadComplete">씬 전환 완료 후 실행할 기능</param>
        public void LoadScene(SceneType type, IEnumerator loadProgress = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            // 씬을 전환할 때, ex) Title -> Ingame 전환 시 한 번에 전환하는 것이 아니라
            // 중간에 로딩 씬을 이용, 최종적으로 Title -> Loading -> Ingame

            // 코루틴 -> 유니티에서 특정 작업을 비동기로 실행할 수 있게 해주는 기능
            //           실제 비동기는 아님, 실행 타임을 다르게하여 비동기처럼 보이게하는 것

            // LoadScene 메서드에선 사용가능한 로컬함수 선언
            IEnumerator WaitForLoad()
            {
                // 로딩 진행 상태를 0으로 초기화
                loadState = 0;

                // 비동기로 현재 씬에서 로딩 씬으로 전환
                //  -> 씬 전환 시, 화면이 멈추지 않게 하기 위해서
                yield return SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

                // 로딩 씬으로 전환 완료 후에 아래 로직이 들어옴

                // 내가 변경하고자 하는 씬을 추가 (현재 게임에 씬이 2개가 된 상태)
                //  -> LoadScene을 씬 변경 시, 2번째 파라미터를 사용하지 않는다면 기본 값이 적용됨
                //     기본 값은 현재 씬(로딩) 비활성화하고 변경하고자 하는 씬을 활성화
                //     하지만, 현재 하고자 하는 작업은 기존 씬을 그대로 두고 새로운 씬을 추가하는 작업
                //     이 때, 씬 로드 방식을 Additive로 설정하면 됨
                var asyncOper = SceneManager.LoadSceneAsync(type.ToString(), LoadSceneMode.Additive);

                // 결과적으로 현재 게임에는 2개의 씬이 활성화된 상태 (로딩 씬, 변경하고자 하는 씬)
                // 따라서, 원치 않게 2개의 씬의 객체들의 모두 렌더되므로, 현재 당장 필요하지 않은
                // 변경하고자 하는 씬을 비활성화 한다.
                //  -> 씬은 그대로 하이라키 상에 존재하는데 비활성화만 시킴

                // 비활성화 시키는 법? 비동기로 씬 로드 시, LoadSceneAsync 메서드가 AsyncOper 객체를 반환함
                // AsyncOper 객체로 현재 비동기로 부르고 있는 씬의 로드 상태 확인, 활성화 설정 등을 할 수 있음
                asyncOper.allowSceneActivation = false;

                // 변경하고자 하는 씬에 필요한 작업이 존재한다면 실행
                if (loadProgress != null)
                {
                    // 해당 작업이 완료될 때까지 대기
                    yield return StartCoroutine(loadProgress);
                }

                // 위의 작업이 완료된 후에 로직이 실행됨
                //  -> 변경하고자 하는 씬에 필요한 작업은 이 시점에 모두 완료됐다는 뜻

                // 로딩바에 진행 상태를 변경하여 유저에게 로딩 상태를 알림
                // 추가로, 변경하고자 하는 씬이 로드가 완료된 상태인지를 확인해야 함
                //  -> 비동기로 로드하였고, 로드 시 yield return 하지 않았기 때문에 어느 시점에 완료될 지 알 수 없음

                // 비동기로 로드한 씬이 로드가 완료되지 않았다면 특정 작업을 반복 
                while (!asyncOper.isDone)
                {
                    // loadState 값을 이용해 유저에게 상태를 알림

                    if (loadState >= 0.9f)
                    {
                        // 90 퍼센트 이상 완료됐다면 강제로 100퍼센트로 만듬
                        //  이유? asyncOper의 progress의 값은 정확하게 1이 들어오지않음
                        loadState = 1f;

                        // 로딩바가 마지막까지 차는 것을 유저에게 보여주기 위해 1초 정도 대기
                        yield return new WaitForSeconds(1f);

                        // 변경하고자 하는 씬을 다시 활성화
                        //  (isDone은 씬이 활성 상태가 아니라면, 로드가 완료되었더라도 true가 되지 않음)
                        asyncOper.allowSceneActivation = true;
                    }
                    else
                    {
                        // asyncOper에 현재 로드 진행 상태를 0부터 1의 값으로 나타내는 프로퍼티가 존재
                        // 해당 값을 loadState에 대입
                        loadState = asyncOper.progress;
                    }

                    // 코루틴 내에서 반복문 사용 시, 로직을 반복문 내의 로직을 한 번 실행 후
                    // 메인 로직을 실행할 수 있게 yield return
                    yield return null;
                }

                // 로딩 씬에서 변경하고자 하는 씬에 필요한 작업을 전부 수행했으므로
                // 로딩씬을 비활성화 시킴
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                // 모든 작업이 완료되었으므로, 추가적으로 실행할 작업이 존재한다면? 실행
                loadComplete?.Invoke();
            }
        }

        /// <summary>
        /// 실제 씬을 변경하는 것이 아닌, 로딩 씬을 추가하여 스테이지 전환에 필요한 작업을 한다.
        /// 인게임 씬에서 스테이지 전환 시 사용
        /// ex) 인게임씬(시작마을) -> 인게임씬(초보자사냥터)
        /// </summary>
        /// <param name="loadProgress"></param>
        /// <param name="loadComplete"></param>
        public void OnAdditiveLoadingScene(IEnumerator loadProgress = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            IEnumerator WaitForLoad()
            {
                loadState = 0;

                // 로딩씬을 비동기로 추가 로드한다.
                var asyncOper = SceneManager.LoadSceneAsync(SceneType.Loading.ToString(), LoadSceneMode.Additive);

                var uiWindowManager = UIWindowManager.Instance;
                UILoading uiLoading = null;
                while (uiLoading == null)
                {
                    // 로딩씬을 비동기로 로드하고 있기 때문에 UILoading에 접근하는 시점에 초기화가 완료된 시점인지 알 수 없다.
                    // 하지만, UILoading이 초기화가 됬다면 UWM에 등록이 된 상태이기 때문에 UWM에 접근하여 
                    // UILoading 인스턴스를 찾을 때까지 대기한다면, 찾았을 때의 시점은 UILoading이 초기화가 완료되었다는 뜻
                    //  -> 결론은 UILoading 초기화를 기다림
                    uiLoading = uiWindowManager.GetWindow<UILoading>();
                    yield return null;
                }

                // 스테이지 전환 시에는, 2개의 씬이 동시에 활성화된 상태 따라서 UILoading에 있는 카메라를 비활성화
                //  -> 인게임씬도 활성화된 상태로 카메라가 존재하기 때문에
                uiLoading.cam.enabled = false;

                // 추가로 현재 스테이지 전환 시의 로딩게이지는 실제 로드 작업에 관한 퍼센테이지를 나타내지않음
                //  -> 단순한 출력 용도

                loadState = .3f;

                // 스테이지 전환 시 필요한 작업이 있다면 진행
                if (loadProgress != null)
                    yield return StartCoroutine(loadProgress);

                loadState = .8f;

                yield return new WaitForSeconds(.5f);
                
                loadState = 1f;

                yield return new WaitForSeconds(.5f);

                // 위의 작업이 완료되었다면 아래 로직 실행
                // 전환 완료 후, 실행할 작업이 존재한다면 진행
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                loadComplete?.Invoke();
            }
        }

    }
}