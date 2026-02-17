using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIScripts
{
    public class UIPanelManager : MonoBehaviour
    {
        [Header("Panel: ")]
        [SerializeField] private GameObject raceOverPanel = null;

        [SerializeField] private GameObject levelPanel;
        [SerializeField] private GameObject playerIdentPanel;
        [SerializeField] private List<Image> playerIdentImages = new List<Image>();

        private int players;

        private void Start()
        {
            if (raceOverPanel != null)
                raceOverPanel.SetActive(false);
            else
                raceOverPanel = null;

            if (playerIdentPanel != null)
                playerIdentPanel.SetActive(false);

            for (int i = 0; i < playerIdentImages.Count; i++)
            {
               playerIdentImages[i].gameObject.SetActive(false);
            }
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void SetPlayers(int _players)
        {
            players = _players;
        }

        public void ShowPlayerIdents()
        {
            playerIdentPanel.SetActive(true);

            for (int i = 0; i < players; i++)
            {
                playerIdentImages[i].gameObject.SetActive(true);
            }
        }

        public void FadeOutPlayerIdents()
        {
            StartCoroutine(FadeOutPlayerIdentsCoroutine());
        }

        private IEnumerator FadeOutPlayerIdentsCoroutine()
        {
            float alphaValue = 0.1f;
            Color tempColor = new Color(1, 1, 1, 1);
            
            while (tempColor.a > 0.05f)
            {
                for (int i = 0; i < players; i++)
                { 
                    tempColor = playerIdentImages[i].color;
                    tempColor.a -= alphaValue;
                    playerIdentImages[i].color = tempColor;
                }

                yield return new WaitForEndOfFrame();
            }

            playerIdentPanel.SetActive(false);
            
            yield return null;
        }

        public void SetGameOver()
        {
            ShowRaceOverScreen();
        }

        private void ShowRaceOverScreen()
        {
            levelPanel?.SetActive(false);

            StartCoroutine(WaitToDisableRaceOverPanel());

            if (raceOverPanel != null)
                raceOverPanel.SetActive(true);
        }

        private IEnumerator WaitToDisableRaceOverPanel()
        {
            yield return new WaitForSecondsRealtime(2f);

            if (raceOverPanel != null)
                raceOverPanel?.SetActive(false);
        }
    }
}