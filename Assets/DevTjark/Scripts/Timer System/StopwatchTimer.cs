using UnityEngine;

namespace ImprovedTimers {
    public class StopwatchTimer : Timer {
        public StopwatchTimer(bool autoTick = false) : base(0, autoTick) { }

        public override void Tick(float deltaTime) {
            if (IsRunning) {
                CurrentTime += Time.deltaTime;
            }
        }

        public void Reset() => CurrentTime = 0;

        public float GetTime() => CurrentTime;
    }
}
