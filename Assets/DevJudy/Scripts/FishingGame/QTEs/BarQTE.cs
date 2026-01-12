using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FishingGame.QuickTimeEvents
{
    public class BarQTE : QuickTimeEvent
    {
        [Header("Components: ")]
        [SerializeField] private GameObject barQTEHolder;

        [SerializeField] private GameObject target;
        [SerializeField] private GameObject catcher;
        private RectTransform targetRT;
        private RectTransform catcherRT;
        private RawImage catcherImage;

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

        private void Start()
        {
            if (target == null)
                Debug.LogError("Target is null");
            if (catcher == null)
                Debug.LogError("Catcher is null");

            catcherImage = catcher.GetComponent<RawImage>();
            catcherRT = catcher.GetComponent<RectTransform>();

            targetRT = target.GetComponent<RectTransform>();

            if (successSlider == null)
                Debug.LogError("SuccessSlider is null");

            barQTEHolder.SetActive(false);

            SetUpBarQTE();
        }

        private void SetUpBarQTE()
        {
            successSlider.maxValue = successThreshold;
            successSlider.minValue = failThreshold;

            successCounter = successThreshold + failThreshold;
            successSlider.value = successCounter;
        }

        public override void StartQTE()
        {
            QTERunning = true;
            QTEFinishedSuccessfully = false;

            barQTEHolder.SetActive(true);

            ChangeTargetDestination();
        }

        public void OnLeftRightInput(InputAction.CallbackContext _context)
        {
            if (!QTERunning)
                return;

            if (_context.started)
            {
                moveInput = _context.ReadValue<float>();

                if (moveInput > 0.02f)
                    moveInput = 1f;
                else if (moveInput < -0.02f)
                    moveInput = -1f;
                else
                    moveInput = 0f;
            }

            if (_context.canceled)
                moveInput = 0f;
        }

        private void FixedUpdate()
        {
            if (QTERunning)
            {
                MoveTarget();

                MoveCatcher();

                rtsOverlapping = CheckIfRTsOverlapping(targetRT, catcherRT);

                OverlappingCalculation();
            }
        }

        private void MoveTarget()
        {
            target.transform.localPosition = Vector3.MoveTowards(target.transform.localPosition,
                new Vector3(targetDestination, target.transform.localPosition.y,
                    target.transform.localPosition.z), targetMoveSpeed * Time.deltaTime);

            if (Mathf.Approximately(target.transform.localPosition.x, targetDestination))
                ChangeTargetDestination();

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

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
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
            bool overlap = Overlaps(_rt1, _rt2);

            if (overlap)
                catcherImage.color = Color.green;
            else
            {
                catcherImage.color = Color.red;
            }

            return overlap;
        }

        private bool Overlaps(RectTransform _a, RectTransform _b)
        {
            return WorldRect(_a).Overlaps(WorldRect(_b));
        }

        private Rect WorldRect(RectTransform _rectTransform)
        {
            Vector2 sizeDelta = _rectTransform.sizeDelta;
            float rectTransformWidth = sizeDelta.x * _rectTransform.lossyScale.x;
            float rectTransformHeight = sizeDelta.y * _rectTransform.lossyScale.y;

            Vector3 position = _rectTransform.position;
            return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
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
                QTEFinishedSuccessfully = true;
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

            QTERunning = false;

            // Disable the gameObjects
            barQTEHolder.SetActive(false);
        }
    }
}