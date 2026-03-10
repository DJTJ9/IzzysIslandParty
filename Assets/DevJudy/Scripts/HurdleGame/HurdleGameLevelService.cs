using System.Collections;
using Audio;
using HelperScripts;
using HurdleGame.Camera;
using MultiuseScripts;
using Player.Collections;
using TMPro;
using UIScripts;
using UnityEngine;

namespace HurdleGame.LevelService
{
    public class HurdleGameLevelService : RacingGameLevelService
    {
        #region consts

        private const string levelStartText = "START";
        private const string dotsText = "...";

        private const int maxNumberOfPlayers = 4;

        #endregion

        [Header("Dependencies: ")]
        [SerializeField] private SO_PlayerCollectionRacingGames currentPlayersRacing;
        [SerializeField] private CameraMoverAddition cameraMoverAddition;
        [SerializeField] private UIPanelManager uiPanelManager;
        [SerializeField] private GameAudioManager audioManager;
        
        [Header("Level start/end: ")]
        [SerializeField] private int secondsToStartLevel;
        [SerializeField] private Transform goalTransform;
        [SerializeField] private TextMeshProUGUI levelCountdownText;

        private int playerCount = 0;
        private int humanPlayerCount = 0;
        private int placement = 0;

        private bool raceStarted = false;
        private bool raceEnded = false;
        
        [Header("Temp: ")]
        [SerializeField] private TextMeshProUGUI onFinishLineCrossedText;
        [SerializeField] private bool checkXOnly;
        
        
        private void Start()
        {
           Cursor.visible = false;
           //Cursor.lockState = CursorLockMode.Locked;
            
            if (audioManager != null)
            {
                audioManager.gameObject.SetActive(true);
                audioManager.enabled = true;
                
                audioManager.StartBackgroundMusic(() => !raceEnded);
            }

            if (goalTransform == null)
            {
                Debug.LogWarning("Goal transform not set");
                checkPlacements = false;
            }
        }

        private void CheckPlacementList()
        {
            if (placementOrder.Length > maxNumberOfPlayers)
            {
                Debug.LogWarning($"PlacementOrder array is more than maximum number of players ({maxNumberOfPlayers}), resizing array");
                placementOrder = ArrayHelper.ResizeArray(placementOrder, maxNumberOfPlayers);
            }

            for (int i = placementOrder.Length - 1; i > 0; i--)
            {
                if (placementOrder[i] == null)
                {
                    Debug.LogWarning($"PlacementOrder[{i}] is empty, resizing array");
                    placementOrder = ArrayHelper.RemoveEmptySpotsFromArray(placementOrder);
                }
            }
        }

        public override void OnPlayerJoined(GameObject _player)
        {
            if (humanPlayerCount >= maxNumberOfPlayers)
                return;
            
            if (placementOrder == null || placementOrder.Length < 1)
                placementOrder = new GameObject[maxNumberOfPlayers];

            if (playerCount == 0)
                cameraMoverAddition.SetOrientationCharacter(_player.gameObject.GetComponent<CharacterMover>());

            if (placementOrder.Length == 1)
                cameraMoverAddition.SetOrientationCharacter(_player.GetComponent<CharacterMover>());

            ArrayHelper.AddToArray(placementOrder, _player);

            playerCount++;
            humanPlayerCount++;
            
            uiPanelManager.SetPlayers(humanPlayerCount);
        }

        public override void OnNPCJoined(GameObject _npc)
        {
            if (placementOrder.Length >= maxNumberOfPlayers)
                return;
            
            Debug.Log("Adding npc to array");
            ArrayHelper.AddToArray(placementOrder, _npc);
            playerCount++;
        }

        public override void StartLevel()
        {
            CheckPlacementList();

            StartCoroutine(CountdownToLevelStart());
        }

        private void OnCoroutineOver()
        {
            StopCoroutine(CountdownToLevelStart());

            OnLevelStart.Invoke();

            raceStarted = true;
        }

        private IEnumerator CountdownToLevelStart()
        {
            if (levelCountdownText == null)
                OnCoroutineOver();

            levelCountdownText.gameObject.SetActive(true);
            levelCountdownText.enabled = true;

            for (int i = secondsToStartLevel; i > 0; i--)
            {
                levelCountdownText.text = i + dotsText;

                yield return new WaitForSecondsRealtime(1f);
            }

            levelCountdownText.text = levelStartText;

            yield return new WaitForSecondsRealtime(1f);

            levelCountdownText.enabled = false;

            OnCoroutineOver();

            yield return null;
        }

        private void FixedUpdate()
        {
            if (raceStarted)
                CheckRacerPlacements();
        }

        private void CheckRacerPlacements()
        {
            for (int i = 1; i < placementOrder.Length; i++)
            {
                float currentDistanceToCompare = GetDistanceToGoal(placementOrder[i].transform.position);
                GameObject currentGameObjectBeingCompared = placementOrder[i];

                int leftNeighbour = i - 1;

                // While the leftNeighbour is not out of bounds, and the ln distance is more than the currentDistance
                while (leftNeighbour >= 0 && GetDistanceToGoal(placementOrder[leftNeighbour].transform.position) > currentDistanceToCompare)
                {
                    // Set the index [ln + 1] (first pass index of current key), to be the value of [ln]
                    placementOrder[leftNeighbour + 1] = placementOrder[leftNeighbour];
                    // Then go further down the array and look at the next ln
                    leftNeighbour = leftNeighbour - 1;
                }

                // If the distance is less, or all left neighbours have been checked, make sure to give the key value to the current index (ln + 1)
                placementOrder[leftNeighbour + 1] = currentGameObjectBeingCompared;
            }
        }

        private float GetDistanceToGoal(Vector3 _gameObjectPos)
        {
            var goalPos = new Vector2(goalTransform.position.x, checkXOnly ? _gameObjectPos.z : goalTransform.position.z);

            return Vector2.Distance(new Vector2(_gameObjectPos.x, _gameObjectPos.z), goalPos);
        }

        public override void OnFinishLineCrossed(GameObject _triggeringObj)
        {
            placement = (placement + 1);

           int triggeringObjPlayerIndex = GetTriggeringObjFromCurrentPlayers(_triggeringObj);
           
           currentPlayersRacing.Players[triggeringObjPlayerIndex].PlayerScore.Value = placement;
            
            if (((1 << _triggeringObj.layer) & playerLayerMask) != 0)
                OnPlayerCrossedFinishLine();
            
            if (placement >= maxNumberOfPlayers)
                EndLevel();
        }

        private int GetTriggeringObjFromCurrentPlayers(GameObject _triggeringObj)
        {
            for (int i = 0; i < currentPlayersRacing.Players.Count; i++)
            {
                if (currentPlayersRacing.Players[i].Name == _triggeringObj.name)
                    return i;
            }
            Debug.LogError("Something went wrong");
            return -1;
        }

        private void OnPlayerCrossedFinishLine()
        {
            humanPlayerCount--;

            if (humanPlayerCount == 0)
            {
                if (onFinishLineCrossedText != null)
                    onFinishLineCrossedText.gameObject.SetActive(true);
            }
        }
        
        public override void EndLevel()
        {
            raceEnded = true;

            onFinishLineCrossedText?.gameObject.SetActive(true);
            raceStarted = false;

            OnLevelEnd?.Invoke();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}