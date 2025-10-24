using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace HollowKnightLike.UI
{
    [AddComponentMenu("HollowKnightLike/UI/Mobile On Screen Button")]
    public class MobileOnScreenButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [InputControl(layout = "Button")]
        [SerializeField] private string controlPath;

        [SerializeField] private float pressedValue = 1f;

        protected override string controlPathInternal => controlPath;

        public void SetControlPath(string path)
        {
            bool wasEnabled = isActiveAndEnabled;
            if (wasEnabled)
            {
                enabled = false;
            }

            controlPath = path;

            if (wasEnabled)
            {
                enabled = true;
            }
        }

        public void SetPressedValue(float value)
        {
            pressedValue = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SendValueToControl(pressedValue);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(0f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SendValueToControl(0f);
        }

        protected override void OnDisable()
        {
            SendValueToControl(0f);
            base.OnDisable();
        }
    }
}
