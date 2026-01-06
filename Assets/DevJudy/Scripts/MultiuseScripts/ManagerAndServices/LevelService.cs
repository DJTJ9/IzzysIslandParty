using System;
using System.Collections;
using HelperScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelService : MonoBehaviour
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

    private bool levelStarted = false;

    private Coroutine levelCountdownCoroutine = null;

    private void Awake()
    {
        if (goalTransform == null)
            Debug.LogWarning("Goal transform not set");
        
        if (placementOrder.Length > maxNumberOfPlayers)
        {
            Debug.LogWarning($"PlacementOrder array is more than maximum number of players ({maxNumberOfPlayers}), resizing array");
            placementOrder = ArrayHelper.ResizeArray(placementOrder, maxNumberOfPlayers);
        }
    }
    
    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        if (levelCountdownText != null && levelCountdownCoroutine == null)
            levelCountdownCoroutine = StartCoroutine(CountdownToLevelStart());

        if (levelCountdownCoroutine == null)
        {
            Debug.Log("Invoking correctly");
            onLevelStart.Invoke();
            levelStarted = true;
        }
    }

    private void OnCoroutineOver()
    {
        StopCoroutine(CountdownToLevelStart());
    }

    private IEnumerator CountdownToLevelStart()
    {
        levelCountdownText.enabled = true;

        for (int i = secondsToStartLevel; i > 0; i--)
        {
            levelCountdownText.text = i.ToString() + dotsText;

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
        if (levelStarted)
            CheckPlacements();
    }

    private void CheckPlacements()
    {
        //int n = arr.Length;
        // for (int i = 1; i < n; ++i) {
        //     int key = arr[i];
        // int j = i - 1;

        /* Move elements of arr[0..i-1], that are
           greater than key, to one position ahead
           of their current position */
        //  while (j >= 0 && arr[j] > key) {
        //      arr[j + 1] = arr[j];
        //      j = j - 1;
        //  }
        //  arr[j + 1] = key;
        
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
    }
    // [Mario, Sonic, Peach, Amy, Daisy]
    // 9, 7, 4, 5, 4
    
    // -- first pass = currDistance = 7; gm = sonic; lN = 0;
    // distance of [ln (0) aka mario] (9) > currDi (7) 
    // -> arr[ln + 1 (1), aka Sonic] = arr[ln (0) aka Mario] == [Mario, Mario, Peach, Amy, Daisy]
    // -> ln (0) = ln - 1 (-1)
    // ! ln >= 0
    // arr[ln + 1 (0) aka Mario] = gm (sonic) == [Sonic, Mario, Peach, Amy, Daisy]
    
    
    // -- second pass = key = 4; ln = 1;
    // arr[ln (1)] (int 7) > key (int 4)
    // -> arr[ln +1 (2)] (int 4) = arr[ln (1)] (int 7);   // [1, 7, 7, 2, 5]
    // -> ln = ln - 1 (0);
    // arr[ln (0)] (int 1) !> key (int 4)
    // arr[ln + 1 (1)] (int = key (int 4)   // [1, 4, 7, 2, 5]
    

    private float GetDistanceToGoal(Vector3 _gameObjectPos)
    {
        return Vector2.Distance(new Vector2(_gameObjectPos.x, _gameObjectPos.z), new Vector2(goalTransform.position.x, goalTransform.position.z));
    }

    public void EndLevel()
    {
        onLevelEnd.Invoke();
    }
}