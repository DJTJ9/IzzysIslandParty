using System.Collections.Generic;
using UnityEngine;

namespace ImprovedTimers
{
    public static class TimerManager
    {
        static readonly List<Timer> timers = new();

        public static void RegisterTimer(Timer timer)
        {
            if (!timers.Contains(timer)) timers.Add(timer);
        }

        public static void DeregisterTimer(Timer timer) => timers.Remove(timer);

        public static bool IsRegistered(Timer timer) => timers.Contains(timer);

        public static void UpdateTimers()
        {
            foreach (var timer in new List<Timer>(timers))
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public static void Clear() => timers.Clear();
    }
}