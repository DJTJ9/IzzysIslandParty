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
    [SerializeField] private GameObject preparationPhaseTimer;
    [SerializeField] private TMP_Text preparationPhaseTimerLabel;
    [SerializeField] private GameObject roundTimer;
    [SerializeField] private TMP_Text roundTimerLabel;
    [SerializeField] private GameObject score;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private SO_PlayerCollection playersSO;
    [SerializeField] private PlayerControllerBowlingBattle playerController;

    private Button m_currentSelectedButton;

    private void Update()
    {
        UpdateScoreAndTimerLabels();
    }
    
    public void ShowPreparationPhaseTimer()
    {
        bowlingBattleUI.SetActive(true);
        preparationPhaseTimer.SetActive(true);
    }

    public void HidePreparationPhaseTimer()
    {
        preparationPhaseTimer.SetActive(false);
    }
    
    public void ShowRoundTimer()
    {
        bowlingBattleUI.SetActive(true);
        roundTimer.SetActive(true);
    }
    
    public void HideRoundTimer()
    {
        roundTimer.SetActive(false);
    }

    private void UpdateScoreAndTimerLabels()
    {
        preparationPhaseTimerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        roundTimerLabel.text = BowlingBattleGameManager.RoundTimer.ToString("0");
        scoreLabel.text = playersSO.Players[playerController.GetPlayerIndex()].PlayerScore.Value.ToString("0");
    }
}