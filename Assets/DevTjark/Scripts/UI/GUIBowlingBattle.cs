using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GUIBowlingBattle : MonoBehaviour
{
    [FoldoutGroup("Canvas Elements", expanded: false)]
    [SerializeField] private GameObject bowlingBattleUI;
    [SerializeField] private GameObject ballButtons;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject score;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private SO_PlayerCollection playerSO;
    
    [SerializeField] private PlayerControllerBowlingBattle playerController;

    private void Update()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("00");
        scoreLabel.text = playerSO.Players[playerController.GetPlayerIndex()].PlayerScore.Value.ToString("00");
    }

    public void ShowUI()
    {
        bowlingBattleUI.SetActive(true);
        ballButtons.SetActive(true);
        timer.SetActive(true);
    }

    public void HideUI()
    {
        ballButtons.SetActive(false);
        timer.SetActive(false);
    } 
}