using System.Collections;
using UnityEngine;
using TMPro;

namespace UIScripts
{
    public class UITimerManager : MonoBehaviour
    {
        [Header("Timer: ")]
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private float deductionFeedbackDuration = 1f;
        [SerializeField] private Color timeDeductionColor;
        
        public void UpdateTimerText(string _timerText)
        {
            timerText.text = _timerText;
        }

        public void UpdateTimerPenaltyText(int _minutesPenalty, int _secondsPenalty)
        {
            timerText.text = $"{_minutesPenalty:00}:{_secondsPenalty:00}:{00:00}";
        }

        public IEnumerator TimeDeductionFeedback()
        {
            timerText.color = timeDeductionColor;

            yield return new WaitForSeconds(deductionFeedbackDuration);

            timerText.color = Color.white;

            yield return null;
        }
    }
}