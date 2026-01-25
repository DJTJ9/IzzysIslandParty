using System.Collections;
using MultiuseScripts;
using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class UIPanelManager : MonoBehaviour
    {
        [Header("Panel: ")]
        [SerializeField] private GameObject raceOverPanel;
        [SerializeField] private GameObject scoresPanel;
        [SerializeField] private GameObject levelPanel;

        [Header("Text: ")]
        [SerializeField] private TextMeshProUGUI playerPlacementsText;
        [SerializeField] private TextMeshProUGUI playerNamesText;
        [SerializeField] private TextMeshProUGUI playerTimesText;

        [Header("Dependencies:")]
        [SerializeField] private RacingGameLevelService racingGameLevelService;
        [SerializeField] private GameObject endGameMenu;

        [Header("Variables:")]
        [SerializeField] private float waitTimeAfterGameOver = 5f;

        private float minutes;
        private float seconds;
        private float milliSeconds;

        private void Start()
        {
            raceOverPanel?.SetActive(false);
            scoresPanel?.SetActive(false);
            endGameMenu?.SetActive(false);
        }
        
        public void SetGameOver()
        {
            ShowRaceOverScreen();
        }

        private void ShowRaceOverScreen()
        {
            levelPanel?.SetActive(false);
            raceOverPanel?.SetActive(true);

            // Stop registering jetski-input
            StartCoroutine(WaitForGameOver());
        }

        private void SetRankingTexts()
        {
            ClearPlacementTextFields();
            
            for (int i = 1; i < racingGameLevelService.WinnerPlacementOrder.Count + 1; i++)
            {
                playerPlacementsText.text += $"{(i)}. \n";
                playerNamesText.text += racingGameLevelService.WinnerPlacementOrder[i].Item1.name + "\n";
                playerTimesText.text += racingGameLevelService.WinnerPlacementOrder[i].Item2 + "\n";
            }
        }

        private void ClearPlacementTextFields()
        {
            playerPlacementsText.text = "";
            playerNamesText.text = "";
            playerTimesText.text = "";
        }

        private void ShowScores()
        {
            raceOverPanel?.SetActive(false);

            // Maybe do switch case display for different game modes
            if (racingGameLevelService.CheckPlacements)
            {
                SetRankingTexts();
                scoresPanel?.SetActive(true);
            }
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

                yield return new WaitForSeconds(waitTimeAfterGameOver * 2);

                showStuff = false;
            }

            ShowEndGameMenu();
        }
    }
}