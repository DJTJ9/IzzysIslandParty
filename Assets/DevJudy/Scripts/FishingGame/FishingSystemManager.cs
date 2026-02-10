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
        private int playerIndex;

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
        public bool FishDisplayActive { get; set; }

        private void Awake()
        {
            fishingRodController = GetComponent<FishingRodController>();

            catchEventHandler = GetComponentInChildren<CatchEventHandler>();
            if (catchEventHandler == null)
                Debug.LogError("FishingSystemManager is missing catch EventHandler");
        }

        public void OnPlayerJoined(GameScoreSO _gameScore, int _playerIndex)
        {
            gameScore = _gameScore;
            playerIndex = _playerIndex;

            gameScore.Value = 0;
        }

        public void SetUpFishDisplay(List<SO_Fish> _fishList)
        {
            if (playerIndex == 0)
                fishDisplay.ClearPrefabReferences(_fishList);

            if (fishDisplay != null)
            {
                fishDisplay.enabled = true;

                fishDisplay.SetupFishDisplay(playerIndex);

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
            fishDisplay?.DisplayFish(_caughtFish, playerIndex);
        }

        public void DisplayIcon(EEmotion _emotion)
        {
            iconHandler?.DisplayIcon(_emotion);
        }

        public void StartFishing()
        {
            FishingSystem.Instance.StartFishing(this);
        }

        public void StartFishEvent(SO_Fish _caughtFish)
        {
            catchEventHandler.StartFishEvent(_caughtFish, QTEController, QTEDisplayService, playerIndex);
        }

        public bool CheckIfCatchEventFinished()
        {
            return catchEventHandler.CatchEventVariables[playerIndex].CatchEventFinished;
        }

        public bool CheckIfCatchEventSucceeded()
        {
            return catchEventHandler.CatchEventVariables[playerIndex].CatchEventSuccess;
        }

        public void PressedCatch()
        {
            if (!FishingSystem.Instance.PressedCatch)
                iconHandler?.DisplayIcon(EEmotion.Happy);

            FishingSystem.Instance.PressedCatch = true;
        }

        public void StopFishing()
        {
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