using System.Collections.Generic;

namespace AI_Project.Controller
{
    public class InputController
    {
        /// <summary>
        /// 버튼 타입의 키 입력 시 실행할 메서드를 대리할 델리게이트 정의
        /// </summary>
        public delegate void InputButtonDel();
        /// <summary>
        /// 축 타입의 키 입력시 실행할 메서드를 대리할 델리게이트 정의
        /// </summary>
        /// <param name="value">축 값</param>
        public delegate void InputAxisDel(float value);

        public List<KeyValuePair<string, AxisHandler>> inputAxes = new List<KeyValuePair<string, AxisHandler>>();
        public List<KeyValuePair<string, ButtonHandler>> inputButtons = new List<KeyValuePair<string, ButtonHandler>>();

        public void AddAxis(string key, InputAxisDel action)
        {
            inputAxes.Add(new KeyValuePair<string, AxisHandler>(key, new AxisHandler(action)));
        }

        public void AddButton(string key, params InputButtonDel[] actions)
        {
            inputButtons.Add(new KeyValuePair<string, ButtonHandler>(key, new ButtonHandler(actions[0], actions[1], actions[2], actions[3])));
        }

        public class AxisHandler
        {
            private InputAxisDel axisEvent;

            public AxisHandler(InputAxisDel axisEvent)
            {
                this.axisEvent = axisEvent;
            }

            public void GetAxisValue(float value)
            {
                axisEvent.Invoke(value);
            }
        }

        public class ButtonHandler
        {
            private InputButtonDel downEvent; // 누른 순간 (1번)
            private InputButtonDel upEvent; // 뗀 순간 (1번)
            private InputButtonDel pressEvent; // 누르고 있는 상태 (여러번)
            private InputButtonDel notPressEvent; // 떼고 있는 상태 (여러번)

            public ButtonHandler(InputButtonDel downEvent, InputButtonDel upEvent, InputButtonDel pressEvent, InputButtonDel notPressEvent)
            {
                this.downEvent = downEvent;
                this.upEvent = upEvent;
                this.pressEvent = pressEvent;
                this.notPressEvent = notPressEvent;
            }

            public void OnDown()
            {
                downEvent?.Invoke();
            }

            public void OnUp()
            {
                upEvent?.Invoke();
            }

            public void OnPress()
            {
                pressEvent?.Invoke();
            }

            public void OnNotPress()
            {
                notPressEvent?.Invoke();
            }
        }
    }
}
