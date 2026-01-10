using System.Collections;
using System.Collections.Generic;
using enums;
using FishingGame.QuickTimeEvents;
using ScriptableObjects;
using UnityEngine;

namespace FishingGame
{
    public class CatchEventHandler : MonoBehaviour
    {
        private TimingQTE timingQTE;
        private ButtonMashQTE buttonMashQTE;
        private BarQTE barQTE;

        private Coroutine qteCoroutine;

        private int quickTimeEventCounter;
        private bool failedEncounter;

        public bool CatchEventSuccess { get; private set; }
        public bool CatchEventFinished { get; private set; }


        private void Start()
        {
            barQTE = GetComponent<BarQTE>();
            if (barQTE == null)
                Debug.LogWarning("No BarQTE script attached to " + gameObject.name);

            buttonMashQTE = GetComponent<ButtonMashQTE>();
            if (buttonMashQTE == null)
                Debug.LogWarning("No ButtonMashQTE script attached to " + gameObject.name);

            timingQTE = GetComponent<TimingQTE>();
            if (timingQTE == null)
                Debug.LogWarning("No TimingQTE script attached to " + gameObject.name);
        }
        
        public void StartFishEvent(SO_Fish _fishToCatch)
        {
            ResetCatchEventVariables();
            
            StartCoroutine(CatchEventCoroutine(_fishToCatch.CatchEvents));
        }

        private void ResetCatchEventVariables()
        {
            CatchEventFinished = false;
            CatchEventSuccess = false;
            failedEncounter = false;

            quickTimeEventCounter = 0;
        }
        
        private IEnumerator CatchEventCoroutine(List<EQuickTimeEvent> _quickTimeEvent)
        {
            CatchEventSuccess = false;

            while (quickTimeEventCounter < _quickTimeEvent.Count && !failedEncounter)
            {
                // Go through list of unityEvents and invoke those instead of repeating the same one
                if (qteCoroutine == null)
                {
                    qteCoroutine = StartCoroutine(QTECoroutine(GetQuickTimeEvent(_quickTimeEvent[quickTimeEventCounter])));
                    quickTimeEventCounter++;
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => qteCoroutine == null);

            if (!failedEncounter)
                CatchEventSuccess = true;

            CatchEventFinished = true;

            yield return null;
        }

        private QuickTimeEvent GetQuickTimeEvent(EQuickTimeEvent _quickTimeEvent)
        {
            switch (_quickTimeEvent)
            {
                case EQuickTimeEvent.Timing:
                    return timingQTE;
                case EQuickTimeEvent.ButtonMash:
                    return buttonMashQTE;
                case EQuickTimeEvent.Bar:
                    return barQTE;
                default:
                    return null;
            }
        }

        private IEnumerator QTECoroutine(QuickTimeEvent _quickTimeEvent)
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