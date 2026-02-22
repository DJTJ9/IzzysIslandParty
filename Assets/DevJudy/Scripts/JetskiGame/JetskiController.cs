using System;
using Audio;
using enums;
using Juice;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace JetskiGame
{
    public class JetskiController : Controller
    {
        private SO_PlayerRacingGames player;

        private int timeDeductionSeconds;
        private int timeDeductionMinutes;

        [Header("Drive settings: ")]
        private Rigidbody rb;

        [SerializeField] private Transform motor;
        [SerializeField] private float power = 20f;
        [SerializeField] private float steerPower = 800f;
        private Vector2 moveInput = Vector2.zero;
        private bool driving;

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

        [Header("Pausing: ")]
        [SerializeField] private UnityEvent OnPauseGame;

        [SerializeField] private UnityEvent OnUnpauseGame;
        private bool isPaused;

        [Header("Dependencies: ")]
        [SerializeField] private IconHandler iconHandler;
        [SerializeField] private GameObject onFinishLineCrossedText;

        [Header("Debug: ")]
        [SerializeField] private ForceMode forceMode;
        [SerializeField] private ForceMode jumpForceMode;
        [SerializeField] private bool showSpeedText;
        [SerializeField] private TextMeshProUGUI speedText;


        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            if (!showSpeedText && speedText != null)
                speedText.gameObject.SetActive(false);
        }

        public void OnPlayerJoined(SO_PlayerRacingGames _player)
        {
            player = _player;

            player.PlayerScore.Value = 0;
            player.TimeValue = 0;
            player.Time = String.Empty;
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
            Vector3 moveInput3d = new Vector3(0f, 0f, moveInput.y);

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

            if (speedText != null && showSpeedText)
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
            var groundCheckPosition = new Vector3(motor.position.x, motor.position.y, cancelledJumpHeight);

            Collider[] results = new Collider[1];

            isGrounded = Physics.OverlapSphereNonAlloc(groundCheckPosition, groundCheckRadius, results, waterLayerMask) > 0;
        }

        public void SetPlacement(int _placement)
        {
            player.PlayerScore.Value = _placement;
        }

        public void SetTime(string _time)
        {
            player.Time = _time;
        }

        public void OnObstacleCleared()
        {
            int randomEmote = Random.Range(0, 2);

            if (randomEmote == 0)
                iconHandler.DisplayIcon(EEmotion.Love);
            else
                iconHandler.DisplayIcon(EEmotion.Happy);
        }

        public void OnObstacleMissed(float _timeDeduction, out int _timeDeductionMinutes, out int _timeDeductionSeconds)
        {
            int randomEmote = Random.Range(0, 3);

            switch (randomEmote)
            {
                case 0:
                    iconHandler.DisplayIcon(EEmotion.Sad);
                    break;
                case 1:
                    iconHandler.DisplayIcon(EEmotion.Embarrassed);
                    break;
            }

            int currentDeduction = (int)(timeDeductionSeconds + _timeDeduction);

            if (currentDeduction >= 60)
            {
                timeDeductionMinutes++;
                timeDeductionSeconds = (currentDeduction % 60);
            }
            else
                timeDeductionSeconds = currentDeduction;

            _timeDeductionMinutes = timeDeductionMinutes;
            _timeDeductionSeconds = timeDeductionSeconds;
        }

        public void OnFinishLineCrosses()
        {
            onFinishLineCrossedText.SetActive(true);
        }

        public void GetFinalTimeDeduction(out int _minutes, out int _seconds)
        {
            _minutes = timeDeductionMinutes;
            _seconds = timeDeductionSeconds;
        }

        public override void SwitchToPlayerInputMap()
        {
            playerInput.SwitchCurrentActionMap("JetskiGame");
        }

        public void OnDrawGizmos()
        {
            var groundCheckPosition = new Vector3(motor.position.x, motor.position.y, motor.position.z); // - groundCheckOffset

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheckPosition, groundCheckRadius);
        }
    }
}