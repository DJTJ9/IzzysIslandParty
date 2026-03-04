using System.Diagnostics;
using ImprovedTimers;
using Player.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIMinigolfMayhem : MonoBehaviour
{
    [FoldoutGroup("Canvas Elements", expanded: false)]
    [SerializeField] private Image shootForceBar;
    [SerializeField] private TMP_Text shootForceText;
    [SerializeField] private GameObject cooldownTimer;
    [SerializeField] private Image cooldownBar;
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private GameObject roundTimer;
    [SerializeField] private TMP_Text roundTimerText;
    [SerializeField] private GameObject shootCounter;
    [SerializeField] private TMP_Text shootCounterText;

    [SerializeField] private SO_PlayerCollectionRacingGames playersSO;

    private MinigolfMayhemGameManager minigolfMayhemGameManager;
    private Controller playerController;
    private RigidbodyMovement rigidbodyMovement;
    private GoapRigidbodyMovement goapRigidbodyMovement;
    private Stopwatch finishTimer;

    private const float DISAPPEAR_TIME_BUFFER = 0.3f;
    private bool isPlayer;
    private bool isNPC;
    private bool racingMode;
    private bool classicMode;

    private void Start()
    {
        minigolfMayhemGameManager = FindFirstObjectByType<MinigolfMayhemGameManager>();
        racingMode = minigolfMayhemGameManager.RaceMode;
        classicMode = minigolfMayhemGameManager.ClassicMode;
        playerController = transform.parent.GetComponentInChildren<Controller>();
        rigidbodyMovement = playerController.transform.parent.GetComponentInChildren<RigidbodyMovement>();
        isPlayer = rigidbodyMovement != null;
        goapRigidbodyMovement = playerController.transform.parent.GetComponentInChildren<GoapRigidbodyMovement>();
        isNPC = goapRigidbodyMovement != null;
        shootForceBar.fillAmount = 0;
        cooldownBar.fillAmount = 0;

        if (racingMode)
        {
            roundTimer.SetActive(true);
            finishTimer = new Stopwatch();
            finishTimer.Start();
        }
        if (classicMode)
        {
            shootCounter.SetActive(true);
        }
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
        if (isPlayer)
        {
            var normalizedForce = (rigidbodyMovement.CurrentShootForce - rigidbodyMovement.minShootForce) 
                                  / (rigidbodyMovement.maxShootForce - rigidbodyMovement.minShootForce);
            shootForceBar.fillAmount = Mathf.Clamp01(normalizedForce);
            shootForceText.text = $"{Mathf.RoundToInt(normalizedForce * 100).ToString()}%";
            
            cooldownBar.fillAmount = Mathf.Clamp01(rigidbodyMovement.MovementCooldownTimer.CurrentTime / rigidbodyMovement.MovementCooldown);
            cooldownText.text = $"{Mathf.RoundToInt(rigidbodyMovement.MovementCooldownTimer.CurrentTime).ToString()}s";
            cooldownTimer.SetActive(rigidbodyMovement.MovementCooldownTimer.CurrentTime > DISAPPEAR_TIME_BUFFER && rigidbodyMovement.MovementCooldownTimer.IsRunning);
            
            playersSO.Players[playerController.PlayerIndex].PlayerScore.Value = rigidbodyMovement.ShotsTaken;
            shootCounterText.text = rigidbodyMovement.ShotsTaken.ToString();
        }

        if (isNPC)
        {
            cooldownBar.fillAmount = Mathf.Clamp01(goapRigidbodyMovement.MovementCooldownTimer.CurrentTime / goapRigidbodyMovement.MovementCooldown);
            cooldownText.text = $"{Mathf.RoundToInt(goapRigidbodyMovement.MovementCooldownTimer.CurrentTime).ToString()}s";
            cooldownTimer.SetActive(goapRigidbodyMovement.MovementCooldownTimer.CurrentTime > DISAPPEAR_TIME_BUFFER && goapRigidbodyMovement.MovementCooldownTimer.IsRunning);
            
            playersSO.Players[playerController.PlayerIndex].PlayerScore.Value = goapRigidbodyMovement.ShotsTaken;
            shootCounterText.text = goapRigidbodyMovement.ShotsTaken.ToString();
        }

        if (!racingMode) return;
        roundTimerText.text = finishTimer.Elapsed.ToString("m':'ss':'ff");
        playersSO.Players[playerController.PlayerIndex].Time = finishTimer.Elapsed.ToString("m':'ss':'ff");
        playersSO.Players[playerController.PlayerIndex].TimeValue = (float)finishTimer.Elapsed.TotalMilliseconds;
    }

    private void OnPlayerFinished(int _playerIndex)
    {
        if (_playerIndex == playerController.PlayerIndex) StopTimer();
    }
    
    public void StartTimer() => finishTimer.Start();
    
    public void StopTimer() => finishTimer.Stop();
    
    public string GetFinishTime() => finishTimer.Elapsed.ToString("m':'ss':'ff");
}