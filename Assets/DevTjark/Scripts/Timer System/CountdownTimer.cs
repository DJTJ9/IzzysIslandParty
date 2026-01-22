namespace ImprovedTimers
{
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value)
        {
        }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= deltaTime;
            }

            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public bool IsFinished => CurrentTime <= 0;

        public void Reset() => CurrentTime = initialTime;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }

        // Needed for the hurlde game slide-animation
        
        /// <summary>
        /// Change the duration of the time to the given duration, but if the timer is already less than the new duration, keeps it running as before
        /// </summary>
        /// <param name="_newDuration"></param>
        public void ChangeDurationWithoutResetting(float _newDuration)
        {
            if (CurrentTime > _newDuration)
                Reset(_newDuration);
        }
    }
}