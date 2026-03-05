using System.Collections.Generic;
using enums;
using FishingGame.Display;
using FishingGame.QuickTimeEvents;
using Juice;
using ScriptableObjects;
using TMPro;
using UIScripts;
using UnityEngine;

namespace FishingGame
{
    public class FishingSystemManager : MonoBehaviour
    {
        public int PlayerIndex { get; private set; }

        [Header("Scripts: ")]
        [SerializeField] public QTEController QTEController;
        [SerializeField] public QTEDisplayService QTEDisplayService;
        [SerializeField] private FishDisplay fishDisplay;
        [SerializeField] private IconHandler iconHandler;
        [SerializeField] private UIPointsService pointsService;
        private FishingRodController fishingRodController;
        private CatchEventHandler catchEventHandler;

        [Header("GameObjects: ")]
        [SerializeField] private TextMeshProUGUI pointsText;

        private GameScoreSO gameScore;
        public Coroutine FishingRoutine;

        public bool FishHooked { get; set; }
        public bool InQTE { get; set; }
        public bool FishDisplayActive { get; set; }

        private void Awake()
        {
            fishingRodController = GetComponent<FishingRodController>();

            catchEventHandler = GetComponentInChildren<CatchEventHandler>();
            if (catchEventHandler == null)
                Debug.LogError("FishingSystemManager is missing catch EventHandler");
        }

        private void Start()
        {
            if (PlayerIndex == 0)
            {
                ActivateFishingSystem();
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void ActivateFishingSystem()
        {
            FishingSystem.Instance.gameObject.SetActive(true);
            FishingSystem.Instance.enabled = true;
        }

        public void OnPlayerJoined(GameScoreSO _gameScore, int _playerIndex)
        {
            gameScore = _gameScore;
            PlayerIndex = _playerIndex;

            gameScore.Value = 0;
        }

        public void SetUpFishDisplay(List<SO_Fish> _fishList)
        {
            if (PlayerIndex == 0)
                fishDisplay.ClearPrefabReferences(_fishList);

            if (fishDisplay != null)
            {
                fishDisplay.enabled = true;

                fishDisplay.SetupFishDisplay(PlayerIndex);

                SpawnInFishDisplayObjects(_fishList);
                StopFishDisplay();
            }
        }

        private void SpawnInFishDisplayObjects(List<SO_Fish> _fishList)
        {
            for (int i = 0; i < _fishList.Count; i++)
            {
                _fishList[i].PrefabReferences.Add(fishDisplay.SpawnInFishPrefabs(_fishList[i]));
            }
        }

        public void DisplayFish(SO_Fish _caughtFish)
        {
            fishDisplay?.DisplayFish(_caughtFish, PlayerIndex);
        }

        public void DisplayIcon(EEmotion _emotion)
        {
            iconHandler.DisplayIcon(_emotion);
        }

        public void StartFishing()
        {
            if (!FishingSystem.Instance.enabled || !FishingSystem.Instance.gameObject.activeInHierarchy)
                ActivateFishingSystem();

            FishingSystem.Instance.StartFishing(this);
        }

        public void StartFishEvent(SO_Fish _caughtFish)
        {
            InQTE = true;
            
            catchEventHandler.StartFishEvent(_caughtFish, QTEController, QTEDisplayService, PlayerIndex);
        }

        public bool CheckIfCatchEventFinished()
        {
            return catchEventHandler.CatchEventVariables[PlayerIndex].CatchEventFinished;
        }

        public bool CheckIfCatchEventSucceeded()
        {
            return catchEventHandler.CatchEventVariables[PlayerIndex].CatchEventSuccess;
        }

        public void PressedCatch()
        {
            if (!FishingSystem.Instance.PressedCatch[PlayerIndex])
                iconHandler.DisplayIcon(EEmotion.Happy);

            FishingSystem.Instance.PressedCatch[PlayerIndex] = true;
        }

        public void StopFishing()
        {
            InQTE = false;
            
            catchEventHandler.StopQTE(PlayerIndex);
            FishingSystem.Instance.StopFishing(this);
        }

        public void StopFishDisplay()
        {
            if (!FishDisplayActive)
                return;

            fishDisplay?.StopDisplayFish();

            FishDisplayActive = false;
        }

        public void PlayFishBitingAnimation(bool _withIcon)
        {
            if (_withIcon)
                DisplayIcon(EEmotion.Alert);

            fishingRodController.PlayFishBitingAnimation();
        }

        public void StopFishBitingAnimation()
        {
            fishingRodController.StopFishBitingAnimation();
        }

        public void PullBackFishingRod(bool _stopFishingRoutine)
        {
            if (_stopFishingRoutine)
                StopFishing();

            fishingRodController.PullBackFishingRod();
        }

        public void UpdatePoints(int _points)
        {
            gameScore.Value += _points;
            pointsService?.UpdatePointsText(pointsText, gameScore.Value);
        }
    }
}