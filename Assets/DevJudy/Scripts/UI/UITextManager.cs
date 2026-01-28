using System.Collections;
using UnityEngine;
using TMPro;

namespace UIScripts
{
    public class UITextManager : MonoBehaviour
    {
        // --- Update Text

        [Header("Points: ")]
        [SerializeField] private TextMeshProUGUI pointsText;

        private int currentScore = 0;

        [Header("Timer: ")]
        [SerializeField] private TextMeshProUGUI timerText;

        public void UpdatePointsText(int _addedPoints)
        {
            currentScore += _addedPoints;

            if (pointsText != null)
                pointsText.text = "Points: " + currentScore.ToString();
            else
                Debug.LogWarning("No points text set");
        }
        
        public void UpdateTimerText(string _timerText)
        {
            timerText.text = _timerText;
        }
        
        public IEnumerator TimeDeductionFeedback(float _deductionFeedbackDuration)
        {
            timerText.color = Color.red;

            yield return new WaitForSeconds(_deductionFeedbackDuration);

            timerText.color = Color.white;

            yield return null;
        }
    }

}