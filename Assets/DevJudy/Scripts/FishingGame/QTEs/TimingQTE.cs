using enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimingQTE : MonoBehaviour, IQuickTimeEvent
{
    private QTEController qteController;

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

    private bool qteRunning;
    private bool qteFinishedSuccessfully;

    public bool QTERunning => qteRunning;

    public bool QTEFinishedSuccessfully => qteFinishedSuccessfully;

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
    public void StartQTE()
    {
        qteFinishedSuccessfully = false;

        targetRing.color = ringNormalColor;

        currentButtonToPress = (EButton)Random.Range(0, 4);
        qteButtonText.text = qteController.DisplayButtonToPress(currentButtonToPress);

        qteController.CurrentQuickTimeEvent = EQuickTimeEvent.ButtonMash;

        timingEventHolder.gameObject.SetActive(true);

        qteRunning = true;
    }

    private void ShrinkRing()
    {
        movingRing.transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed) * Time.deltaTime;
    }

    private void StopShrinkingRing()
    {
        timingEventHolder.gameObject.SetActive(false);

        qteRunning = false;
        movingRing.transform.localScale = ringStartScale;
    }

    public void CheckTimingSuccess(EButton _buttonPressed)
    {
        float ringScale = movingRing.transform.lossyScale.x;

        if (ringTargetScale + allowedTimingOffset >= ringScale && _buttonPressed == currentButtonToPress)
        {
            targetRing.color = ringSuccessColor;
            qteFinishedSuccessfully = true;
        }
        else
        {
            targetRing.color = ringFailureColor;
        }
    }
}