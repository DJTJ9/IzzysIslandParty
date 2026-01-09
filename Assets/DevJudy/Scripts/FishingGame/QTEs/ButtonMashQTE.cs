using System.Collections;
using enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMashQTE : MonoBehaviour, IQuickTimeEvent
{
    private QTEController qteController;
    
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

    private bool qteRunning;
    private bool qteFinishedSuccessfully;

    public bool QTERunning => qteRunning;

    public bool QTEFinishedSuccessfully => qteFinishedSuccessfully;

    private EButton currentButtonToPress;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        qteController = GetComponent<QTEController>();
        
        ButtonMashEventSetup();
    }

    private void ButtonMashEventSetup()
    {
        mashButtonTransform = mashButtonImage.transform;
        normalButtonScale = mashButtonTransform.localScale;

        mashButtonImage.color = normalButtonColor;

        mashEventHolder.gameObject.SetActive(false);
    }
    
    [ContextMenu("ButtonMashEvent")]
    public void StartQTE()
    {
        mashButtonImage.gameObject.transform.localScale = normalButtonScale;

        qteFinishedSuccessfully = false;
        buttonMashCounter = 0;

        currentButtonToPress = (EButton)Random.Range(0, 4);
        mashButtonText.text = qteController.DisplayButtonToPress(currentButtonToPress);

        qteController.CurrentQuickTimeEvent = EQuickTimeEvent.ButtonMash;
        
        mashEventHolder.gameObject.SetActive(true);

        qteRunning = true;

        if (buttonMashCoroutine == null)
            buttonMashCoroutine = StartCoroutine(MashButtonsTimerCoroutine());
    }

    public void CheckButtonMashSuccess(EButton _buttonPressed)
    {
        if (!qteRunning)
            return;

        if (_buttonPressed != currentButtonToPress)
            OnWrongButtonMash();
        else
            OnButtonMash();

        if (buttonMashCounter >= buttonMashAmount)
        {
            qteFinishedSuccessfully = true;
            
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
        qteRunning = false;
        
        if (buttonMashCoroutine != null)
        {
            StopCoroutine(buttonMashCoroutine);
            buttonMashCoroutine = null;
        }

        if (qteFinishedSuccessfully)
            Debug.Log("Bm succeeded :)");
        else
            Debug.Log("Bm failed :(");

        // maybe also show failed color for 0.5 seconds...

        mashEventHolder.gameObject.SetActive(false);
        
        qteController.CurrentQuickTimeEvent = EQuickTimeEvent.None;
        Debug.Log("Its over!");
    }

    private IEnumerator MashButtonsTimerCoroutine()
    {
        while (qteRunning)
        {
            yield return new WaitForSeconds(buttonMashTime);
            
            qteRunning = false;
        }
        StopButtonMashEvent();
        
        yield return null;
    }

}
