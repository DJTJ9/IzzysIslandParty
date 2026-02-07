using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class UIPointsService : MonoBehaviour
    {
        public void UpdatePointsText(TextMeshProUGUI _pointsText, float _currentScore)
        {
            if (_pointsText != null)
                _pointsText.text = "Points: " + _currentScore;
            else
                Debug.LogWarning("No points text set");
        }
        
        
    }
}
