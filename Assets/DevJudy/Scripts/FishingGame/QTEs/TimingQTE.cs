using enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FishingGame.QuickTimeEvents
{
    public class TimingQTE : QuickTimeEvent
    {
        [SerializeField] private ButtonDisplayService buttonDisplayService;
        [SerializeField] private QTEController qteController;

        [Header("TimingEvent needed components: ")]
        [SerializeField] private Image targetRing;

        [SerializeField] private GameObject movingRing;
        [SerializeField] private TextMeshProUGUI qteButtonText;
        [SerializeField] private GameObject timingEventHolder;

        [Header("TimingEvent variables: ")]
        [SerializeField] private float allowedTimingOffset = 0.00007f;
        [SerializeField] private float shrinkSpeed = 0.5f;
        private float ringTargetScale;
        private Vector3 ringStartScale;

        [SerializeField] private Color ringSuccessColor;
        [SerializeField] private Color ringFailureColor;
        [SerializeField] private Color ringNormalColor;
        
        private EButton currentButtonToPress;

        private bool qteFailed;

        private void Start()
        {  
            TimingEventSetup();
        }

        private void TimingEventSetup()
        {
            ringStartScale = movingRing.transform.localScale;

            ringTargetScale = targetRing.gameObject.transform.lossyScale.x;

            timingEventHolder.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (QTERunning)
                ShrinkRing();

            if (movingRing.transform.localScale.x <= 0.03 || qteFailed)
                StopShrinkingRing();
        }

        [ContextMenu("ShrinkRing")]
        public override void StartQTE()
        {
            qteFailed = false;
            QTEFinishedSuccessfully = false;

            targetRing.color = ringNormalColor;

            currentButtonToPress = (EButton)Random.Range(0, 4);
            qteButtonText.text = buttonDisplayService.DisplayButtonToPress(currentButtonToPress);

            qteController.CurrentQuickTimeEvent = EQuickTimeEvent.Timing;

            timingEventHolder.gameObject.SetActive(true);

            QTERunning = true;
        }

        private void ShrinkRing()
        {
            movingRing.transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed) * Time.deltaTime;
        }

        private void StopShrinkingRing()
        {
            timingEventHolder.gameObject.SetActive(false);

            QTERunning = false;
            movingRing.transform.localScale = ringStartScale;
        }

        public void CheckTimingSuccess(EButton _buttonPressed)
        {
            float ringScale = movingRing.transform.lossyScale.x;
            
            if (ringTargetScale + allowedTimingOffset >= ringScale && _buttonPressed == currentButtonToPress)
            {
                targetRing.color = ringSuccessColor;
                QTEFinishedSuccessfully = true;
            }
            else
            {
                targetRing.color = ringFailureColor;
                qteFailed = true;
            }
        }

        public override void StopQTE()
        {
            StopShrinkingRing();
            
            qteController.CurrentQuickTimeEvent = EQuickTimeEvent.None;
        }
    }
}