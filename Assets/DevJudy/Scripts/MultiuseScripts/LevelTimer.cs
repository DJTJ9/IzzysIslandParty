using UIScripts;
using UnityEngine;

namespace MultiuseScripts
{
    public class LevelTimer : MonoBehaviour
    {
        private static LevelTimer instance;
        public static LevelTimer Instance => instance;
        
        [Header("Dependencies: ")]
        [SerializeField] private UITextManager textManager;
        [SerializeField] private UIPanelManager uiPanelManager;

        [Header("Variables: ")]
        [SerializeField] private float durationInMinutes;

        [SerializeField] private float deductionFeedbackDuration = 2f;
        [SerializeField] private bool timerRunningDown;
        [SerializeField] private bool startTimerOnLevelStart;

        private float time;
        private float minutes;
        private float seconds;
        private float milliseconds;

        private bool updateTimer = false;
        public bool UpdateTimer
        {
            get => updateTimer;
            set => updateTimer = value;
        }

        private bool timerFinished = false;
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

        private LevelTimer()
        {
            instance = this;
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

            StartCoroutine(textManager.TimeDeductionFeedback(deductionFeedbackDuration));
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

            textManager.UpdateTimerText($"Time: {minutes:00}:{seconds:00}:{milliseconds:00}");
        }

        public string GetTimeAsString()
        {
            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
        
        public void EndTimerAndDisplayFinishTime()
        {
            UpdateTimer = false;

            textManager.UpdateTimerText($"Time: {minutes:00}:{seconds:00}:{milliseconds:00}");
        }
    }
}