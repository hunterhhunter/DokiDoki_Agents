using Newtonsoft.Json;
using System;
using UnityEngine;

namespace AI_Project.Network
{
    /// <summary>
    /// 통신에 사용되는 모든 데이터셋의 베이스 클래스
    ///  -> 통신에 사용되는 모든 데이터들이 공통으로 갖는 데이터가 정의됨
    /// </summary>
    [Serializable]
    public class DtoBase
    {
        // 통신에 사용되는 데이터이므로 DB상에서 보일 필요가 없음
        // 따라서 인스펙터에 노출되지 않도록 숨긴다
        /// <summary>
        /// 통신 결과에 대한 에러코드
        /// </summary>
        [HideInInspector]
        public int errorCode;
        /// <summary>
        /// 에러에 대한 내용
        /// </summary>
        [HideInInspector]
        public string errorMessage;
    }

    /// <summary>
    /// 서버 통신 후 받는 데이터에 대한 응답 처리를 일반화하여 수행할 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseHandler<T> where T : DtoBase
    {
        /// <summary>
        /// 요청 성공 시 호출될 메서드를 담을 델리게이트
        /// </summary>
        /// <param name="result">요청해서 받은 데이터 셋</param>
        public delegate void OnSuccess(T result);

        /// <summary>
        /// 요청 실패 시 호출될 메서드를 담을 델리게이트
        /// </summary>
        /// <param name="erro">에러코드와 에러메세지</param>
        public delegate void OnFailed(DtoBase erro);

        protected OnSuccess success;
        protected OnFailed failed;

        /// <summary>
        /// ResponseHandler 객체 생성 시, 요청 성공/실패 시에 실행시킬 메서드를 받아온다
        /// </summary>
        /// <param name="success"></param>
        /// <param name="failed"></param>
        public ResponseHandler(OnSuccess success, OnFailed failed)
        {
            this.success = success;
            this.failed = failed;
        }

        /// <summary>
        /// 서버에 데이터 요청 성공에 따른 모든 dto 데이터에 대한 공통 처리
        /// </summary>
        /// <param name="response">요청한 데이터를 갖는 json</param>
        public void HandleSuccess(string response)
        {
            T data = null;

            // 서버에서 받은 json이 존재한다면
            if (response != null)
            {
                // json을 내가 다루고자 하는 T타입으로 변환
                data = JsonConvert.DeserializeObject<T>(response);

                // 에러코드가 존재하는지 체크
                if (CheckFail(data))
                {
                    // 에러가 존재하므로 실패 처리
                    failed?.Invoke(data);
                    return;
                }

                // 요청 성공 시 실행할 메서드가 존재한다면 실행
                success?.Invoke(data);
            }
        }

        /// <summary>
        /// 서버에 데이터 요청 실패 시 에러 처리
        ///  -> 현재 프로젝트에서는 더미서버만 사용하므로 사실상 호출할 일은 없음
        /// </summary>
        /// <param name="response"></param>
        public void HandleFailed(string response)
        {
            // 서버에서 받은 json 데이터를 에러코드,메세지를 갖는 데이터셋으로 변환
            DtoBase data = JsonConvert.DeserializeObject<T>(response);

            // 요청 실패 시 실행시킬 기능이 있다면 실행
            failed?.Invoke(data);
        }

        /// <summary>
        /// 서버에 데이터 용청 시 응답 에러코드를 통한 에러 체크
        /// </summary>
        /// <param name="dtoBase"></param>
        /// <returns></returns>
        private bool CheckFail(T dtoBase)
        {
            // 에러코드가 0 초과라면 에러가 존재한다고 가정
            return dtoBase.errorCode > 0;
        }
    }
}
