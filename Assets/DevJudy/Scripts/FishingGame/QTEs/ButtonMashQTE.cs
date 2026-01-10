using System.Collections;
using enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FishingGame.QuickTimeEvents
{
    public class ButtonMashQTE : QuickTimeEvent
    {
        [SerializeField] private QTEController qteController;

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
        private int buttonMashCounter = 0;
        private int falseButtonCounter = 0;
        private int maxFalseButtonPresses = 3;

        private Vector3 normalButtonScale;

        private EButton currentButtonToPress;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
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
        public override void StartQTE()
        {
            mashButtonImage.gameObject.transform.localScale = normalButtonScale;

            QTEFinishedSuccessfully = false;
            buttonMashCounter = 0;

            currentButtonToPress = (EButton)Random.Range(0, 4);
            mashButtonText.text = qteController.DisplayButtonToPress(currentButtonToPress);

            qteController.CurrentQuickTimeEvent = EQuickTimeEvent.ButtonMash;

            mashEventHolder.gameObject.SetActive(true);

            QTERunning = true;

            if (buttonMashCoroutine == null)
                buttonMashCoroutine = StartCoroutine(MashButtonsTimerCoroutine());
        }

        public void CheckButtonMashSuccess(EButton _buttonPressed)
        {
            if (!QTERunning)
                return;

            if (_buttonPressed != currentButtonToPress)
                OnWrongButtonMash();
            else
                OnButtonMash();

            if (buttonMashCounter >= buttonMashAmount)
            {
                QTEFinishedSuccessfully = true;

                StopButtonMashEvent();
                return;
            }

            if (falseButtonCounter >= maxFalseButtonPresses)
                StopButtonMashEvent();
        }

        private void OnWrongButtonMash()
        {
            falseButtonCounter++;
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
            QTERunning = false;

            if (buttonMashCoroutine != null)
            {
                StopCoroutine(buttonMashCoroutine);
                buttonMashCoroutine = null;
            }

            if (QTEFinishedSuccessfully)
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
            while (QTERunning)
            {
                yield return new WaitForSeconds(buttonMashTime);

                QTERunning = false;
            }

            StopButtonMashEvent();

            yield return null;
        }
    }
}
