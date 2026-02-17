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

        private const string northButtonPS = "Δ";
        private const string eastButtonPS = "O";
        private const string southButtonPS = "X";
        private const string westButtonPS = "□";

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
                case EControlScheme.PlayStation:
                    return GetPlayStationButton(_buttonToPress);
                case EControlScheme.Xbox:
                default:
                    return GetXboxButton(_buttonToPress);
            }
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
        
        private string GetPlayStationButton(EButton _buttonToPress)
        {
            switch (_buttonToPress)
            {
                case EButton.NorthButton:
                    return northButtonPS;
                case EButton.EastButton:
                    return eastButtonPS;
                case EButton.SouthButton:
                    return southButtonPS;
                case EButton.WestButton:
                    return westButtonPS;
            }

            return "?";
        }
        
        private string GetXboxButton(EButton _buttonToPress)
        {
            switch (_buttonToPress)
            {
                case EButton.NorthButton:
                    return northButtonXbox;
                case EButton.EastButton:
                    return eastButtonXbox;
                case EButton.SouthButton:
                    return southButtonXbox;
                case EButton.WestButton:
                    return westButtonXbox;
            }

            return "?";
        }
    }
}