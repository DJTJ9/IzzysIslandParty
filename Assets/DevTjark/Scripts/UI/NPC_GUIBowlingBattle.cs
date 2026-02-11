using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class NPC_GUIBowlingBattle : MonoBehaviour
{
    [FoldoutGroup("Canvas Elements", expanded: false)]
    [SerializeField] private GameObject bowlingBattleUI;
    // [SerializeField] private GameObject ballButtons;
    [SerializeField] private GameObject timer;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private GameObject roundTimer;
    [SerializeField] private TMP_Text roundTimerLabel;
    [SerializeField] private GameObject score;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private SO_PlayerCollection npcSO;
    
    [SerializeField] private NPC_BowlingBattleController npcController;

    private void Update()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        roundTimerLabel.text = BowlingBattleGameManager.RoundTimer.ToString("0");
        scoreLabel.text = npcSO.Players[npcController.NPCIndex].PlayerScore.Value.ToString("0");
    }

    public void ShowPreparationPhaseTimer()
    {
        bowlingBattleUI.SetActive(true);
        timer.SetActive(true);
    }

    public void HidePreparationPhaseTimer()
    {
        timer.SetActive(false);
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
}
