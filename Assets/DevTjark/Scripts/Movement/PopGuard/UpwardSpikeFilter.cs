using UnityEngine;

namespace PopGuard
{
    /// <summary>
    /// Per-physics-step filter against unexplained upward velocity spikes (ghost collisions).
    /// An upward speed above the reflection of the previous step plus a tolerance is treated
    /// as a pop and reduced to that reflection, unless a legitimate impulse opened a grace window.
    /// </summary>
    public sealed class UpwardSpikeFilter
    {
        private float graceRemaining;

        /// <summary>Vertical velocity after the previous step (already filtered).</summary>
        public float LastVy { get; private set; }

        /// <summary>
        /// Opens a grace window during which no spike is clamped. Call before applying
        /// an intended upward impulse (shoot, jump, bump).
        /// </summary>
        /// <param name="graceDuration">Window length in seconds.</param>
        public void AllowUpwardImpulse(float graceDuration)
        {
            graceRemaining = graceDuration;
        }

        /// <summary>
        /// Filters one physics step.
        /// </summary>
        /// <param name="vy">Current vertical velocity.</param>
        /// <param name="deltaTime">Physics step length.</param>
        /// <param name="maxUpwardDeltaPerStep">Allowed rise above the previous step's reflection.</param>
        /// <param name="filteredVy">Vertical velocity to apply.</param>
        /// <returns>True if the velocity was clamped.</returns>
        public bool Step(float vy, float deltaTime, float maxUpwardDeltaPerStep, out float filteredVy)
        {
            filteredVy = vy;
            var clamped = false;

            if (graceRemaining > 0f)
            {
                graceRemaining -= deltaTime;
            }
            else
            {
                var reflection = Mathf.Abs(LastVy);
                if (vy > reflection + maxUpwardDeltaPerStep)
                {
                    filteredVy = reflection;
                    clamped = true;
                }
            }

            LastVy = filteredVy;
            return clamped;
        }
    }
}
