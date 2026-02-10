using System.Collections;
using UnityEngine;

namespace UIScripts
{
    public class UIPanelManager : MonoBehaviour
    {
        [Header("Panel: ")]
        [SerializeField] private GameObject raceOverPanel = null;

        [SerializeField] private GameObject levelPanel;

        private void Start()
        {
            if (raceOverPanel != null)
                raceOverPanel.SetActive(false);
            else
                raceOverPanel = null;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
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