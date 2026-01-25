using System.Collections;
using HelperScripts;
using Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Service
{
    public class JetskiGameLevelService : MonoBehaviour
    {
        #region consts

        private const string levelStartText = "START";
        private const string dotsText = "...";

        private const int maxNumberOfPlayers = 5;

        #endregion

        [Header("Level start/end: ")]
        [SerializeField] private TextMeshProUGUI levelCountdownText;

        [SerializeField] private int secondsToStartLevel;

        [SerializeField] private UnityEvent onLevelStart;
        [SerializeField] private UnityEvent onLevelEnd;

        [Header("Level running: ")]
        // !! This is kinda only for the racing-games...
        [SerializeField] private Transform goalTransform;

        [SerializeField] private GameObject[] placementOrder;

        [SerializeField] private bool checkPlacements;
        private bool levelStarted;

        private Coroutine levelCountdownCoroutine = null;

        [Header("Temp ")]
        [SerializeField] private TextMeshProUGUI placementText;

        [SerializeField] private TextMeshProUGUI onFinishLineCrossedText;

        private void Awake()
        {
            CheckPlacementList();
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
                    ArrayHelper.RemoveFromArray(placementOrder, placementOrder[i]);
                }
            }
        }

        private void Start()
        {
            if (goalTransform == null)
            {
                Debug.LogWarning("Goal transform not set");
                checkPlacements = false;
            }

            StartLevel();
        }

        private void StartLevel()
        {
            levelCountdownCoroutine = StartCoroutine(CountdownToLevelStart());
        }

        private void LetNPCsStart()
        {
            foreach (GameObject obj in placementOrder)
            {
                if (obj.TryGetComponent(out PathfindingUnit pathfindingUnit))
                    pathfindingUnit.CanFollowPath();
            }
        }

        private void OnCoroutineOver()
        {
            StopCoroutine(CountdownToLevelStart());

            LetNPCsStart();

            onLevelStart.Invoke();
            levelStarted = true;
        }

        private IEnumerator CountdownToLevelStart()
        {
            if (levelCountdownText == null)
                OnCoroutineOver();

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
            if (levelStarted && checkPlacements)
                CheckPlacements();
        }

        private void CheckPlacements()
        {
            for (int i = 1; i < placementOrder.Length; i++)
            {
                float currentDistanceToCompare = GetDistanceToGoal(placementOrder[i].transform.position);
                GameObject currentGameObjectBeingCompared = placementOrder[i];

                int leftNeighbour = i - 1; // 0, 1

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

            var placement = GetPlayerNumber() + 1;

            placementText.text = (placement.ToString() + "/" + placementOrder.Length);
        }

        private int GetPlayerNumber()
        {
            for (int i = 0; i < placementOrder.Length; i++)
            {
                if (placementOrder[i].CompareTag("Player"))
                    return i;
            }

            return -1;
        }

        private float GetDistanceToGoal(Vector3 _gameObjectPos)
        {
            return Vector2.Distance(new Vector2(_gameObjectPos.x, _gameObjectPos.z), new Vector2(goalTransform.position.x, goalTransform.position.z));
        }

        public void OnFinishLineCrossed()
        {
            if (onFinishLineCrossedText != null)
                onFinishLineCrossedText.gameObject.SetActive(true);
        }

        public void EndLevel()
        {
            onFinishLineCrossedText?.gameObject.SetActive(false);
            onLevelEnd.Invoke();
        }
    }
}