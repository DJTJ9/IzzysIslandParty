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
    [SerializeField] private GameObject score;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private SO_PlayerCollection npcSO;
    
    [SerializeField] private NPC_BowlingBattleController npcController;

    private void Update()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        scoreLabel.text = npcSO.Players[npcController.NPCIndex].PlayerScore.Value.ToString("0");
    }

    public void ShowUI()
    {
        bowlingBattleUI.SetActive(true);
        // ballButtons.SetActive(true);
        timer.SetActive(true);
    }

    public void HideUI()
    {
        // ballButtons.SetActive(false);
        timer.SetActive(false);
    } 
}
