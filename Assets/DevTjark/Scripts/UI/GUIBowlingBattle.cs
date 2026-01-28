using System;
using TMPro;
using UnityEngine;

public class GUIBowlingBattle : MonoBehaviour
{
    [SerializeField] private GameObject bowlingBattleUI;
    [SerializeField] private TMP_Text timerLabel;
    
 private void Update()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("00");
    }

    public void ShowUI() => bowlingBattleUI.SetActive(true);
    public void HideUI() => bowlingBattleUI.SetActive(false);
}
