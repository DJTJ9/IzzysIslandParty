using UnityEngine;
using UnityEngine.InputSystem;
using enums;

namespace FishingGame.QuickTimeEvents
{
    public class QTEController : MonoBehaviour
    {
        private TimingQTE timingQTE;
        private ButtonMashQTE buttonMashQTE;
        private BarQTE barQTE;

        [HideInInspector] public EQuickTimeEvent CurrentQuickTimeEvent = EQuickTimeEvent.None;

        private EButton currentButtonToPress;
        private float moveInput;

        private void Awake()
        {
            timingQTE = GetComponentInChildren<TimingQTE>();
            if (timingQTE == null)
                Debug.LogError("Timing QTE not found in children");
            
            buttonMashQTE = GetComponentInChildren<ButtonMashQTE>();
            if (buttonMashQTE == null)
                Debug.LogError("ButtonMashQTE not found in children");
            
            barQTE = GetComponentInChildren<BarQTE>();
            if (barQTE == null)
                Debug.LogError("BarQTE not found in children");
        }

        public void OnNorthButtonPressed(InputAction.CallbackContext _context)
        {
            if (_context.performed && CurrentQuickTimeEvent != EQuickTimeEvent.None)
            {
                switch (CurrentQuickTimeEvent)
                {
                    case EQuickTimeEvent.Timing:
                        timingQTE.CheckTimingSuccess(EButton.NorthButton);
                        break;
                    case EQuickTimeEvent.ButtonMash:
                        buttonMashQTE.CheckButtonMashSuccess(EButton.NorthButton);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnEastButtonPressed(InputAction.CallbackContext _context)
        {
            if (_context.performed && CurrentQuickTimeEvent != EQuickTimeEvent.None)
            {
                switch (CurrentQuickTimeEvent)
                {
                    case EQuickTimeEvent.Timing:
                        timingQTE.CheckTimingSuccess(EButton.EastButton);
                        break;
                    case EQuickTimeEvent.ButtonMash:
                        buttonMashQTE.CheckButtonMashSuccess(EButton.EastButton);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnSouthButtonPressed(InputAction.CallbackContext _context)
        {
            if (_context.performed && CurrentQuickTimeEvent != EQuickTimeEvent.None)
            {
                switch (CurrentQuickTimeEvent)
                {
                    case EQuickTimeEvent.Timing:
                        timingQTE.CheckTimingSuccess(EButton.SouthButton);
                        break;
                    case EQuickTimeEvent.ButtonMash:
                        buttonMashQTE.CheckButtonMashSuccess(EButton.SouthButton);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnWestButtonPressed(InputAction.CallbackContext _context)
        {
            if (_context.performed && CurrentQuickTimeEvent != EQuickTimeEvent.None)
            {
                switch (CurrentQuickTimeEvent)
                {
                    case EQuickTimeEvent.Timing:
                        timingQTE.CheckTimingSuccess(EButton.WestButton);
                        break;
                    case EQuickTimeEvent.ButtonMash:
                        buttonMashQTE.CheckButtonMashSuccess(EButton.WestButton);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnLeftRightInput(InputAction.CallbackContext _context)
        {
            if (!barQTE.QTERunning)
                return;

            if (_context.started)
            {
                moveInput = _context.ReadValue<float>();

                if (moveInput > 0.02f)
                    moveInput = 1f;
                else if (moveInput < -0.02f)
                    moveInput = -1f;
                else
                    moveInput = 0f;
            }

            if (_context.canceled)
                moveInput = 0f;

            barQTE.SetLeftRightInput(moveInput);
        }

// !! IS THIS LEFTOVER?
        public void StopCurrentQTE()
        {
            switch (CurrentQuickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    timingQTE.StopQTE();
                    break;
                case EQuickTimeEvent.ButtonMash:
                    buttonMashQTE.StopQTE();
                    break;
                case EQuickTimeEvent.Bar:
                    barQTE.StopQTE();
                    break;
                default:
                    break;
            }
        }
    }
}