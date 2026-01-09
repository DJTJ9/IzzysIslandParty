using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingGame
{
    public class FishingRodController : MonoBehaviour
    {
        // !! These don't exist
        private static readonly int cast = Animator.StringToHash("IsCast");
        private static readonly int fishBiting = Animator.StringToHash("FishBiting");

        private Animator animator;
        private LineRenderer lineRenderer;

        [SerializeField] private Transform[] rodLineRendererPositions;

        private bool isCast = false;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogWarning("No animator attached to children of " + gameObject.name);

            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                Debug.LogWarning("No lineRenderer attached to " + gameObject.name);
            
            lineRenderer.enabled = true;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = lineRenderer.endWidth = 0.02f;
            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
        }

        public void OnCast(InputAction.CallbackContext _context)
        {
            if (_context.performed)
            {
                if (!isCast)
                {
                    isCast = true;
                    animator.SetBool(cast, isCast);

                    FishingSystem.GetInstance().StartFishing();

                    return;
                }

                if (FishingSystem.GetInstance().FishHooked)
                {
                    FishingSystem.GetInstance().PressedCatch = true;

                    return;
                }

                PullBackFishingRod();
            }
        }

        public void PullBackFishingRod()
        {
            FishingSystem.GetInstance().StopFishing();

            isCast = false;

            animator.SetBool(cast, isCast);
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
            lineRenderer.SetPosition(0, rodLineRendererPositions[0].position);
            lineRenderer.SetPosition(1, rodLineRendererPositions[1].position);
        }
    }
}