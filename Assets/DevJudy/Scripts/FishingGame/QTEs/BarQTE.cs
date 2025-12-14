using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.QuickTimeEvent
{
    public class BarQTE : MonoBehaviour
    {
        [Header("Components: ")]
        [SerializeField] private GameObject barQTEHolder;

        [SerializeField] private GameObject target;
        [SerializeField] private GameObject catcher;
        private RectTransform targetRT;
        private RectTransform catcherRT;

        [SerializeField] private Slider successSlider;

        [Header("Target variables: ")]
        [SerializeField] private float targetMaxMovementLeft = -250f;

        [SerializeField] private float targetMaxMovementRight = 250f;
        [SerializeField] private float targetMoveSpeed = 250f;
        [SerializeField] private float targetChangeFrequency = 0.01f;

        private float targetDestination;
        private bool targetMovingRight = true;

        [Header("Catcher variables: ")]
        [SerializeField] private float catcherMaxMovementLeft = -250f;

        [SerializeField] private float catcherMaxMovementRight = 250f;
        [SerializeField] private float catcherMoveSpeed = 250f;
        private float moveInput = 0f;

        [Header("General variables: ")]
        [SerializeField] private float successIncrement = 15f;

        [SerializeField] private float failDecrement = 12;
        [SerializeField] private float successThreshold = 100f;
        [SerializeField] private float failThreshold = -100f;
        private float successCounter = 0f;

        private bool rtsOverlapping;
        private bool barQTESuccessful = false;
        private bool barQTERunning = false;

        // TODO add enumerator so the event only lasts around 7-8 seconds max
        private void Start()
        {
            if (target == null)
                Debug.LogError("Target is null");
            if (catcher == null)
                Debug.LogError("Catcher is null");

            if (successSlider == null)
                Debug.LogError("SuccessSlider is null");

            successSlider.maxValue = successThreshold;
            successSlider.minValue = failThreshold;

            successCounter = successThreshold + failThreshold;
            successSlider.value = successCounter;

            targetRT = target.GetComponent<RectTransform>();
            catcherRT = catcher.GetComponent<RectTransform>();


        }

        public void StartBarQTE()
        {
            barQTERunning = true;
            barQTESuccessful = false;

            barQTEHolder.SetActive(true);

            ChangeTargetDestination();
        }

        public void OnLeftRightInput(InputAction.CallbackContext _context)
        {
            if (_context.started)
                moveInput = _context.ReadValue<float>();
            if (_context.canceled)
                moveInput = 0f;
        }

        private void FixedUpdate()
        {
            MoveTarget();

            MoveCatcher();

            rtsOverlapping = CheckIfRTsOverlapping(targetRT, catcherRT);

            OverlappingCalculation();
        }

        private void MoveTarget()
        {
            target.transform.localPosition = Vector3.MoveTowards(target.transform.localPosition,
                new Vector3(targetDestination, target.transform.localPosition.y,
                    target.transform.localPosition.z), targetMoveSpeed * Time.deltaTime);

            if (Mathf.Approximately(target.transform.localPosition.x, targetDestination))
                ChangeTargetDestination();

            // TODO This doesnt work bc even at tcf 0 it still changes destination
            float rnd = Random.value;

            if (rnd < targetChangeFrequency)
            {
                targetMovingRight = !targetMovingRight;
                targetDestination = targetMovingRight ? targetMaxMovementRight : targetMaxMovementLeft;
            }
        }

        private void MoveCatcher()
        {
            Vector3 movement = Vector3.right * (moveInput * catcherMoveSpeed * Time.fixedDeltaTime);

            Vector3 newPosition = catcher.transform.localPosition + movement;

            newPosition.x = Mathf.Clamp(newPosition.x, catcherMaxMovementLeft, catcherMaxMovementRight);

            // Technically the only thing that needs to move is the x...
            catcher.transform.localPosition = newPosition;
        }

        private void ChangeTargetDestination()
        {
            // Can be a direct comparison bc it's always set to either of these values exactly
            if (targetDestination == targetMaxMovementRight)
            {
                targetDestination = targetMaxMovementLeft;
                return;
            }

            targetDestination = targetMaxMovementRight;
        }

        private bool CheckIfRTsOverlapping(RectTransform _rt1, RectTransform _rt2)
        {
            Rect rect1 = new Rect(_rt1.position.x, _rt1.position.y, _rt1.rect.width, _rt1.rect.height);
            Rect rect2 = new Rect(_rt2.position.x, _rt2.position.y, _rt2.rect.width, _rt2.rect.height);

            return rect1.Overlaps(rect2);
        }

        private void OverlappingCalculation()
        {
            if (rtsOverlapping)
                successCounter += successIncrement * Time.deltaTime; // Why the delta time???
            else
                successCounter -= failDecrement * Time.deltaTime;

            successCounter = Mathf.Clamp(successCounter, failThreshold, successThreshold);

            // Adjust the slider
            successSlider.value = successCounter;

            if (successCounter >= successThreshold)
            {
                barQTESuccessful = true;
                StopBarQTE();
            }
            else if (successCounter <= failThreshold)
            {
                StopBarQTE();
            }
        }

        private void StopBarQTE()
        {
            successCounter = 0;
            successSlider.value = successCounter;

            barQTERunning = false;

            // Disable the gameObjects
            barQTEHolder.SetActive(false);
        }
    }
}
