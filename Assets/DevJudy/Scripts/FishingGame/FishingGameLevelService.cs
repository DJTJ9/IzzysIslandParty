using System.Collections.Generic;
using Audio;
using MultiuseScripts;
using UIScripts;
using UnityEngine;

namespace FishingGame
{
    public class FishingGameLevelService : LevelServiceParent
    {
        [SerializeField] private GameAudioManager gameAudioManager;
        [SerializeField] private LevelTimer levelTimer;

        [Header("Temp: ")]
        [SerializeField] private UIPointsService points;
        [SerializeField] private UIPanelManager uiPanelManager;
        [SerializeField] private List<string> playerNames;
        [SerializeField] private Vector2 npcPointRange;
        private int numberOfPlayers = 1;
        private int maxNumberOfPlayers = 4;

        private List<int> npcScores = new List<int>();
        
        private void Start()
        {
            if (gameAudioManager == null || levelTimer == null)
                Debug.LogWarning("GameAudioManager or levelTimer is null");
            else
                gameAudioManager.StartBackgroundMusic(() => !levelTimer.TimerFinished);
            
            StartLevel();
        }

        public override void StartLevel()
        {
            OnLevelStart.Invoke();
        }

        public void GetNPCScores()
        {
            npcScores.Add(points.CurrentScore);
            
            for (int i = numberOfPlayers; i < maxNumberOfPlayers; i++)
            {
                float randomScore = Random.Range(npcPointRange.x, npcPointRange.y + 1);
                npcScores.Add((int)randomScore);
            }
            
            uiPanelManager.SetPointsAndRankingsTexts(playerNames, npcScores);
        }

        public override void EndLevel()
        {
            Debug.Log("Ending level");
            OnLevelEnd.Invoke();
        }
    }
}