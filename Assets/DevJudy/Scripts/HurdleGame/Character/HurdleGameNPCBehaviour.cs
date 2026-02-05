using System;
using ImprovedTimers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HurdleGame
{
    public class HurdleGameNPCBehaviour : MonoBehaviour
    {
        private Rigidbody rb;

        [Header("Obstacle Check:")]
        [SerializeField] private float obstacleCheckSize;

        [SerializeField] private Vector3 obstacleCheckPosition;
        [SerializeField] private LayerMask obstacleLayerMask;

        [Header("Jump variables:")]
        [SerializeField] private float jumpChancePercent = 0.5f;

        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float hopMultiplier = 0.5f;
        private Vector2 bigJumpHeight;
        private Vector2 smallJumpHeight;

        private Timer jumpCooldownTimer;
        private float jumpCooldown = 1f;
        private Timer smallJumpTimer;
        private float timeUntilSmallJump = 0.6f;

        [Header("GroundCheck variables: ")]
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private float groundCheckOffset = 1f;
        [SerializeField] private float groundCheckRadius = 0.3f;

        [field: SerializeField] public bool IsGrounded { get; private set; }
        [field: SerializeField] private bool canJump = true;
        private bool jumping;

        private void OnValidate()
        {
            jumpChancePercent = Mathf.Clamp(jumpChancePercent, 0, 1);
        }

        private void Start()
        {
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            jumpCooldownTimer.OnTimerStop += () => canJump = true;

            smallJumpTimer = new CountdownTimer(timeUntilSmallJump);
            smallJumpTimer.OnTimerStop += SmallJump;

            rb = GetComponent<Rigidbody>();

            bigJumpHeight = new Vector2(rb.linearVelocity.x, jumpForce);
            smallJumpHeight = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * hopMultiplier);
        }

        private void FixedUpdate()
        {
            GroundCheck();
            
            if (Physics.OverlapSphere(transform.position + obstacleCheckPosition, obstacleCheckSize, obstacleLayerMask).Length > 0
                && canJump)
                CheckIfCharJumps();
        }

        private void CheckIfCharJumps()
        {
            if (!IsGrounded)
                return;

            canJump = false;

            var jump = Random.Range(1, 10 + 1);
            if (jump * 0.1 >= jumpChancePercent)
                return;

            Jump(bigJumpHeight);

            var bigOrSmall = Random.Range(0, 2);

            if (bigOrSmall == 0)
                smallJumpTimer.Start();
        }

        private void SmallJump()
        {
            jumping = true;
            Jump(smallJumpHeight);
        }

        private void Jump(Vector2 _jumpHeight)
        {
            jumping = true;
            rb.linearVelocity = _jumpHeight;

            jumpCooldownTimer.Start();
        }

        private void GroundCheck()
        {
            Vector3 groundCheckPos = new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z);

            IsGrounded = Physics.OverlapSphere(groundCheckPos, groundCheckRadius, groundLayer).Length > 0;

            if (IsGrounded && rb.linearVelocity.y < 0.1)
            {
                jumping = false;
                canJump = true;
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z),
                groundCheckRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + obstacleCheckPosition, obstacleCheckSize);
        }
    }
}