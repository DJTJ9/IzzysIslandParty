using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using enums;

public class QTEHandler : MonoBehaviour
{
    #region constants

    private const float totalHeight = 240;
    private const float totalWidth = 490;

    private const string northButtonKB = "W";
    private const string eastButtonKB = "D";
    private const string southButtonKB = "S";
    private const string westButtonKB = "A";

    private const string northButtonCTRL = "Δ";
    private const string eastButtonCTRL = "O";
    private const string southButtonCTRL = "X";
    private const string westButtonCTRL = "☐";

    #endregion
    
    #region TimingEvent

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

    private Vector2 ringYAxisRange = new Vector2(-10f, 4f);
    private Vector2 ringXAxisRange = new Vector2(-29f, 531f);

    [SerializeField] private Color ringSuccessColor;
    [SerializeField] private Color ringFailureColor;
    [SerializeField] private Color ringNormalColor;

    public bool TimingEventRunning { get; private set; }
    public bool TimingEventSuccessful { get; private set; }

    #endregion

    #region MashEvent

    [Header("MashEvent needed components: ")]
    [SerializeField] private TextMeshProUGUI mashButtonText;
    [SerializeField] private GameObject mashEventHolder;
    [SerializeField] private Image mashButtonImage;
    private Transform mashButtonTransform;
    private Coroutine buttonMashCoroutine;

    [Header("MashEvent needed components: ")]
    [SerializeField] private Color normalButtonColor;
    [SerializeField] private Color buttonPressedColor;
    [SerializeField] private int buttonMashAmount;
    [SerializeField] private float buttonMashTime;
    private float buttonMashCounter = 0f;
    private Vector3 normalButtonScale;

    public bool ButtonMashEventSuccessful { get; private set; }
    public bool ButtonMashEventRunning { get; private set; }

    #endregion

    #region BarEvent
    #endregion
    
    private EButton currentButtonToPress;
    private EQuickTimeEvent currentQuickTimeEvent;

    
    private void Start()
    {
        TimingEventSetup();

        MashEventSetup();

        currentQuickTimeEvent = EQuickTimeEvent.None;
    }

    private void TimingEventSetup()
    {
        ringStartScale = movingRing.transform.localScale;

        ringTargetScale = targetRing.gameObject.transform.lossyScale.x;

        timingEventHolder.gameObject.SetActive(false);
    }

    private void MashEventSetup()
    {
        mashButtonTransform = mashButtonImage.transform;
        normalButtonScale = mashButtonTransform.localScale;

        mashButtonImage.color = normalButtonColor;

        mashEventHolder.gameObject.SetActive(false);
    }

