using enums;
using FishingGame.QuickTimeEvents;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace FishingGame
{
    [RequireComponent(typeof(FishingSystemManager))]
    [RequireComponent(typeof(PlayerInput))]
    public class FishingRodController : Controller
    {
        private static readonly int cast = Animator.StringToHash("IsCast");
        private static readonly int fishBiting = Animator.StringToHash("FishBiting");

        private Animator animator;
        private LineRenderer lineRenderer;
        private FishingSystemManager fishingSystemManager;

        [SerializeField] private Transform[] rodLineRendererPositions;
        
        [Header("Dependencies: ")]
        [SerializeField] private QTEDisplayService qteDisplayService;
        [SerializeField] private MeshRenderer animationLureRenderer;
        [SerializeField] private MeshRenderer physicsLureRenderer;

        private bool isCast = false;
        private bool firstTimeCast = true;

        [Header("Pausing: ")]
        [SerializeField] private UnityEvent OnPauseGame;

        [SerializeField] private UnityEvent OnUnpauseGame;
        private bool isPaused;

        private void Awake()
        {
            fishingSystemManager = GetComponent<FishingSystemManager>();
            playerInput = GetComponent<PlayerInput>();

            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning("No animator attached to children of " + gameObject.name);

            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                Debug.LogWarning("No lineRenderer attached to " + gameObject.name);

            physicsLureRenderer.material = animationLureRenderer.material;
            
            lineRenderer.enabled = true;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
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

        public void OnJoin(InputAction.CallbackContext _context)
        {
            var device = _context.control.device;

            if (device is Gamepad gamepad)
            {
                if (gamepad is DualShockGamepad dualShockGamepad)
                    qteDisplayService.OnJoin(EControlScheme.PlayStation, PlayerIndex);
                else if (gamepad is XInputController xInputController)
                    qteDisplayService.OnJoin(EControlScheme.Xbox, PlayerIndex);
            }
            else if (device is Keyboard keyboard)
                qteDisplayService.OnJoin(EControlScheme.Keyboard, PlayerIndex);
        }

        public void OnStopFishDisplay(InputAction.CallbackContext _context)
        {
            if (_context.performed)
                fishingSystemManager.StopFishDisplay();
        }

        public void OnCast(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                if (firstTimeCast)
                {
                    OnJoin(_context);
                    firstTimeCast = false;
                }

                if (!isCast)
                {
                    isCast = true;
                    animator.SetBool(cast, isCast);

                    fishingSystemManager.StartFishing();

                    return;
                }

                if (fishingSystemManager.FishHooked)
                {
                    fishingSystemManager.PressedCatch();

                    return;
                }

                PullBackFishingRod();
            }
        }

        public void PullBackFishingRod()
        {
            isCast = false;
            animator.SetBool(cast, isCast);

            fishingSystemManager.StopFishing();
        }

        public void PlayFishBitingAnimation()
        {
            animator.SetBool(fishBiting, true);
        }

        public void StopFishBitingAnimation()
        {
            animator.SetBool(fishBiting, false);
        }

        private void LateUpdate()
        {
            if (lineRenderer == null || rodLineRendererPositions == null || rodLineRendererPositions.Length < 1)
                return;

            lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);

            // The second position is either the animationLure, or the physicsLure depending on what is currently active
            if (rodLineRendererPositions[1].gameObject.activeInHierarchy)
                lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
            else if (rodLineRendererPositions[2].gameObject.activeInHierarchy)
                lineRenderer.SetPosition(1, rodLineRendererPositions[2].position);
        }

        public override void SwitchToPlayerInputMap()
        {
            playerInput.SwitchCurrentActionMap("FishingGame");
        }
    }
}