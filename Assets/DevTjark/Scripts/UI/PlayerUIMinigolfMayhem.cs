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
    private Stopwatch m_finishTimer;

    private const float k_DisappearTimeBuffer = 0.3f;
    private bool m_isPlayer;
    private bool m_isNPC;
    private bool m_racingMode;
    private bool m_classicMode;

    private void Start()
    {
        minigolfMayhemGameManager = FindFirstObjectByType<MinigolfMayhemGameManager>();
        m_racingMode = minigolfMayhemGameManager.RaceMode;
        m_classicMode = minigolfMayhemGameManager.ClassicMode;
        playerController = transform.parent.GetComponentInChildren<Controller>();
        rigidbodyMovement = playerController.transform.parent.GetComponentInChildren<RigidbodyMovement>();
        m_isPlayer = rigidbodyMovement != null;
        goapRigidbodyMovement = playerController.transform.parent.GetComponentInChildren<GoapRigidbodyMovement>();
        m_isNPC = goapRigidbodyMovement != null;
        shootForceBar.fillAmount = 0;
        cooldownBar.fillAmount = 0;

        if (m_racingMode)
        {
            roundTimer.SetActive(true);
            m_finishTimer = new Stopwatch();
            m_finishTimer.Start();
        }
        if (m_classicMode)
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
        if (m_isPlayer)
        {
            var normalizedForce = (rigidbodyMovement.CurrentShootForce - rigidbodyMovement.minShootForce) 
                                  / (rigidbodyMovement.maxShootForce - rigidbodyMovement.minShootForce);
            shootForceBar.fillAmount = Mathf.Clamp01(normalizedForce);
            shootForceText.text = $"{Mathf.RoundToInt(normalizedForce * 100).ToString()}%";
            
            cooldownBar.fillAmount = Mathf.Clamp01(rigidbodyMovement.MovementCooldownTimer.CurrentTime / rigidbodyMovement.MovementCooldown);
            cooldownText.text = $"{Mathf.RoundToInt(rigidbodyMovement.MovementCooldownTimer.CurrentTime).ToString()}s";
            cooldownTimer.SetActive(rigidbodyMovement.MovementCooldownTimer.CurrentTime > k_DisappearTimeBuffer && rigidbodyMovement.MovementCooldownTimer.IsRunning);
            
            playersSO.Players[playerController.PlayerIndex].PlayerScore.Value = rigidbodyMovement.ShotsTaken;
            shootCounterText.text = rigidbodyMovement.ShotsTaken.ToString();
        }

        if (m_isNPC)
        {
            cooldownBar.fillAmount = Mathf.Clamp01(goapRigidbodyMovement.MovementCooldownTimer.CurrentTime / goapRigidbodyMovement.MovementCooldown);
            cooldownText.text = $"{Mathf.RoundToInt(goapRigidbodyMovement.MovementCooldownTimer.CurrentTime).ToString()}s";
            cooldownTimer.SetActive(goapRigidbodyMovement.MovementCooldownTimer.CurrentTime > k_DisappearTimeBuffer && goapRigidbodyMovement.MovementCooldownTimer.IsRunning);
            
            playersSO.Players[playerController.PlayerIndex].PlayerScore.Value = goapRigidbodyMovement.ShotsTaken;
            shootCounterText.text = goapRigidbodyMovement.ShotsTaken.ToString();
        }

        if (!m_racingMode) return;
        roundTimerText.text = m_finishTimer.Elapsed.ToString("m':'ss':'ff");
        playersSO.Players[playerController.PlayerIndex].Time = m_finishTimer.Elapsed.ToString("m':'ss':'ff");
        playersSO.Players[playerController.PlayerIndex].TimeValue = (float)m_finishTimer.Elapsed.TotalMilliseconds;
    }

    public void StartTimer()
    {
        if (!roundTimer.activeSelf) return;
        m_finishTimer.Start();
    }
    
    public string GetFinishTime() => m_finishTimer.Elapsed.ToString("m':'ss':'ff");

    private void OnPlayerFinished(int _playerIndex)
    {
        if (_playerIndex == playerController.PlayerIndex && roundTimer.activeSelf) StopTimer();
    }
    
    private void StopTimer() => m_finishTimer.Stop();
}