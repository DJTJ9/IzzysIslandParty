using Audio;
using enums;
using ImprovedTimers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HurdleGame
{
    public class HurdlePlayerController : Controller
    {
        private CountdownTimer slideTimer;

        private Rigidbody rb;
        private CapsuleCollider capCollider;

        [Header("Jump variables: ")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float hopMultiplier = 0.5f;
        [SerializeField] private Vector2 jumpVolumeRange = new Vector2(1f, 1f);
        [SerializeField] private Vector2 jumpPitchRange = new Vector2(1f, 1f);
        private Vector2 bigJumpHeight;
        private Vector2 smallJumpHeight;

        [Header("Slide variables: ")]
        [SerializeField] private float longSlideSeconds = 2f;
        [SerializeField] private float shortSlideMultiplier = 0.5f;
        private float shortSlideSeconds;

        [Header("Slide collider: ")]
        [SerializeField] private Vector2 slideColliderSize = new Vector2(0.2f, 0.3f);
        [SerializeField] private Vector3 slideColliderCenter = new Vector3(0f, 0.2f, 0f);

        private Vector2 regColliderSize = new Vector2(0.2f, 0.61f);
        private Vector3 regColliderCenter = new Vector3(0f, 0.31f, 0f);

        [Header("GroundCheck variables: ")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckOffset = 1f;
        [SerializeField] private float groundCheckRadius = 0.3f;
        
        private bool IsGrounded
        {
            get;
            set;
        }
        
        [Header("Pausing: ")]
        [SerializeField] private UnityEvent OnPauseGame;
        [SerializeField] private UnityEvent OnUnpauseGame;
        private bool isPaused;

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
        }

        public void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
        
        public void OnPause(InputAction.CallbackContext _context)
        {
            if (_context.started)
            {
                if (!isPaused)
                {
                    isPaused = true;
                    OnPauseGame.Invoke();
                }
                else
                {
                    isPaused = false;
                    OnUnpauseGame.Invoke();
                }
            }
        }
        
        public void OnJump(InputAction.CallbackContext _context)
        {
            if (_context.started && IsGrounded)
            {
                AudioService.Instance.PlaySoundWithRandomPitch(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["JumpingSound"], EAudioType.SFX,
                    jumpVolumeRange, jumpPitchRange);
                
                Jump(bigJumpHeight);
            }

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
            // !! Tick slideTimer
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
            GroundCheck();
        }

        private void GroundCheck()
        {
            Vector3 groundCheckPos = new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z);

            IsGrounded = Physics.OverlapSphere(groundCheckPos, groundCheckRadius, groundLayer).Length > 0;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - groundCheckOffset, transform.position.z),
                groundCheckRadius);
        }
    }
}