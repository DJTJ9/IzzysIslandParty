using NUnit.Framework;

namespace ImprovedTimers.Tests
{
    public class TimerAutoTickTests
    {
        [SetUp]
        public void SetUp() => TimerManager.Clear();

        [TearDown]
        public void TearDown() => TimerManager.Clear();

        [Test]
        public void Start_WithoutAutoTick_DoesNotRegister()
        {
            var timer = new CountdownTimer(1f);

            timer.Start();

            Assert.IsFalse(TimerManager.IsRegistered(timer));
        }

        [Test]
        public void Start_WithAutoTick_Registers()
        {
            var timer = new CountdownTimer(1f, autoTick: true);

            timer.Start();

            Assert.IsTrue(TimerManager.IsRegistered(timer));
        }

        [Test]
        public void Stop_WithAutoTick_Deregisters()
        {
            var timer = new CountdownTimer(1f, autoTick: true);
            timer.Start();

            timer.Stop();

            Assert.IsFalse(TimerManager.IsRegistered(timer));
        }

        [Test]
        public void Dispose_WithAutoTick_Deregisters()
        {
            var timer = new CountdownTimer(1f, autoTick: true);
            timer.Start();

            timer.Dispose();

            Assert.IsFalse(TimerManager.IsRegistered(timer));
        }

        [Test]
        public void StartAfterPause_WithAutoTick_RegistersOnlyOnce()
        {
            var timer = new CountdownTimer(1f, autoTick: true);
            timer.Start();
            timer.Pause();
            timer.Start();

            timer.Stop();

            Assert.IsFalse(TimerManager.IsRegistered(timer));
        }

        [Test]
        public void StopwatchTimer_WithAutoTick_Registers()
        {
            var timer = new StopwatchTimer(autoTick: true);

            timer.Start();

            Assert.IsTrue(TimerManager.IsRegistered(timer));
        }

        [Test]
        public void ManualTick_WithoutAutoTick_CountsDownByGivenDelta()
        {
            var timer = new CountdownTimer(1f);
            timer.Start();

            timer.Tick(0.25f);

            Assert.AreEqual(0.75f, timer.CurrentTime, 1e-5f);
        }
    }
}
