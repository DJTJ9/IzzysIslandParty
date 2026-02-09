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

        [Header("Slider: ")]
        [SerializeField] private Slider successSlider;

        [SerializeField] private Image sliderFillImage;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color catcherInFrameColor;
        [SerializeField] private Color catcherOutFrameColor;

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
            else
                targetRT = target.GetComponent<RectTransform>();

            if (catcher == null)
                Debug.LogError("Catcher is null");
            else
            {
                catcherImage = catcher.GetComponent<RawImage>();
                catcherRT = catcher.GetComponent<RectTransform>();
            }

            if (successSlider == null)
                Debug.LogError("SuccessSlider is null");

            if (sliderFillImage == null)
                Debug.LogError("SliderFillImage is null");
            else
                sliderFillImage.color = normalColor;

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

        public void SetLeftRightInput(float _moveInput)
        {
            moveInput = _moveInput;
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
            //bool overlap = Overlaps(_rt1, _rt2);
            bool overlap = RectContainsAnother1(_rt2, _rt1);

            if (overlap)
                sliderFillImage.color = catcherInFrameColor;
            else
                sliderFillImage.color = catcherOutFrameColor;

            return overlap;
        }

        private bool RectContainsAnother1(RectTransform _rct, RectTransform _other)
        {
            Vector2 minMaxValuesX = new Vector2(0, 0);
            Vector2 minMaxValuesY = new Vector2(0, 0);

            minMaxValuesX.x = _other.localPosition.x - (_other.rect.width / 2);
            minMaxValuesX.y = _other.localPosition.x + (_other.rect.width / 2);
            minMaxValuesY.x = _other.localPosition.y - (_other.rect.height / 2);
            minMaxValuesY.y = _other.localPosition.y + (_other.rect.height / 2);

            if (_rct.localPosition.x > minMaxValuesX.x & _rct.localPosition.x < minMaxValuesX.y)
            {
                if (_rct.localPosition.y > minMaxValuesY.x & _rct.localPosition.y < minMaxValuesY.y)
                    return true;
            }

            return false;
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

            barQTEHolder.SetActive(false);
        }

        public override void StopQTE()
        {
            StopBarQTE();
        }
    }
}