using UnityEngine;
using UnityEngine.InputSystem;
using enums;
using FishingGame.QuickTimeEvents;

public class QTEController : MonoBehaviour
{
    #region constants

    private const string northButtonKB = "W";
    private const string eastButtonKB = "D";
    private const string southButtonKB = "S";
    private const string westButtonKB = "A";

    private const string northButtonCTRL = "Δ";
    private const string eastButtonCTRL = "O";
    private const string southButtonCTRL = "X";
    private const string westButtonCTRL = "☐";

    #endregion

    [SerializeField] private TimingQTE timingQTE;
    [SerializeField] private ButtonMashQTE buttonMashQTE;
    [SerializeField] private BarQTE barQTE;
    
    [HideInInspector] public EQuickTimeEvent CurrentQuickTimeEvent = EQuickTimeEvent.None;

    private EButton currentButtonToPress;

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

    public string DisplayButtonToPress(EButton _buttonToPress)
    {
        switch (_buttonToPress)
        {
            case EButton.NorthButton:
                return northButtonKB;
            case EButton.EastButton:
                return eastButtonKB;
            case EButton.SouthButton:
                return southButtonKB;
            case EButton.WestButton:
                return westButtonKB;
        }

        // This should never return anything but a valid button
        return "!";
    }
}