using System.Collections;
using TMPro;
using UIScripts;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [Header("GameOver UI:")]
    [SerializeField] private GameObject raceOverPanel;

    [SerializeField] private GameObject scoresPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject endGameMenu;

    [Header("Dependencies:")]
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private UITextManager uiTextManager;

    [Header("Variables:")]
    [SerializeField] private float waitTimeAfterGameOver = 5f;

    private float minutes;
    private float seconds;
    private float milliSeconds;

    private bool gameOver = false;

    public void SetGameOver()
    {
        gameOver = true;

        if (uiTextManager != null)
            minutes = uiTextManager.EndTimerAndGetFinishTime(out seconds, out milliSeconds);

        Debug.Log("-----------------Game Over------------------");

        ShowRaceOverScreen();
    }

    private void ShowRaceOverScreen()
    {
        levelPanel.SetActive(false);
        raceOverPanel.SetActive(true);

        // Stop registering jetski-input
        StartCoroutine(WaitForGameOver());
    }

    private void ShowScores()
    {
        raceOverPanel.SetActive(false);
        
        // Maybe do switch for game mode
        scoreText.text = $"{minutes:00}:{seconds:00}:{milliSeconds:00}";
        scoresPanel.SetActive(true);
    }

    private void ShowEndGameMenu()
    {
        scoresPanel.SetActive(false);
        endGameMenu.SetActive(true);
    }

    private IEnumerator WaitForGameOver()
    {
        bool showStuff = true;
        while (showStuff)
        {
            yield return new WaitForSeconds(waitTimeAfterGameOver);

            ShowScores();
            // TBA Wait for input from player

            yield return new WaitForSeconds(waitTimeAfterGameOver);

            showStuff = false;
        }

        ShowEndGameMenu();
    }
}