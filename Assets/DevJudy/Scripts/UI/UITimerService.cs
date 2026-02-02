using System.Collections;
using UnityEngine;
using TMPro;

namespace UIScripts
{
    public class UITimerManager : MonoBehaviour
    {
        [Header("Timer: ")]
        [SerializeField] private TextMeshProUGUI timerText;
        
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