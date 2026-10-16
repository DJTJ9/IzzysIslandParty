using NUnit.Framework;

namespace PopGuard.Tests
{
    public class UpwardSpikeFilterTests
    {
        private const float Dt = 0.02f;
        private const float MaxDelta = 6f;

        [Test]
        public void Step_SpikeWhileRolling_ClampsToZero()
        {
            var filter = new UpwardSpikeFilter();
            filter.Step(0f, Dt, MaxDelta, out _);

            var clamped = filter.Step(16f, Dt, MaxDelta, out var vy);

            Assert.IsTrue(clamped);
            Assert.AreEqual(0f, vy, 1e-4f);
        }

        [Test]
        public void Step_RampTransition_NotClamped()
        {
            var filter = new UpwardSpikeFilter();
            filter.Step(0f, Dt, MaxDelta, out _);

            var clamped = filter.Step(3.4f, Dt, MaxDelta, out var vy);

            Assert.IsFalse(clamped);
            Assert.AreEqual(3.4f, vy, 1e-4f);
        }

        [Test]
        public void Step_LandingBounce_NotClamped()
        {
            var filter = new UpwardSpikeFilter();
            filter.Step(-10f, Dt, MaxDelta, out _);

            var clamped = filter.Step(8f, Dt, MaxDelta, out var vy);

            Assert.IsFalse(clamped);
            Assert.AreEqual(8f, vy, 1e-4f);
        }

        [Test]
        public void Step_SpikeWhileLanding_ClampsToReflection()
        {
            var filter = new UpwardSpikeFilter();
            filter.Step(-10f, Dt, MaxDelta, out _);

            var clamped = filter.Step(20f, Dt, MaxDelta, out var vy);

            Assert.IsTrue(clamped);
            Assert.AreEqual(10f, vy, 1e-4f);
        }

        [Test]
        public void Step_WithinGrace_NotClamped()
        {
            var filter = new UpwardSpikeFilter();
            filter.Step(0f, Dt, MaxDelta, out _);
            filter.AllowUpwardImpulse(0.2f);

            var clamped = filter.Step(16f, Dt, MaxDelta, out var vy);

            Assert.IsFalse(clamped);
            Assert.AreEqual(16f, vy, 1e-4f);
        }

        [Test]
        public void Step_AfterGraceExpired_Clamps()
        {
            var filter = new UpwardSpikeFilter();
            filter.AllowUpwardImpulse(0.05f);
            filter.Step(0f, Dt, MaxDelta, out _);
            filter.Step(0f, Dt, MaxDelta, out _);
            filter.Step(0f, Dt, MaxDelta, out _);

            var clamped = filter.Step(16f, Dt, MaxDelta, out var vy);

            Assert.IsTrue(clamped);
            Assert.AreEqual(0f, vy, 1e-4f);
        }

        [Test]
        public void Step_ClampedValueBecomesBaseline()
        {
            var filter = new UpwardSpikeFilter();
            filter.Step(0f, Dt, MaxDelta, out _);
            filter.Step(16f, Dt, MaxDelta, out _);

            var clamped = filter.Step(16f, Dt, MaxDelta, out var vy);

            Assert.IsTrue(clamped);
            Assert.AreEqual(0f, vy, 1e-4f);
            Assert.AreEqual(0f, filter.LastVy, 1e-4f);
        }
    }
}
