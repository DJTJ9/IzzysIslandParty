using System.Diagnostics;
using ImprovedTimers;
using Player.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private bool isRacingMode;
    [SerializeField] private bool isNormalMode;
    
    [SerializeField] private Image shootForceBar;
    [SerializeField] private TMP_Text shootForceText;
    [SerializeField] private GameObject timer;
    [SerializeField] private TMP_Text timerText;
    // [SerializeField] private GameObject shootCounter;
    // [SerializeField] private TMP_Text shootCounterText;

    [SerializeField] private SO_PlayerCollectionRacingGames playersSO;

    private Controller playerController;
    private RigidbodyMovement rigidbodyMovement;
    
    private Stopwatch finishTimer;

    private void Start()
    {
        playerController = transform.parent.GetComponentInChildren<Controller>();
        rigidbodyMovement = playerController.transform.parent.GetComponentInChildren<RigidbodyMovement>();
        shootForceBar.fillAmount = 0;

        if (isRacingMode)
        {
            timer.SetActive(true);
            finishTimer = new Stopwatch();
            finishTimer.Start();
        }
        // if (isNormalMode)
        // {
        //     shootCounter.SetActive(true);
        // }
    }

    private void OnEnable()
    {
        MinigolfHole.onMinigolfPlayerFinished += OnPlayerFinished;
    }

    private void OnDisable()
    {
        MinigolfHole.onMinigolfPlayerFinished -= OnPlayerFinished;
    }

    private void Update()
    {
        if (isRacingMode)
        {
            // timerText.text = finishTimer.CurrentTime.ToString("m':'ss':'ff");
            timerText.text = finishTimer.Elapsed.ToString("m':'ss':'ff");
            playersSO.Players[playerController.PlayerIndex].Time = finishTimer.Elapsed.ToString("m':'ss':'ff");
            playersSO.Players[playerController.PlayerIndex].TimeValue = (float)finishTimer.Elapsed.TotalMilliseconds;
        }
        
        if (rigidbodyMovement == null) return;
        var normalizedForce = (rigidbodyMovement.CurrentShootForce - rigidbodyMovement.minShootForce) / (rigidbodyMovement.maxShootForce - rigidbodyMovement.minShootForce);
        shootForceBar.fillAmount = Mathf.Clamp01(normalizedForce);
        shootForceText.text = $"{Mathf.RoundToInt(normalizedForce * 100).ToString()}%";
        // else
        // {
        //     shootCounterText.text = playerController
        // }
    }

    private void OnPlayerFinished(int _playerIndex)
    {
        if (_playerIndex == playerController.PlayerIndex) StopTimer();
    }
    
    public void StartTimer() => finishTimer.Start();
    
    public void StopTimer() => finishTimer.Stop();
    
    public string GetFinishTime() => finishTimer.Elapsed.ToString("m':'ss':'ff");
}