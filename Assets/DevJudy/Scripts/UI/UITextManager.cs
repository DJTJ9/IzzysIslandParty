using System.Collections;
using UnityEngine;
using TMPro;

namespace UIScripts
{
    public class UITextManager : MonoBehaviour
    {
        // --- Update Text

        [SerializeField] private TextMeshProUGUI pointsText;
        private int currentScore = 0;

        public void UpdatePointsText(int _addedPoints)
        {
            currentScore += _addedPoints;

            if (pointsText != null)
                pointsText.text = "Points: " + currentScore.ToString();
            else
                Debug.LogWarning("No points text set");
        }

        // --- UpdateTimer

        // !!Should be its own logic

        [Header("GameObjects: ")]
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private UIPanelManager uiPanelManager;

        [Header("Variables: ")]
        private float time;

        private float minutes;
        private float seconds;
        private float milliseconds;

        [SerializeField] private float durationInMinutes;
        [SerializeField] private float deductionFeedbackDuration = 2f;

        private bool updateTimer = false;
        [SerializeField] private bool timerRunningDown;
        [SerializeField] private bool startTimerOnLevelStart;

        private bool timerFinished;

        public bool TimerFinished
        {
            get => timerFinished;
            private set
            {
                timerFinished = value;

                if (timerFinished)
                    updateTimer = false;
            }
        }

        public bool UpdateTimer
        {
            get => updateTimer;
            set => updateTimer = value;
        }

        private void Start()
        {
            if (startTimerOnLevelStart)
                StartTimer();
        }

        public void StartTimer()
        {
            if (timerRunningDown)
                time = durationInMinutes * 60;
            else
                time = 0;

            updateTimer = true;
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!updateTimer)
                return;

            if (timerRunningDown)
                DisplayRunningDownTimer();
            else
                DisplayRunningTimer();
        }

        public void DeduceTime(float _timeDeduction)
        {
            if (timerRunningDown)
                time -= _timeDeduction;
            else
                time += _timeDeduction;

            StartCoroutine(TimeDeductionFeedback());
        }

        private void DisplayRunningTimer()
        {
            time += Time.fixedDeltaTime;

            TimeToTimerTextFormat(time);
        }

        private void DisplayRunningDownTimer()
        {
            time -= Time.fixedDeltaTime;

            TimeToTimerTextFormat(time);

            if (time <= 0.001f)
            {
                timerFinished = true;
                uiPanelManager.SetGameOver();
            }
        }

        private void TimeToTimerTextFormat(float _time)
        {
            minutes = Mathf.FloorToInt(_time / 60);
            seconds = Mathf.FloorToInt(_time % 60);
            milliseconds = Mathf.Round((_time % 1) * 1000);
            milliseconds = Mathf.RoundToInt((milliseconds) / 10);

            timerText.text = $"Timer: {minutes:00}:{seconds:00}:{milliseconds:00}";
        }

        public float EndTimerAndGetFinishTime(out float _seconds, out float _milliseconds)
        {
            UpdateTimer = false;

            _seconds = seconds;
            _milliseconds = milliseconds;
            return minutes;
        }

        private IEnumerator TimeDeductionFeedback()
        {
            timerText.color = Color.red;

            yield return new WaitForSeconds(deductionFeedbackDuration);

            timerText.color = Color.white;

            yield return null;
        }
    }
}