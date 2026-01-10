using enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FishingGame.QuickTimeEvents
{
    public class TimingQTE : QuickTimeEvent
    {
        [SerializeField] private QTEController qteController;

        [Header("TimingEvent needed components: ")]
        [SerializeField] private Image targetRing;

        [SerializeField] private GameObject movingRing;
        [SerializeField] private TextMeshProUGUI qteButtonText;
        [SerializeField] private GameObject timingEventHolder;

        [Header("TimingEvent variables: ")]
        [SerializeField] private float allowedTimingOffset;

        [SerializeField] private float shrinkSpeed;
        private float ringTargetScale;
        private Vector3 ringStartScale;

        [SerializeField] private Color ringSuccessColor;
        [SerializeField] private Color ringFailureColor;
        [SerializeField] private Color ringNormalColor;

        private EButton currentButtonToPress;

        private void Start()
        {
            qteController = GetComponent<QTEController>();

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

            if (movingRing.transform.localScale.x <= 0.03)
                StopShrinkingRing();
        }

        [ContextMenu("ShrinkRing")]
        public override void StartQTE()
        {
            QTEFinishedSuccessfully = false;

            targetRing.color = ringNormalColor;

            currentButtonToPress = (EButton)Random.Range(0, 4);
            qteButtonText.text = qteController.DisplayButtonToPress(currentButtonToPress);

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
            }
        }
    }
}