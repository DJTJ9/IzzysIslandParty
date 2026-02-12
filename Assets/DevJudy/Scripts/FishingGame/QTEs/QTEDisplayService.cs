using enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame.QuickTimeEvents
{
    public class QTEDisplayService : MonoBehaviour
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

        private const string northButtonXbox = "Y";
        private const string eastButtonXbox = "B";
        private const string southButtonXbox = "A";
        private const string westButtonXbox = "X";

        #endregion

        [SerializeField] private InputAction ia;
        
        public EControlScheme controlScheme;

        public string DisplayButtonToPress(EButton _buttonToPress)
        {
            switch (controlScheme)
            {
                case EControlScheme.Keyboard:
                    return GetKeyboardButton(_buttonToPress);
            }

            // This should never return anything but a valid button
            return "!";
        }

        private string GetKeyboardButton(EButton _buttonToPress)
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

            return "?";
        }
        
        // // !! Beim joinen holen
       // var device = _context.control.device;

       //     if (device is Gamepad gamepad)
       // {
       //     if (gamepad is DualShockGamepad dualShockGamepad)
       //         Debug.Log("PlayStation controller");
       //     else if (gamepad is XInputController xInputController)
       //         Debug.Log("Xbox controller");
       //         
       // }
       // else if (device is Keyboard keyboard)
       // Debug.Log("Keyboard controller");
    }
}