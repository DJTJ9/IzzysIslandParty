using UIScripts;
using UnityEngine;

namespace MultiuseScripts
{
    public class LevelTimer : MonoBehaviour
    {
        private static LevelTimer instance;
        public static LevelTimer Instance => instance;

        [Header("Dependencies: ")]
        [SerializeField] private UITimerManager timerManager;

        [SerializeField] private LevelServiceParent levelService;

        [Header("Variables: ")]
        [SerializeField] private float durationInMinutes;
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

        private void Awake()
        {
            if (instance != null)
                Destroy(gameObject);

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
            if (timerFinished)
                return;

            if (timerRunningDown)
                time -= _timeDeduction;
            else
                time += _timeDeduction;

            StartCoroutine(timerManager.TimeDeductionFeedback());
        }
        
        private void DisplayRunningTimer()
        {
            time += Time.fixedDeltaTime;

            TimeToTimerTextFormat(time);
        }

        private void DisplayRunningDownTimer()
        {
            time = Mathf.Max(time - Time.fixedDeltaTime, 0f);
            TimeToTimerTextFormat(time);

            if (time <= 0.001f)
            {
                TimerFinished = true;
                time = 0.00f;

                levelService?.EndLevel();
            }
        }

        private void TimeToTimerTextFormat(float _time)
        {
            minutes = Mathf.FloorToInt(_time / 60);
            seconds = Mathf.FloorToInt(_time % 60);
            milliseconds = Mathf.Round((_time % 1) * 1000);
            milliseconds = Mathf.RoundToInt((milliseconds) / 10);
            
            timerManager.UpdateTimerText($"Time: {minutes:00}:{seconds:00}:{milliseconds % 100:00}");
        }

        public void GetTime(out int _minutes, out int _seconds, out int _milliseconds)
        {
            _minutes = (int)minutes;
            _seconds = (int)seconds;
            _milliseconds = (int)milliseconds;
        }
        
        public string GetTimeAsString(int _minutes, int _seconds, int _milliseconds)
        {
            return $"{_minutes:00}:{_seconds:00}:{_milliseconds % 100:00}";
        }

        public void EndTimerAndDisplayFinishTime()
        {
            UpdateTimer = false;

            timerManager.UpdateTimerText($"Time: {minutes:00}:{seconds:00}:{milliseconds % 100:00}");
        }
    }
}