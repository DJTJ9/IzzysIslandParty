using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class UIPointsService : MonoBehaviour
    {
        [Header("Points: ")]
        [SerializeField] private TextMeshProUGUI pointsText;

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
