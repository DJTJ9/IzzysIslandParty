using HurdleGame;
using ImprovedTimers;
using UnityEngine;

namespace AnimationHandler
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    public class HurdleAnimationHandler : MonoBehaviour
    {
        private static readonly int velocityY = Animator.StringToHash("VelocityY");
        private static readonly int hitObstacle = Animator.StringToHash("HitObstacle");
        private static readonly int gameStart = Animator.StringToHash("GameStart");

        private Animator animator;
        private Rigidbody rb;

        private Timer obstacleHitTimer;
        private float obstacleHitCountdown = 1f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            obstacleHitTimer = new CountdownTimer(obstacleHitCountdown);
            obstacleHitTimer.OnTimerStop += HitObstacleOver;
        }

        public void OnGameStart()
        {
            animator.SetBool(gameStart, true);
        }

        private void Update()
        {
            animator.SetFloat(velocityY, rb.linearVelocity.y);
        }

        public void OnHitObstacle()
        {
            if (obstacleHitTimer.IsRunning)
                return;

            animator.SetBool(hitObstacle, true);

            obstacleHitTimer.Start();
        }

        private void HitObstacleOver()
        {
            animator.SetBool(hitObstacle, false);
        }
    }
}