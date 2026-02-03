using enums;
using UnityEngine;

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

        #endregion
        
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
}
