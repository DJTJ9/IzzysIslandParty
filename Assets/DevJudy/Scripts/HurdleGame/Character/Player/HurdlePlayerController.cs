using ImprovedTimers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HurdleGame
{
    public class HurdlePlayerController : MonoBehaviour
    {
        private CountdownTimer slideTimer;

        private Rigidbody rb;
        private CapsuleCollider capCollider;

        [Header("Jump variables: ")]
        [SerializeField] private float jumpForce = 7f;

        [SerializeField] private float hopMultiplier = 0.5f;
        [SerializeField] private float additionalFallWeight = 0.3f;
        private Vector2 bigJumpHeight;
        private Vector2 smallJumpHeight;
        private float prevUpVelocity;
        private float gravity = 9.8f;

        [Header("Slide variables: ")]
        [SerializeField] private float longSlideSeconds = 2f;

        [SerializeField] private float shortSlideMultiplier = 0.5f;
        private float shortSlideSeconds;

        [Header("Slide collider: ")]
        [SerializeField] private Vector2 slideColliderSize = new Vector2(0.2f, 0.3f);

        [SerializeField] private Vector3 slideColliderCenter = new Vector3(0f, 0.2f, 0f);

        private Vector2 regColliderSize = new Vector2(0.2f, 0.61f);
        private Vector3 regColliderCenter = new Vector3(0f, 0.31f, 0f);

        // [Header("Movement variables: ")]
        // private float allowedPosOffset = 2f;
        private Vector3 ogRunningPos;

        [Header("GroundCheck variables: ")]
        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private float groundCheckOffset = 1f;
        [SerializeField] private float groundCheckRadius = 0.3f;

        [SerializeField] private bool isGrounded;

        private bool IsGrounded
        {
            get => isGrounded;
            set
            {
                isGrounded = value;

                if (value)
                    isFalling = false;
            }
        }

        [SerializeField] private bool isFalling;

        [SerializeField] private bool testGravity;
        [SerializeField] private bool testAddWeight;
        [SerializeField] private bool testPrevVelocity;

        private void Awake()
        {
            slideTimer = new CountdownTimer(longSlideSeconds);

            rb = GetComponent<Rigidbody>();
            capCollider = GetComponent<CapsuleCollider>();

            regColliderCenter = capCollider.center;
            regColliderSize = new Vector2(capCollider.radius, capCollider.height);

            bigJumpHeight = new Vector2(rb.linearVelocity.x, jumpForce);
            smallJumpHeight = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * hopMultiplier);

            shortSlideSeconds = longSlideSeconds * shortSlideMultiplier;

            ogRunningPos = transform.position;
        }

        public void OnJump(InputAction.CallbackContext _context)
        {
            if (_context.started && IsGrounded)
                Jump(bigJumpHeight);

            if (_context.canceled)
            {
                // Only change the jumpForce if the player isn't falling yet
                if (rb.linearVelocity.y > 0.1f)
                    Jump(smallJumpHeight);
            }
        }

        private void Jump(Vector2 _jumpHeight)
        {
            if (slideTimer.IsRunning)
            {
                slideTimer.Stop();
                ChangeCollider(false);
            }

            rb.linearVelocity = _jumpHeight;
        }

        public void OnSlide(InputAction.CallbackContext _context)
        {
            if (!IsGrounded)
                return;

            if (_context.started && IsGrounded)
            {
                Slide(false);
            }

            if (_context.canceled)
                Slide(true);
        }

        private void Slide(bool _cancelledSlide)
        {
            if (!slideTimer.IsRunning && !_cancelledSlide)
            {
                slideTimer.Start();

                ChangeCollider(true);

                // play animation
                return;
            }

            if (_cancelledSlide)
                slideTimer.ChangeDurationWithoutResetting(shortSlideSeconds);

            if (!slideTimer.IsRunning)
                ChangeCollider(false);
        }

        private void ChangeCollider(bool _sliding)
        {
            if (_sliding)
            {
                capCollider.radius = slideColliderSize.x;
                capCollider.height = slideColliderSize.y;

                capCollider.center = slideColliderCenter;

                return;
            }

            capCollider.radius = regColliderSize.x;
            capCollider.height = regColliderSize.y;

            capCollider.center = regColliderCenter;
        }

        private void FixedUpdate()
        {
            if (slideTimer.IsRunning && !slideTimer.IsRunning)
            {
                ChangeCollider(false);

                // Cancel animation
            }

            GroundCheck();

            if (!IsGrounded)
            {
                if (testPrevVelocity)
                {
                    rb.linearVelocity -= new Vector3(0, prevUpVelocity, 0f);
                    prevUpVelocity = rb.linearVelocity.y;
                }
                else if (testAddWeight)
                {
                    rb.linearVelocity -= new Vector3(0, additionalFallWeight, 0f);
                }
                else if (testGravity)
                {
                    rb.linearVelocity -= new Vector3(0, gravity, 0f);
                }
            }


            // if (transform.position.x + ogRunningPos.x > allowedPosOffset || transform.position.x - ogRunningPos.x < -allowedPosOffset)
            // {
            //     rb.transform.position = ogRunningPos;
            //     rb.linearVelocity = Vector3.zero;
            // }
        }

        private void GroundCheck()
        {
            Vector3 groundCheckPos = new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z);

            IsGrounded = Physics.OverlapSphere(groundCheckPos, groundCheckRadius, groundLayer).Length > 0;

            if (!IsGrounded && rb.linearVelocity.y < 0.01f)
                isFalling = true;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z),
                groundCheckRadius);
        }
    }
}