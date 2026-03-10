using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
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
    
    /// <summary>
    /// Displays the preparation phase timer by enabling the bowling battle UI 
    /// and showing the corresponding timer element.
    /// </summary>
    public void ShowPreparationPhaseTimer()
    {
        bowlingBattleUI.SetActive(true);
        preparationPhaseTimer.SetActive(true);
    }

    /// <summary>
    /// Hides the preparation phase timer by disabling the corresponding timer element.
    /// </summary>
    public void HidePreparationPhaseTimer()
    {
        preparationPhaseTimer.SetActive(false);
    }
    
    /// <summary>
    /// Displays the round timer by enabling the bowling battle UI 
    /// and showing the corresponding round timer element.
    /// </summary>
    public void ShowRoundTimer()
    {
        bowlingBattleUI.SetActive(true);
        roundTimer.SetActive(true);
    }
    
    /// <summary>
    /// Hides the round timer by disabling the corresponding round timer element.
    /// </summary>
    public void HideRoundTimer()
    {
        roundTimer.SetActive(false);
    }

    /// <summary>
    /// Updates the labels for the preparation phase timer, round timer, and player score.
    /// Values are pulled from the game manager and player collection.
    /// </summary>
    private void UpdateScoreAndTimerLabels()
    {
        preparationPhaseTimerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        roundTimerLabel.text = BowlingBattleGameManager.RoundTimer.ToString("0");
        scoreLabel.text = playersSO.Players[playerController.GetPlayerIndex()].PlayerScore.Value.ToString("0");
    }
}