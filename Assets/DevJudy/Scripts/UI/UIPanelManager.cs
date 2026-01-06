using System.Collections;
using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class UIPanelManager : MonoBehaviour
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

        //private bool gameOver = false;

        // TBA!!
        public void PauseGame()
        {
            
        }
        
        private void ShowPauseMenu()
        {
            
        }

        public void UnpauseGame()
        {
            
        }

        private void HidePauseMenu()
        {
            
        }
        
        public void SetGameOver()
        {
            //gameOver = true;

            if (uiTextManager != null)
                minutes = uiTextManager.EndTimerAndGetFinishTime(out seconds, out milliSeconds);

            Debug.Log("-----------------Game Over------------------");

            ShowRaceOverScreen();
        }

        private void ShowRaceOverScreen()
        {
            levelPanel?.SetActive(false);
            raceOverPanel?.SetActive(true);

            // Stop registering jetski-input
            StartCoroutine(WaitForGameOver());
        }

        private void ShowScores()
        {
            raceOverPanel?.SetActive(false);

            // Maybe do switch case display for different game modes

            if (scoreText != null)
                scoreText.text = $"{minutes:00}:{seconds:00}:{milliSeconds:00}";
            
            scoresPanel?.SetActive(true);
        }

        private void ShowEndGameMenu()
        {
            scoresPanel?.SetActive(false);
            endGameMenu?.SetActive(true);
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
}