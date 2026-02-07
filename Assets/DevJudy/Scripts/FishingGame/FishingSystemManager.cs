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
        [SerializeField] public int PlayerIndex; // !! Get from LevelService or PlayerJoiner or smth

        [Header("Scripts: ")]
        [SerializeField] public QTEController QTEController;

        [SerializeField] public QTEDisplayService QTEDisplayService;
        [SerializeField] private FishDisplay fishDisplay;
        [SerializeField] private IconHandler iconHandler;
        private FishingRodController fishingRodController;
        private CatchEventHandler catchEventHandler;

        [Header("GameObjects: ")]
        [SerializeField] private GameScoreSO gameScore;

        [SerializeField] private TextMeshProUGUI pointsText;
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

        public void SetUpFishDisplay(List<SO_Fish> _fishList)
        {
            if (fishDisplay != null)
            {
                fishDisplay.enabled = true;

                fishDisplay.SetupFishDisplay();
                
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
            iconHandler?.DisplayIcon(_emotion);
        }

        public void StartFishing()
        {
            FishingSystem.Instance.StartFishing(this);
        }

        public void StartFishEvent(SO_Fish _caughtFish)
        {
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
            FishingSystem.Instance.PressedCatch = true;

            iconHandler?.DisplayIcon(EEmotion.Happy);
        }

        private void StopFishing()
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

        public void UpdatePoints(UIPointsService _pointsService, int _points)
        {
            gameScore.Value += _points;
            _pointsService?.UpdatePointsText(pointsText, gameScore.Value);
        }
    }
}