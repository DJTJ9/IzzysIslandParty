using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Serialization;

public class GameMenuEvents : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;
    [SerializeField] private SO_PlayerCollection playerCollection;
    
    [SerializeField] private VisualTreeAsset rowTemplate;
    
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onRestart;
    
    private UIDocument document;
    
    [Header("Menus")]
    private VisualElement pauseMenu;
    private VisualElement playerHub;
    private VisualElement endScreenUI;
    private VisualElement resultsScreen;
    private VisualElement resultsModal;
    
    [Header("Pause Menu Buttons")]
    private Button pauseMenuResumeButton;
    private Button pauseMenuRestartButton;
    private Button pauseMenuChangeLevelButton;
    private Button pauseMenuQuitButton;
    
    [Header("Player HUB Buttons")]
    private Button bowlingBattleButton;
    private Button fishingFrenzyButton;
    private Button jetskiJoyrideButton;
    private Button minigolfMayhemButton;
    private Button swaggySnapshotsButton;
    private Button playerHUBBackButton;
    
    [Header("End Screen Buttons")]
    private Button endScreenRestartButton;
    private Button endScreenChangeLevelButton;
    private Button endScreenQuitButton;
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        
        BindVisualElements();
        BindButtons();
    }

    private void OnEnable()
    {
        RegisterButtonCallbacks();

    }

    private void OnDisable()
    {
        UnregisterButtonCallbacks();
    }

    private void BindVisualElements()
    {
        pauseMenu = document.rootVisualElement.Q("pause-menu__container");
        playerHub = document.rootVisualElement.Q("player-hub__container");
        endScreenUI = document.rootVisualElement.Q("end-screen-menu__container");
        resultsScreen = document.rootVisualElement.Q("results-screen-and-buttons__container");
        resultsModal = document.rootVisualElement.Q("results-screen__container");
    }
    
    private void BindButtons()
    {
        // Pause menu buttons
        pauseMenuResumeButton = document.rootVisualElement.Q("pause-menu-resume__button") as Button;
        pauseMenuRestartButton = document.rootVisualElement.Q("pause-menu-restart__button") as Button;
        pauseMenuChangeLevelButton = document.rootVisualElement.Q("pause-menu-change-level__button") as Button;
        pauseMenuQuitButton = document.rootVisualElement.Q("pause-menu-quit__button") as Button;
        
        //Player HUB buttons
        bowlingBattleButton = document.rootVisualElement.Q("play-bowling-battle__button") as Button;
        fishingFrenzyButton = document.rootVisualElement.Q("play-fishing-frenzy__button") as Button;
        jetskiJoyrideButton = document.rootVisualElement.Q("play-jetski-joyride__button") as Button;
        minigolfMayhemButton = document.rootVisualElement.Q("play-minigolf-mayhem__button") as Button;
        swaggySnapshotsButton = document.rootVisualElement.Q("play-swaggy-snapshots__button") as Button;
        playerHUBBackButton = document.rootVisualElement.Q("player-hub-back__button") as Button;
        
        //End screen buttons
        endScreenRestartButton = document.rootVisualElement.Q("end-screen-menu-restart__button") as Button;
        endScreenChangeLevelButton = document.rootVisualElement.Q("end-screen-menu-change-level__button") as Button;
        endScreenQuitButton = document.rootVisualElement.Q("end-screen-menu-quit__button") as Button;
    }

    private void RegisterButtonCallbacks()
    {        
        // Pause menu buttons
        pauseMenuResumeButton?.RegisterCallback<ClickEvent>(OnResumeGameClick);
        pauseMenuRestartButton?.RegisterCallback<ClickEvent>(OnRestartGameClick);
        pauseMenuChangeLevelButton?.RegisterCallback<ClickEvent>(OnChangeLevelClick);
        pauseMenuQuitButton?.RegisterCallback<ClickEvent>(OnQuitClick);
        
        //Player HUB buttons
        bowlingBattleButton?.RegisterCallback<ClickEvent>(OnLoadBowlingBattle);
        fishingFrenzyButton?.RegisterCallback<ClickEvent>(OnLoadFishingFrenzy);
        jetskiJoyrideButton?.RegisterCallback<ClickEvent>(OnLoadJetskiJoyride);
        minigolfMayhemButton?.RegisterCallback<ClickEvent>(OnLoadMinigolfMayhem);
        swaggySnapshotsButton?.RegisterCallback<ClickEvent>(OnLoadSwaggySnapshots);
        playerHUBBackButton?.RegisterCallback<ClickEvent>(OnPlayerHubBack);
        
        //End screen buttons
        endScreenRestartButton?.RegisterCallback<ClickEvent>(OnRestartGameClick);
        endScreenChangeLevelButton?.RegisterCallback<ClickEvent>(OnChangeLevelClick);
        endScreenQuitButton?.RegisterCallback<ClickEvent>(OnQuitClick);
    }

    private void UnregisterButtonCallbacks()
    {
        // Pause menu buttons
        pauseMenuResumeButton?.UnregisterCallback<ClickEvent>(OnResumeGameClick);
        pauseMenuRestartButton?.UnregisterCallback<ClickEvent>(OnRestartGameClick);
        pauseMenuChangeLevelButton?.UnregisterCallback<ClickEvent>(OnChangeLevelClick);
        pauseMenuQuitButton?.UnregisterCallback<ClickEvent>(OnQuitClick);

        //Player HUB buttons
        bowlingBattleButton?.UnregisterCallback<ClickEvent>(OnLoadBowlingBattle);
        fishingFrenzyButton?.UnregisterCallback<ClickEvent>(OnLoadFishingFrenzy);
        jetskiJoyrideButton?.UnregisterCallback<ClickEvent>(OnLoadJetskiJoyride);
        minigolfMayhemButton?.UnregisterCallback<ClickEvent>(OnLoadMinigolfMayhem);
        swaggySnapshotsButton?.UnregisterCallback<ClickEvent>(OnLoadSwaggySnapshots);
        playerHUBBackButton?.UnregisterCallback<ClickEvent>(OnPlayerHubBack);
        
        //End screen buttons
        endScreenRestartButton?.UnregisterCallback<ClickEvent>(OnRestartGameClick);
        endScreenChangeLevelButton?.UnregisterCallback<ClickEvent>(OnChangeLevelClick);
        endScreenQuitButton?.UnregisterCallback<ClickEvent>(OnQuitClick);
    }

    public void ShowPauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
    }
    
    public void HidePauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }
    
    public void ShowEndScreenUI()
    {
        endScreenUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideEndScreenUI()
    {
        endScreenUI.style.display = DisplayStyle.None;
    }
    
    private void OnRestartGameClick(ClickEvent _evt)
    {
        onRestart.Invoke();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnResumeGameClick(ClickEvent _evt)
    {
        onUnpause.Invoke();
    }

    private void OnChangeLevelClick(ClickEvent _evt)
    {
        pauseMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
    }
    
    private void OnPlayerHubBack(ClickEvent _evt)
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
    }

    private void OnQuitClick(ClickEvent _evt)
    {
        Time.timeScale = 1f;
        LoadSingleScene(SceneNames.MainMenu);
    }

    public void LoadBowlingBattle()
    {
        LoadSceneWithLevel(SceneNames.BowlingBattleGame, SceneNames.BowlingBattleLevel);
    }

    public void LoadMinigolfMayhemLevel1()
    {
        LoadSceneWithLevel(SceneNames.MinigolfMayhemGame, SceneNames.MinigolfMayhemLevel1);
    }
    
    public void LoadSwaggySnapshots()
    {
        LoadSceneWithLevel(SceneNames.SwaggySnapshotsGame, SceneNames.SwaggySnapshotsLevel);
    }

    private void OnLoadBowlingBattle(ClickEvent _evt)
    {
        LoadSceneWithLevel(SceneNames.BowlingBattleGame, SceneNames.BowlingBattleLevel);
    }

    private void OnLoadFishingFrenzy(ClickEvent _evt)
    {
        LoadSingleScene(SceneNames.FishingFrenzy);
    }

    private void OnLoadJetskiJoyride(ClickEvent _evt)
    {
        LoadSingleScene(SceneNames.JetskiJoyride);
    }

    private void OnLoadMinigolfMayhem(ClickEvent _evt)
    {
        LoadSceneWithLevel(SceneNames.MinigolfMayhemGame, SceneNames.MinigolfMayhemLevel1);
    }

    private void OnLoadSwaggySnapshots(ClickEvent _evt)
    {
        LoadSceneWithLevel(SceneNames.SwaggySnapshotsGame, SceneNames.SwaggySnapshotsLevel);
    }

    private void LoadSingleScene(SceneNames sceneName)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(sceneName, out var sceneNameFromCollection) ? sceneNameFromCollection : throw new KeyNotFoundException());
    }

    private void LoadSceneWithLevel(SceneNames gameScene, SceneNames levelScene)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(gameScene, out var gameSceneName) ? gameSceneName : throw new KeyNotFoundException());
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(levelScene, out var levelSceneName) ? levelSceneName : throw new KeyNotFoundException(), LoadSceneMode.Additive);
    }

    public void ShowResultsScreen()
    {
        ShowResults(playerCollection.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
    }
    
    private void ShowResults(List<SO_Player> _results)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = resultsModal;

        container.Clear();

        var ordered = _results
            .OrderByDescending(_r => _r.PlayerScore.Value)
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            var data = ordered[i];
            var row = rowTemplate.CloneTree();

            row.Q<Label>("RankLabel").text = (i + 1).ToString();
            row.Q<Label>("NameLabel").text = data.Name;
            row.Q<Label>("ScoreLabel").text = data.PlayerScore.Value.ToString();

            // if (i == 0)
            //     row.AddToClassList("winner");
            //
            // if (data.IsNPC)
            //     row.AddToClassList("npc");

            container.Add(row);
        }
    }
}
