using Audio;
using enums;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JetskiGame
{
    public class JetskiController : MonoBehaviour
    {
        [Header("Drive settings: ")]
        [SerializeField] private Transform motor;

        private PlayerInput playerInput;
        private Rigidbody rb;

        [SerializeField] private float power = 20f;
        [SerializeField] private float steerPower = 800f;
        private Vector2 moveInput = Vector2.zero;
        //private bool driving = false;

        [Header("Jump settings: ")]
        [SerializeField] private float regJumpHeight = 10f;

        [SerializeField] private float cancelledJumpHeight = 2f;
        private float prevUpwardVelocity;
        private float jumpHeight;
        private bool jumpPressedLastFrame;

        [Header("GroundCheck settings: ")]
        [SerializeField] private LayerMask waterLayerMask;

        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private bool isGrounded;

        [Header("Temp: ")]
        [SerializeField] private ForceMode forceMode;

        [SerializeField] private bool steerWithAddForceAtPos;
        [SerializeField] private ForceMode jumpForceMode;
        [SerializeField] private TextMeshProUGUI speedText;

        private bool driving;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            //DisablePlayerInput();
        }


        public void EnablePlayerInput()
        {
            if (playerInput != null)
                playerInput.enabled = true;
        }

        public void DisablePlayerInput()
        {
            // ?? Doesn't this also disable the ability to pause?
            playerInput.enabled = false;
        }

        public void OnMove(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                moveInput = _context.ReadValue<Vector2>();

                if (!driving)
                {
                    driving = true;

                    AudioService.Instance.PlaySoundWhile(() => driving,
                        AudioCollection.Instance.levelSoundsDictionary.LevelAudios["JetskiEngineSound"],
                        EAudioType.SFX, true, 0.3f, 0.25f);
                }
            }

            if (_context.canceled)
            {
                moveInput = Vector2.zero;

                driving = false;
            }
        }

        private void MoveJetski()
        {
            Vector3 moveInput3d;

            if (steerWithAddForceAtPos)
                moveInput3d = new Vector3(0f, 0f, moveInput.y);
            else
                moveInput3d = new Vector3(moveInput.x, 0f, moveInput.y);

            // !! Change after playtest
            switch (forceMode)
            {
                case ForceMode.VelocityChange:
                    var vcPower = power / 100f;
                    rb.AddRelativeForce(moveInput3d.normalized * vcPower, ForceMode.VelocityChange);
                    break;
                case ForceMode.Acceleration:
                    rb.AddRelativeForce(moveInput3d.normalized * power, ForceMode.Acceleration);
                    break;
                case ForceMode.Force:
                    rb.AddRelativeForce(moveInput3d.normalized * power, ForceMode.Force);
                    break;
            }

            if (speedText != null)
                speedText.text = "Speed: " + Mathf.Round(new Vector3(0f, 0f, rb.linearVelocity.z).magnitude);
        }

        public void OnJump(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                jumpPressedLastFrame = true;

                jumpHeight = regJumpHeight;

                if (isGrounded)
                    Jump(jumpHeight);
            }

            if (_context.canceled)
            {
                jumpHeight = cancelledJumpHeight;

                if (rb.linearVelocity.y > 0.1f)
                    Jump(cancelledJumpHeight);
            }
        }

        private void Jump(float _jumpHeight)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, _jumpHeight, rb.linearVelocity.z);

            jumpPressedLastFrame = false;
        }

        private void FixedUpdate()
        {
            GroundCheck();

            if (isGrounded && jumpPressedLastFrame)
                Jump(jumpHeight);
            else if (!isGrounded && jumpPressedLastFrame)
                jumpPressedLastFrame = false;


            // First do the steering
            if (steerWithAddForceAtPos)
                rb.AddForceAtPosition(transform.right * (-moveInput.x * steerPower) / 100f, motor.position);

            MoveJetski();

            // if (driving)
            //     If particleSystem
            // not playing, play

            // if (!driving)
            //     If particleSystem
            // not playing, play
        }


        private void GroundCheck()
        {
            var groundCheckPosition = new Vector3(motor.position.x, motor.position.y, cancelledJumpHeight); // - groundCheckOffset

            Collider[] results = new Collider[1];

            isGrounded = Physics.OverlapSphereNonAlloc(groundCheckPosition, groundCheckRadius, results, waterLayerMask) > 0;
        }

        public void OnDrawGizmos()
        {
            var groundCheckPosition = new Vector3(motor.position.x, motor.position.y, motor.position.z); // - groundCheckOffset

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheckPosition, groundCheckRadius);
        }
    }
}