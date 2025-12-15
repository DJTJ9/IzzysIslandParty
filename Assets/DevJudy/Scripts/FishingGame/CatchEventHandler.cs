using System.Collections;
using enums;
using FishingGame.QuickTimeEvent;
using UnityEngine;

namespace FishingGame
{
    public class CatchEventHandler : MonoBehaviour
    {
        // For debug:
        [SerializeField] private BarQTE barQTE;
        
        // For all:
        [SerializeField] private FishingRodController fishingController;
        private Coroutine flounderEventCoroutine;
        private Coroutine mackerelEventCoroutine;
        private Coroutine welsEventCoroutine;
        private Coroutine timingQTECoroutine;
        private Coroutine buttonMashCoroutine;
        private Coroutine barQTECoroutine;

        // Do I really need all these bools??
        private bool caughtFish = false;
        private bool failedEncounter = false;
        public bool CatchEventSuccess { get; private set; }
        public bool CatchEventFinished { get; private set; }

        // For flounder: 

        [SerializeField] private QTEHandler qteHandler;

        private int quickTimeEventCounter = 0;
        
        private int numberOfEventsToPerform = 3; // (to be got from flounder)

        public void StartFishEvent(EFish _fishToCatch)
        {
            CatchEventFinished = false;
            CatchEventSuccess = false;
            failedEncounter = false;

            quickTimeEventCounter = 0;

            int random = Random.Range(0, 3);

            Debug.Log("ChoseRandomEvent: " + random);

            switch (_fishToCatch)
            {
                case EFish.Wels:
                case EFish.Salmon:
                    WelsEvent();
                    break;
                
                case EFish.RainbowTrout:
                case EFish.Flounder:
                    FlounderEvent();
                    break;
                
                case EFish.Mackerel:
                case EFish.Sturgeon:
                    MackerelEvent();
                    break;
            }
            
           if (random == 0)
               FlounderEvent();
           else if (random == 1)
               MackerelEvent();
           else
                WelsEvent();
        }

        public void FlounderEvent()
        {
            if (flounderEventCoroutine == null)
                flounderEventCoroutine = StartCoroutine(FlounderCoroutine());
        }

        private IEnumerator FlounderCoroutine()
        {
            while (quickTimeEventCounter < numberOfEventsToPerform && !failedEncounter)
            {
                if (timingQTECoroutine == null)
                {
                    timingQTECoroutine = StartCoroutine(TimingQTE());
                    quickTimeEventCounter++;
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => timingQTECoroutine == null);

            if (!failedEncounter)
                CatchEventSuccess = true;

            CatchEventFinished = true;

            flounderEventCoroutine = null;
            yield return null;
        }

        // Do i need both the caughtFish and the failedEncounter
        private IEnumerator TimingQTE()
        {
            qteHandler.StartShrinkingRingQTE();

            while (qteHandler.TimingEventRunning)
            {
                yield return new WaitForEndOfFrame();
            }

            if (qteHandler.TimingEventSuccessful)
            {
                // Keep going until amountOfQuickTimeEvents is finished
                if (quickTimeEventCounter >= numberOfEventsToPerform)
                    caughtFish = true;
            }
            else
            {
                failedEncounter = true;
                caughtFish = false;
            }

            timingQTECoroutine = null;
            yield return null;
        }

        // ---- These are super similar, can I just use reuse them and give them vars? ----
        public void MackerelEvent()
        {
            if (mackerelEventCoroutine == null)
                mackerelEventCoroutine = StartCoroutine(MackerelCoroutine());
        }

        private IEnumerator MackerelCoroutine()
        {
            while (quickTimeEventCounter < numberOfEventsToPerform && !failedEncounter)
            {
                if (buttonMashCoroutine == null)
                {
                    buttonMashCoroutine = StartCoroutine(ButtonMashQTE());
                    quickTimeEventCounter++;
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => buttonMashCoroutine == null);

            if (!failedEncounter)
                CatchEventSuccess = true;

            CatchEventFinished = true;

            mackerelEventCoroutine = null;
            yield return null;
        }

        // Do i need both the caughtFish and the failedEncounter
        private IEnumerator ButtonMashQTE()
        {
            qteHandler.StartButtonMashQTE();

            while (qteHandler.ButtonMashEventRunning)
            {
                yield return new WaitForEndOfFrame();
            }

            if (qteHandler.ButtonMashEventSuccessful)
            {
                // Keep going until amountOfQuickTimeEvents is finished
                if (quickTimeEventCounter >= numberOfEventsToPerform)
                    caughtFish = true;
            }
            else
            {
                failedEncounter = true;
                caughtFish = false;
            }

            buttonMashCoroutine = null;
            yield return null;
        }
        
        // ----- bar ----
        
        public void WelsEvent()
        {
            if (welsEventCoroutine == null)
                welsEventCoroutine = StartCoroutine(WelsCoroutine());
        }

        private IEnumerator WelsCoroutine()
        {
            while (!failedEncounter && !caughtFish)
            {
                if (barQTECoroutine == null)
                {
                    barQTECoroutine = StartCoroutine(BarQTE());
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => barQTECoroutine == null);

            if (!failedEncounter)
                CatchEventSuccess = true;

            Debug.Log("WelsEvent over ");
            CatchEventFinished = true;

            welsEventCoroutine = null;
            yield return null;
        }

        private IEnumerator BarQTE()
        {
            Debug.Log("Started coroutine");
           barQTE.StartBarQTE();

            while (barQTE.BarQTERunning)
            {
                yield return new WaitForEndOfFrame();
            }

            if (barQTE.BarQTESuccessful)
            {
                Debug.Log("BarQTE successful");
                    caughtFish = true;
            }
            else
            {
                Debug.Log("BarQTE not successful");

                failedEncounter = true;
                caughtFish = false;
            }

            barQTECoroutine = null;
            yield return null;
        }
    }
}