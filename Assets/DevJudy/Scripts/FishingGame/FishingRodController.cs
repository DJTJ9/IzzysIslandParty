using enums;
using Juice;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace FishingGame
{
    public class FishingRodController : MonoBehaviour
    {
        private static readonly int cast = Animator.StringToHash("IsCast");
        private static readonly int fishBiting = Animator.StringToHash("FishBiting");

        private Animator animator;
        private LineRenderer lineRenderer;

        [SerializeField] public IconHandler IconHandler;

        // !! Not working yet
        [SerializeField] private Transform[] rodLineRendererPositions;

        private bool isCast = false;
        public Coroutine FishingRoutine;

        [Header("Pausing: ")]
        [SerializeField] private UnityEvent OnPauseGame;

        [SerializeField] private UnityEvent OnUnpauseGame;

        private bool isPaused;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning("No animator attached to children of " + gameObject.name);

            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                Debug.LogWarning("No lineRenderer attached to " + gameObject.name);

            //lineRenderer.enabled = true;
            //lineRenderer.useWorldSpace = true;
            //lineRenderer.startWidth = lineRenderer.endWidth = 0.02f;
            //lineRenderer.positionCount = 2;

            //lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            //lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);

            // lineRenderer = GetComponent<LineRenderer>();
            // if (lineRenderer == null)
            //     Debug.LogWarning("No lineRenderer attached to " + gameObject.name);

            // lineRenderer.enabled = true;
            // lineRenderer.useWorldSpace = true;
            // lineRenderer.startWidth = lineRenderer.endWidth = 0.02f;
            // lineRenderer.positionCount = 2;

            // lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            // lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
        }

        public void OnPause(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                if (!isPaused)
                {
                    isPaused = true;
                    OnPauseGame?.Invoke();
                }
                else
                {
                    isPaused = false;
                    OnUnpauseGame?.Invoke();
                }
            }
        }

        public void OnStopFishDisplay(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                FishingSystem.Instance.StopFishDisplay();
            }
        }

        public void OnCast(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                if (!isCast)
                {
                    isCast = true;
                    animator.SetBool(cast, isCast);

                    FishingSystem.Instance.StartFishing();

                    return;
                }

                if (FishingSystem.Instance.FishHooked)
                {
                    FishingSystem.Instance.PressedCatch = true;
                    IconHandler.DisplayIcon(EEmotion.Happy);

                    return;
                }

                PullBackFishingRod();
            }
        }

        public void PullBackFishingRod(bool _stopFishingRoutine = true)
        {
            if (_stopFishingRoutine)
                FishingSystem.Instance.StopFishing();

            isCast = false;

            animator.SetBool(cast, isCast);
        }

        public void PlayFishBitingAnimation(bool _withIcon)
        {
            if (_withIcon)
                IconHandler.DisplayIcon(EEmotion.Alert);

            animator.SetBool(fishBiting, true);
        }

        public void StopFishBitingAnimation()
        {
            animator.SetBool(fishBiting, false);
        }

        private void LateUpdate()
        {
            //lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            //lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
        }

        //private void LateUpdate()
        //{
        //    if (Keyboard.current.cKey.wasPressedThisFrame)
        //        Debug.Log("C pressed");
        //    
        //    //lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
        //    //lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
        //}
    }
}