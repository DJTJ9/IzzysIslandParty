using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GUIBowlingBattle : MonoBehaviour
{
    [FoldoutGroup("Canvas Elements", expanded: false)]
    [SerializeField] private GameObject bowlingBattleUI;
    // [SerializeField] private GameObject ballButtons;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject score;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private SO_PlayerCollection playersSO;
    [SerializeField] private PlayerControllerBowlingBattle playerController;

    private Button m_currentSelectedButton;
    
    // private void Start()
    // {
    //     ConfigureButtonClickEvents();
    // }

    private void Update()
    {
        UpdateScoreAndTimerLabels();
    }
    
    public void ShowUI()
    {
        bowlingBattleUI.SetActive(true);
        timer.SetActive(true);
    }

    public void HideUI() => timer.SetActive(false);

    private void UpdateScoreAndTimerLabels()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        scoreLabel.text = playersSO.Players[playerController.GetPlayerIndex()].PlayerScore.Value.ToString("0");
    }
}