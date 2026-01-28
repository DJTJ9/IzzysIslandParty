using System.Collections;
using MultiuseScripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UIScripts
{
    public class UIPanelManager : MonoBehaviour
    {
        [Header("Panel: ")]
        [SerializeField] private GameObject raceOverPanel = null;
        [SerializeField] private GameObject scoresPanel;
        [SerializeField] private GameObject levelPanel;

        [Header("Text: ")]
        [SerializeField] private bool showTimer;
        [SerializeField] private TextMeshProUGUI playerPlacementsText;
        [SerializeField] private TextMeshProUGUI playerNamesText;

        [ShowIf("showTimer")]
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
            if (raceOverPanel != null)
                raceOverPanel.SetActive(false);
            else
                raceOverPanel = null;
            
            if (scoresPanel != null)
                scoresPanel?.SetActive(false);

            if (levelPanel != null)
                endGameMenu?.SetActive(false);
        }

        public void SetGameOver()
        {
            ShowRaceOverScreen();
        }

        private void ShowRaceOverScreen()
        {
            levelPanel?.SetActive(false);

            if (raceOverPanel != null)
                raceOverPanel.SetActive(true);

            // Stop registering input
            StartCoroutine(WaitForGameOver());
        }

        private void SetRankingTexts()
        {
            ClearPlacementTextFields();

            for (int i = 1; i < racingGameLevelService.WinnerPlacementOrder.Count + 1; i++)
            {
                playerPlacementsText.text += $"{(i)}. \n";
                playerNamesText.text += racingGameLevelService.WinnerPlacementOrder[i].Item1.name + "\n";

                if (showTimer)
                    playerTimesText.text += racingGameLevelService.WinnerPlacementOrder[i].Item2 + "\n";
            }
        }

        private void ClearPlacementTextFields()
        {
            if (playerPlacementsText != null)
                playerPlacementsText.text = "";

            if (playerNamesText != null)
                playerNamesText.text = "";

            if (playerTimesText != null)
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