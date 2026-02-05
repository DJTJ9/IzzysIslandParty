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
    
    [SerializeField] private MultiplayerEventSystem eventSystem;
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
        // ballButtons.SetActive(true);
        timer.SetActive(true);

        // if (m_currentSelectedButton == null)
        // {
        //     RefocusButton(ballButtons.GetComponentInChildren<Button>());
        //     return;
        // }
        // RefocusButton(m_currentSelectedButton);
    }

    public void HideUI()
    {
        // ballButtons.SetActive(false);
        timer.SetActive(false);
    } 
    
    // private void ConfigureButtonClickEvents()
    // {
    //     var buttons = ballButtons.GetComponentsInChildren<Button>();
    //
    //     foreach (var button in buttons)
    //     {
    //         button.onClick.AddListener(() => RefocusButton(button));
    //     }
    //
    //     m_currentSelectedButton = buttons.First();
    // }
    
    private void UpdateScoreAndTimerLabels()
    {
        timerLabel.text = BowlingBattleGameManager.PreparationPhaseTimer.ToString("0");
        scoreLabel.text = playersSO.Players[playerController.GetPlayerIndex()].PlayerScore.Value.ToString("0");
    }
    
    // private void RefocusButton(Button _button) => StartCoroutine(RefocusButtonAfterFrameCoroutine(_button));
    //
    // private IEnumerator RefocusButtonAfterFrameCoroutine(Button _button)
    // {
    //     yield return null;
    //     eventSystem.SetSelectedGameObject(_button.gameObject);
    //     m_currentSelectedButton = _button;
    // }
}