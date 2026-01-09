using System.Collections;
using enums;
using FishingGame.QuickTimeEvent;
using UnityEngine;

namespace FishingGame
{
    public class CatchEventHandler : MonoBehaviour
    {
        // For debug:
        private TimingQTE timingQTE;
        private ButtonMashQTE buttonMashQTE;
        private BarQTE barQTE;
        
        private Coroutine catchEventsCoroutine;
        private Coroutine qteCoroutine;
        
        private int quickTimeEventCounter;
        private int numberOfEventsToPerform = 3; // (to be got from fish
        
        private bool failedEncounter;
        public bool CatchEventSuccess { get; private set; }
        public bool CatchEventFinished { get; private set; }
        

        // !! Eigentliche idee geht so nicht, also maybe liste von unityEvents/IQuickTimeEvents

        private void Start()
        {
            barQTE = GetComponent<BarQTE>();
            buttonMashQTE = GetComponent<ButtonMashQTE>();
            timingQTE = GetComponent<TimingQTE>();
        }

        // Currently qtes are endless
        public void StartFishEvent(So_Fish _fishToCatch)
        {
            CatchEventFinished = false;
            CatchEventSuccess = false;
            failedEncounter = false;

            quickTimeEventCounter = 0;

            // int random = Random.Range(0, 3);

            Debug.Log("Invoking event for : " + _fishToCatch);

            switch (_fishToCatch.FishType)
            {
                case EFish.RainbowTrout:
                case EFish.Flounder:
                    InvokeEvents(timingQTE);
                    break;

                case EFish.Wels:
                case EFish.Salmon:
                    InvokeEvents(buttonMashQTE);
                    break;

                case EFish.Mackerel:
                case EFish.Sturgeon:
                    InvokeEvents(barQTE);
                    break;
            }
        }

        private void InvokeEvents(IQuickTimeEvent _qteToInvoke)
        {
            if (catchEventsCoroutine == null)
                catchEventsCoroutine = StartCoroutine(CatchEventsCoroutine(_qteToInvoke));
        }

        // Blueprint
        // !! Should prolly have a list of the unityEvents/IQuickTimeEvents then
        private IEnumerator CatchEventsCoroutine(IQuickTimeEvent _quickTimeEvent)
        {
            CatchEventSuccess = false;

            while (quickTimeEventCounter < numberOfEventsToPerform && !failedEncounter)
            {
                // Go through list of unityEvents and invoke those instead of repeating the same one
                if (qteCoroutine == null)
                {
                    qteCoroutine = StartCoroutine(QTECoroutine(_quickTimeEvent)); // list[i].Invoke
                    quickTimeEventCounter = ReferenceEquals(_quickTimeEvent, barQTE) ? numberOfEventsToPerform : quickTimeEventCounter + 1;
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => qteCoroutine == null);

            if (!failedEncounter)
                CatchEventSuccess = true;

            CatchEventFinished = true;

            catchEventsCoroutine = null;
            yield return null;
        }

        private IEnumerator QTECoroutine(IQuickTimeEvent _quickTimeEvent)
        {
            _quickTimeEvent.StartQTE();

            while (_quickTimeEvent.QTERunning)
            {
                yield return new WaitForEndOfFrame();
            }

            if (!_quickTimeEvent.QTEFinishedSuccessfully)
                failedEncounter = true;

            qteCoroutine = null;
            yield return null;
        }
    }
}