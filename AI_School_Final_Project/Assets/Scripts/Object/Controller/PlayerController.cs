using AI_Project.Controller;
using AI_Project.DB;
using AI_Project.Dummy;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Project.Object
{
    using Input = Define.Input;

    /// <summary>
    /// 플레이어 캐릭터의 입력처리
    /// 캐릭터 클래스에서 처리 안하는 이유?
    /// 캐릭터와 플레이어 입력을 분리함으로써
    /// 캐릭터 클래스를 더 다양하게 사용할 수 있기 때문에 (ex : 멀티 환경에서의 캐릭터 제어 등)
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private bool canRot;
        /// <summary>
        /// 현재 플레이어가 타겟을 마우스로 가리키는 중인지?
        /// </summary>
        public bool HasPointTarget { get; private set; }

        private InputController inputController;

        public CameraController cameraController;
        public Character PlayerCharacter { get; private set; }

        /// <summary>
        /// 초기화 시, 플레이어 캐릭터의 참조를 주입받는다
        /// </summary>
        /// <param name="character"></param>
        public void Initialize(Character character)
        {
            // 플레이어 캐릭터의 하이라키 상의 부모를 플레이어 컨트롤러로 지정
            character.transform.SetParent(transform);
            // 플레이어 캐릭터의 태그를 Player 태그로 지정
            // (다른 캐릭터가 존재한다고 가정했을 때, 편리하게 플레이어의 캐릭터를 구분하기 위함)
            character.gameObject.tag = "Player";
            // 플레이어 캐릭터의 레이어를 Character 레이어로 지정
            // TODO : Character 레이어를 아직 안 만들었음으로 에디터에서 나중에 만들어야 됨
            character.gameObject.layer = LayerMask.NameToLayer("Character");

            // 플레이어 캐릭터 참조 전달
            PlayerCharacter = character;
            // 카메라 추적 타겟을 플레이어 캐릭터로 지정
            cameraController.SetTarget(PlayerCharacter.transform);

            // 입력 컨트롤러 객체 생성
            inputController = new InputController();

            // 축 타입 키 등록
            inputController.AddAxis(Input.AxisX, new InputController.InputAxisDel(GetAxisX)); // 현재 사용안함
            inputController.AddAxis(Input.AxisZ, new InputController.InputAxisDel(GetAxisZ)); // 캐릭터 앞, 뒤 이동
            inputController.AddAxis(Input.MouseX, new InputController.InputAxisDel(GetMouseX)); // 캐릭터 좌,우 회전에 사용 (카메라 회전)
            inputController.AddAxis(Input.MouseY, new InputController.InputAxisDel(GetMouseY)); // 현재 사용안함

            // 버튼 타입 키 등록
            inputController.AddButton(Input.MouseLeft, OnDownMouseLeft, null, null, null);
            inputController.AddButton(Input.MouseRight, null, null, OnPressMouseRight, OnNotPressMouseRight);
            inputController.AddButton(Input.Jump, OnDownJump, null, null, null);
        }

        private void FixedUpdate()
        {
            // 플레이어 캐릭터가 세팅되기 전에는 입력할 수 없도록
            if (PlayerCharacter == null)
                return;

            // 플레이어가 죽었다면 입력할 수 없도록
            if (PlayerCharacter.State == Define.Actor.State.Dead)
                return;

            InputUpdate();
            CheckMousePointTarget();
        }

        private void InputUpdate()
        {
            // 축 타입의 키 체크
            foreach (var inputAxis in inputController.inputAxes)
            {
                inputAxis.Value
                    .GetAxisValue(UnityEngine.Input.GetAxisRaw(inputAxis.Key));
            }

            // 버튼 타입의 키 체크
            foreach (var inputButton in inputController.inputButtons)
            {
                if (UnityEngine.Input.GetButtonDown(inputButton.Key))
                {
                    inputButton.Value.OnDown();
                }
                else if (UnityEngine.Input.GetButton(inputButton.Key))
                {
                    inputButton.Value.OnPress();
                }
                else if (UnityEngine.Input.GetButtonUp(inputButton.Key))
                {
                    inputButton.Value.OnUp();
                }
                else
                { 
                    inputButton.Value.OnNotPress();
                }
            }
        }

        /// <summary>
        /// 플레이어가 현재 마우스로 타겟을 가리키고 있는지 체크하는 기능
        /// </summary>
        private void CheckMousePointTarget()
        {
            // 현재 씬에서 사용하는 카메라에서 스크린 좌표계의 마우스 위치로의 레이 생성
            var ray = CameraController.Cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            // 생성한 레이를 통해 해당 레이 방향에 몬스터가 존재하는지 체크
            var hits = Physics.RaycastAll(ray, 200f, 1 << LayerMask.NameToLayer("Monster"));

            // 레이캐스팅 결과가 담긴 배열의 길이가 0이 아니라면 타겟 존재
            HasPointTarget = hits.Length != 0;
        }

        private void OnApplicationQuit()
        {
            var dtoStage = new DtoStage();
            dtoStage.index = GameManager.User.boStage.sdStage.index;

            var playerPos = PlayerCharacter.transform.position;
            dtoStage.posX = playerPos.x;
            dtoStage.posY = playerPos.y;
            dtoStage.posZ = playerPos.z;

            DummyServer.Instance.userData.dtoStage = dtoStage;
            DummyServer.Instance.Save();
        }


        #region 입력 구현부
        private void GetAxisX(float value)
        { 
        
        }

        private void GetAxisZ(float value)
        {
            PlayerCharacter.boActor.moveDir.z = value;
        }

        private void GetMouseX(float value)
        {
            PlayerCharacter.boActor.rotDir.y = canRot ? value : 0;        
        }

        private void GetMouseY(float value)
        { 
        
        }

        private void OnDownMouseLeft()
        {
            if (PlayerCharacter.attackController.CanAttack(true))
                PlayerCharacter.SetState(Define.Actor.State.Attack);
        }

        private void OnPressMouseRight()
        {
            canRot = true;
        }

        private void OnNotPressMouseRight()
        {
            canRot = false;
        }

        private void OnDownJump()
        {
            // 이미 공중이라면 점프할 수 없게 리턴
            if (!PlayerCharacter.boActor.isGround)
                return;

            PlayerCharacter.SetState(Define.Actor.State.Jump);
        }
        #endregion
    }
}