    public void OnNorthButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.performed && currentQuickTimeEvent != EQuickTimeEvent.None)
        {
            switch (currentQuickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    CheckTimingSuccess(EButton.NorthButton);
                    break;
                case EQuickTimeEvent.ButtonMash:
                    CheckButtonMashSuccess(EButton.NorthButton);
                    break;
                case EQuickTimeEvent.Scale:
                    break;
            }
        }
    }


    public void OnEastButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.performed && currentQuickTimeEvent != EQuickTimeEvent.None)
        {
            switch (currentQuickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    CheckTimingSuccess(EButton.EastButton);
                    break;
                case EQuickTimeEvent.ButtonMash:
                    CheckButtonMashSuccess(EButton.EastButton);
                    break;
                case EQuickTimeEvent.Scale:
                    break;
            }
        }
    }

    public void OnSouthButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.performed && currentQuickTimeEvent != EQuickTimeEvent.None)
        {
            switch (currentQuickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    CheckTimingSuccess(EButton.SouthButton);
                    break;
                case EQuickTimeEvent.ButtonMash:
                    CheckButtonMashSuccess(EButton.SouthButton);
                    break;
                case EQuickTimeEvent.Scale:
                    break;
            }
        }
    }

    public void OnWestButtonPressed(InputAction.CallbackContext _context)
    {
        if (_context.performed && currentQuickTimeEvent != EQuickTimeEvent.None)
        {
            switch (currentQuickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    CheckTimingSuccess(EButton.WestButton);
                    break;
                case EQuickTimeEvent.ButtonMash:
                    CheckButtonMashSuccess(EButton.WestButton);
                    break;
                case EQuickTimeEvent.Scale:
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (TimingEventRunning)
            ShrinkRing();

        if (movingRing.transform.localScale.x <= 0.03)
            StopShrinkingRing();
    }

    [ContextMenu("ShrinkRing")]
    public void StartShrinkingRingQTE()
    {
        TimingEventSuccessful = false;

        // TODO: Figure out how to change pos

        targetRing.color = ringNormalColor;

        currentButtonToPress = (EButton)Random.Range(0, 4);
        qteButtonText.text = DisplayButtonToPress(currentButtonToPress);

        timingEventHolder.gameObject.SetActive(true);

        TimingEventRunning = true;
        currentQuickTimeEvent = EQuickTimeEvent.Timing;
    }

    private void ShrinkRing()
    {
        movingRing.transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, shrinkSpeed) * Time.deltaTime;
    }

    private void StopShrinkingRing()
    {
        timingEventHolder.gameObject.SetActive(false);

        TimingEventRunning = false;
        movingRing.transform.localScale = ringStartScale;

        currentQuickTimeEvent = EQuickTimeEvent.None;
    }

    private void CheckTimingSuccess(EButton _buttonPressed)
    {
        float ringScale = movingRing.transform.lossyScale.x;

        if (ringTargetScale + allowedTimingOffset >= ringScale && _buttonPressed == currentButtonToPress)
        {
            targetRing.color = ringSuccessColor;
            TimingEventSuccessful = true;
        }
        else
        {
            targetRing.color = ringFailureColor;
        }
    }

    // ----------- Button Mash Event -------------
    [ContextMenu("ButtonMashEvent")]
    public void StartButtonMashQTE()
    {
        mashButtonImage.gameObject.transform.localScale = normalButtonScale;

        ButtonMashEventSuccessful = false;
        buttonMashCounter = 0;

        currentButtonToPress = (EButton)Random.Range(0, 4);
        mashButtonText.text = DisplayButtonToPress(currentButtonToPress);

        mashEventHolder.gameObject.SetActive(true);

        ButtonMashEventRunning = true;
        currentQuickTimeEvent = EQuickTimeEvent.ButtonMash;

        if (buttonMashCoroutine == null)
            buttonMashCoroutine = StartCoroutine(MashButtonsTimerCoroutine());
    }

    private void CheckButtonMashSuccess(EButton _buttonPressed)
    {
        if (!ButtonMashEventRunning)
            return;

        if (_buttonPressed != currentButtonToPress)
            OnWrongButtonMash();
        else
            OnButtonMash();

        if (buttonMashCounter >= buttonMashAmount)
        {
            ButtonMashEventSuccessful = true;
            
            StopButtonMashEvent();
            return;
        }
        
        if (buttonMashCounter <= 0)
            StopButtonMashEvent();
    }

    private void OnWrongButtonMash()
    {
        buttonMashCounter--;
        
        float buttonTransform = (buttonMashAmount - buttonMashCounter) * 10;

        if (buttonTransform != 0)
            buttonTransform = (buttonTransform / 400) + 0.025f;

        mashButtonTransform.localScale = new Vector3(buttonTransform, buttonTransform, normalButtonScale.z);
    }

    private void OnButtonMash()
    {
        buttonMashCounter++;

        // Show mashed color for like 0.5 seconds...

        float buttonTransform = (buttonMashAmount - buttonMashCounter) * 10;

        if (buttonTransform != 0)
            buttonTransform = (buttonTransform / 400) + 0.025f;

        mashButtonTransform.localScale = new Vector3(buttonTransform, buttonTransform, normalButtonScale.z);
    }

    private void StopButtonMashEvent()
    {
        ButtonMashEventRunning = false;
        
        if (buttonMashCoroutine != null)
        {
            StopCoroutine(buttonMashCoroutine);
            buttonMashCoroutine = null;
        }

        if (!ButtonMashEventSuccessful)
            Debug.Log("Bm failed :(");
        else
            Debug.Log("Bm succeeded :)");

        // maybe also show failed color for 0.5 seconds...

        mashEventHolder.gameObject.SetActive(false);
        
        currentQuickTimeEvent = EQuickTimeEvent.None;
    }

    private IEnumerator MashButtonsTimerCoroutine()
    {
        while (ButtonMashEventRunning)
        {
            yield return new WaitForSeconds(buttonMashTime);
            
            ButtonMashEventRunning = false;
        }
        StopButtonMashEvent();
        
        yield return null;
    }

    private string DisplayButtonToPress(EButton _buttonToPress)
    {
        switch (_buttonToPress)
        {
            case EButton.NorthButton:
                return northButtonKB;
            case EButton.EastButton:
                return eastButtonKB;
            case EButton.SouthButton:
                return southButtonKB;
            case EButton.WestButton:
                return westButtonKB;
        }

        // This should never return anything but a valid button
        return "!";
    }
}