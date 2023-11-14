using AI_Project.Resource;
using UnityEngine;

namespace AI_Project.Object
{
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// 현재 카메라 뷰 상태를 나타내는 필드
        /// </summary>
        public Define.Camera.View camView;
        /// <summary>
        /// 카메라 이동 시, 선형보간을 이용한 이동을 사용할 것임
        /// 선형보간 이동에 사용할 변위 값
        /// </summary>
        public float smooth = 3f;

        /// <summary>
        /// 뒤쪽에서 3인칭으로 캐릭터를 찍을 때 거리,회전 값을 갖는 트랜스폼 객체 참조
        /// </summary>
        private Transform defaultPos;
        /// <summary>
        /// 앞쪽에서 3인칭으로 캐릭터를 찍을 때
        /// </summary>
        private Transform frontPos;
        /// <summary>
        /// 카메라가 추적할 타겟의 트랜스폼 참조(플레이어)
        /// </summary>
        private Transform target;

        /// <summary>
        /// 카메라 컴포넌트를 이용해 서로 다른 좌표계에서 좌표변환을 이용해 연산을 해야할 경우가
        /// 프로젝트 내에서 빈번하게 발생하므로, 처음에 카메라 참조를 한 번 담아둔 다음 편하게
        /// 접근하기 위해 정적 필드를 생성
        /// </summary>
        public static Camera Cam { get; private set; }

        private void Start()
        {
            Cam = GetComponent<Camera>();
        }

        /// <summary>
        /// 카메라의 추적 타겟을 설정하는 기능
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            this.target = target;

            // 이 때 추적하고자 하는 타겟에게 CamPos를 생성하여 하이라키 상의 자식으로 배치한다
            //  -> CamPos에는 디폴트뷰와 프론트뷰를 갖는 자식이 존재하며, 이 때 생성한 CamPos를 
            //     타겟을 기준(로컬)으로 0,0,0에 배치한다면 미리 설정한 디폴트,프론트 뷰에 따른
            //     위치와 회전 값을 갖게 됨
            var camPos = Instantiate(ResourceManager.Instance.LoadObject(Define.Camera.CamPosPath)).transform;

            // CamPos의 부모를 타겟으로 설정
            camPos.SetParent(this.target);
            // 부모를 기준으로 0,0,0에 위치하도록
            camPos.localPosition = Vector3.zero;

            // 디폴트뷰와 프론트뷰 객체 트랜스폼의 참조를 가져온다.
            defaultPos = camPos.GetChild(0);
            frontPos = camPos.GetChild(1);

            // 초기값 설정
            //  카메라의 위치와 방향을 처음엔 디폴트뷰로 설정
            transform.position = defaultPos.position;
            // forward (해당 객체의 앞쪽을 가리키는 벡터(z축, 기즈모 파랑색 화살표))
            transform.forward = defaultPos.forward;
        }

        private void FixedUpdate()
        {
            // 추적 대상이 없다면 리턴
            if (target == null)
                return;

            // 뷰 모드에 따라 이동을 다르게
            switch (camView)
            {
                case Define.Camera.View.Default:
                    SetPosition(true, defaultPos);
                    break;
                case Define.Camera.View.Front:
                    SetPosition(false, frontPos);
                    break;
            }
        }

        /// <summary>
        /// 캐릭터의 위치에 큰 변위가 생겼을 때 사용 (ex: 스테이지 이동)
        /// </summary>
        public void SetForceDefaultView()
        {
            SetPosition(false, defaultPos);
        }

        /// <summary>
        /// 카메라의 이동 및 회전 연산
        /// </summary>
        /// <param name="isLerp">이동 시 보간할 것인지?, 보간하지 않을 시 한 번에 이동</param>
        /// <param name="target">디폴트 뷰 또는 프론트 뷰 둘 중 하나</param>
        private void SetPosition(bool isLerp, Transform target)
        {
            if (isLerp)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.fixedDeltaTime * smooth);
                transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.fixedDeltaTime * smooth);
            }
            else
            {
                transform.position = target.position;
                transform.forward = target.forward;
            }
        }
    }
}
