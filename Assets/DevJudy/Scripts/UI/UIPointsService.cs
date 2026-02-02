using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class UIPointsService : MonoBehaviour
    {
        [Header("Points: ")]
        [SerializeField] private TextMeshProUGUI pointsText;

        // !! This can be updated in the playerScore directly instead of only in UIPointsService
        public int CurrentScore {get; private set;}
        
        public void UpdatePointsText(int _addedPoints)
        {
            CurrentScore += _addedPoints;

            if (pointsText != null)
                pointsText.text = "Points: " + CurrentScore.ToString();
            else
                Debug.LogWarning("No points text set");
        }
    }
}
