using System.Collections;
using System.Collections.Generic;
using enums;
using FishingGame.QuickTimeEvents;
using ScriptableObjects;
using UnityEngine;

namespace FishingGame
{
    public class CatchEventVariables
    {
        public Coroutine QTECoroutine;
        public int QTECounter;
        public bool FailedEncounter;
        public bool CatchEventSuccess;
        public bool CatchEventFinished;
    }

    public class CatchEventHandler : MonoBehaviour
    {
        private const int maxNumberOfPlayers = 4;

        private TimingQTE timingQTE;
        private ButtonMashQTE buttonMashQTE;
        private BarQTE barQTE;
        private QuickTimeEvent currentQTE;

        public readonly List<CatchEventVariables> CatchEventVariables = new List<CatchEventVariables>();

        private void Start()
        {
            for (int i = 0; i < maxNumberOfPlayers; i++)
            {
                CatchEventVariables.Add(new CatchEventVariables());
            }

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

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void StartFishEvent(SO_Fish _fishToCatch, QTEController _qteController, QTEDisplayService _qteDisplayService, int _playerIndex)
        {
            ResetCatchEventVariables(_playerIndex);

            StartCoroutine(CatchEventCoroutine(_fishToCatch.CatchEvents, _playerIndex, _qteController, _qteDisplayService));
        }

        private void ResetCatchEventVariables(int _playerIndex)
        {
            CatchEventVariables[_playerIndex].CatchEventFinished = false;
            CatchEventVariables[_playerIndex].CatchEventSuccess = false;
            CatchEventVariables[_playerIndex].FailedEncounter = false;

            CatchEventVariables[_playerIndex].QTECounter = 0;
        }

        private IEnumerator CatchEventCoroutine(List<EQuickTimeEvent> _quickTimeEvent, int _playerIndex, QTEController _qteController,
            QTEDisplayService _qteDisplayService)
        {
            CatchEventVariables[_playerIndex].CatchEventSuccess = false;

            while (CatchEventVariables[_playerIndex].QTECounter < _quickTimeEvent.Count && !CatchEventVariables[_playerIndex].FailedEncounter)
            {
                // Go through list of unityEvents and invoke those instead of repeating the same one
                if (CatchEventVariables[_playerIndex].QTECoroutine == null)
                {
                    CatchEventVariables[_playerIndex].QTECoroutine = StartCoroutine(
                        QTECoroutine(GetQuickTimeEvent(_quickTimeEvent[CatchEventVariables[_playerIndex].QTECounter]), _playerIndex));

                    CatchEventVariables[_playerIndex].QTECounter++;
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitUntil(() => CatchEventVariables[_playerIndex].QTECoroutine == null);

            if (!CatchEventVariables[_playerIndex].FailedEncounter)
                CatchEventVariables[_playerIndex].CatchEventSuccess = true;

            CatchEventVariables[_playerIndex].CatchEventFinished = true;

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

        private IEnumerator QTECoroutine(QuickTimeEvent _quickTimeEvent, int _playerIndex)
        {
            currentQTE = _quickTimeEvent;
            currentQTE.StartQTE();

            while (currentQTE && currentQTE.QTERunning)
            {
                yield return new WaitForEndOfFrame();
            }

            if (!currentQTE || !currentQTE.QTEFinishedSuccessfully)
                CatchEventVariables[_playerIndex].FailedEncounter = true;

            CatchEventVariables[_playerIndex].QTECoroutine = null;
            currentQTE = null;

            yield return null;
        }

        public void StopQTE(int _playerIndex)
        {
            if (!currentQTE)
                return;

            currentQTE.StopQTE();
            CatchEventVariables[_playerIndex].FailedEncounter = true;
            CatchEventVariables[_playerIndex].QTECoroutine = null;

            currentQTE = null;
        }
    }
}