using UnityEngine;
using UnityEngine.InputSystem;
using enums;

namespace FishingGame.QuickTimeEvents
{
    public class QTEController : MonoBehaviour
    {
        [SerializeField] private TimingQTE timingQTE;
        [SerializeField] private ButtonMashQTE buttonMashQTE;
        [SerializeField] private BarQTE barQTE;

        [HideInInspector] public EQuickTimeEvent CurrentQuickTimeEvent = EQuickTimeEvent.None;

        private EButton currentButtonToPress;
        private float moveInput;

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

        public void StopCurrentQTE()
        {
            switch (CurrentQuickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    timingQTE.StopQTE();
                    break;
                case EQuickTimeEvent.ButtonMash:
                    break;
                case EQuickTimeEvent.Bar:
                    break;
                default:
                    break;
            }
        }
    }
}