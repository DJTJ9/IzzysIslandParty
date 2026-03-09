using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC_GUIBowlingBattle : MonoBehaviour
{
    [FoldoutGroup("Canvas Elements", expanded: false)]
    [SerializeField] private GameObject bowlingBattleUI;
    [SerializeField] private GameObject timer;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private GameObject roundTimer;
    [SerializeField] private TMP_Text roundTimerLabel;
    [SerializeField] private GameObject score;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private SO_PlayerCollection npcSO;
    
    [SerializeField] private NPC_BowlingBattleController npcController;

    /// <summary>
    /// Updates the UI elements dynamically:
    /// - Updates the preparation phase timer label.
    /// - Updates the round timer label.
    /// - Updates the NPC's score label using data from the NPC's Player Collection.
    /// </summary>
    private void Update()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        roundTimerLabel.text = BowlingBattleGameManager.RoundTimer.ToString("0");
        scoreLabel.text = npcSO.Players[npcController.NPCIndex].PlayerScore.Value.ToString("0");
    }

    /// <summary>
    /// Displays the preparation phase timer by enabling the bowling battle UI 
    /// and making the timer element visible.
    /// </summary>
    public void ShowPreparationPhaseTimer()
    {
        bowlingBattleUI.SetActive(true);
        timer.SetActive(true);
    }

    /// <summary>
    /// Hides the preparation phase timer by disabling the timer element.
    /// </summary>
    public void HidePreparationPhaseTimer()
    {
        timer.SetActive(false);
    } 
    
    /// <summary>
    /// Displays the round timer by enabling the bowling battle UI 
    /// and making the round timer element visible.
    /// </summary>
    public void ShowRoundTimer()
    {
        bowlingBattleUI.SetActive(true);
        roundTimer.SetActive(true);
    }
    
    /// <summary>
    /// Hides the round timer by disabling the round timer element.
    /// </summary>
    public void HideRoundTimer()
    {
        roundTimer.SetActive(false);
    }
}
