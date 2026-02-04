using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;

public class GameMenuEvents : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;
    [SerializeField] private SO_PlayerCollection currentPlayers;
    
    [SerializeField] private VisualTreeAsset rowTemplate;

    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onRestart;
    
    private UIDocument document;
    
    [Header("Menus")]
    private VisualElement pauseMenu;
    private VisualElement playerHub;
    private VisualElement resultsScreen;
    private VisualElement resultsModal;
    private VisualElement endScreenUI;
    private VisualElement controllerSelectionMenu;
    
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
    private Button resultScreenContinueButton;
    private Button endScreenRestartButton;
    private Button endScreenChangeLevelButton;
    private Button endScreenQuitButton;
    
    [Header("Controller Selection")]
    private Button controllerSelectionReadyButton;
    private Button controllerSelectionBackButton;
    private VisualElement[] m_slots;
    private int m_joinedPlayers = 0;
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        
        BindVisualElements();
        BindButtons();
        InitializeSlotElements();
        FocusButton(controllerSelectionReadyButton);
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
        resultsScreen = document.rootVisualElement.Q("results-screen-and-buttons__container");
        resultsModal = document.rootVisualElement.Q("results-screen__container");
        endScreenUI = document.rootVisualElement.Q("end-screen-menu__container");
        controllerSelectionMenu = document.rootVisualElement.Q("controller-selection-menu__container");
    }
    
    private void BindButtons()
    {
        // Pause menu buttons
        pauseMenuResumeButton = document.rootVisualElement.Q("pause-menu-resume__button") as Button;
        pauseMenuRestartButton = document.rootVisualElement.Q("pause-menu-restart__button") as Button;
        pauseMenuChangeLevelButton = document.rootVisualElement.Q("pause-menu-change-level__button") as Button;
        pauseMenuQuitButton = document.rootVisualElement.Q("pause-menu-quit__button") as Button;
        
        // Player HUB buttons
        bowlingBattleButton = document.rootVisualElement.Q("play-bowling-battle__button") as Button;
        fishingFrenzyButton = document.rootVisualElement.Q("play-fishing-frenzy__button") as Button;
        jetskiJoyrideButton = document.rootVisualElement.Q("play-jetski-joyride__button") as Button;
        minigolfMayhemButton = document.rootVisualElement.Q("play-minigolf-mayhem__button") as Button;
        swaggySnapshotsButton = document.rootVisualElement.Q("play-swaggy-snapshots__button") as Button;
        playerHUBBackButton = document.rootVisualElement.Q("player-hub-back__button") as Button;
        
        // End screen buttons
        resultScreenContinueButton = document.rootVisualElement.Q("results-screen-continue__button") as Button;
        endScreenRestartButton = document.rootVisualElement.Q("end-screen-menu-restart__button") as Button;
        endScreenChangeLevelButton = document.rootVisualElement.Q("end-screen-menu-change-level__button") as Button;
        endScreenQuitButton = document.rootVisualElement.Q("end-screen-menu-quit__button") as Button;
        
        // Controller selection menu buttons
        controllerSelectionReadyButton = document.rootVisualElement.Q("controller-selection-ready__button") as Button;
        controllerSelectionBackButton = document.rootVisualElement.Q("controller-selection-back__button") as Button;
    }

    private void RegisterButtonCallbacks()
    {        
        // Pause menu buttons
        pauseMenuResumeButton.clicked += OnResumeGameClick;
        pauseMenuRestartButton.clicked += OnRestartGameClick;
        pauseMenuChangeLevelButton.clicked += OnChangeLevelClick;
        pauseMenuQuitButton.clicked += OnQuitClick;
        
        // Player HUB buttons
        bowlingBattleButton.clicked += OnLoadBowlingBattle;
        fishingFrenzyButton.clicked += OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked += OnLoadJetskiJoyride;
        minigolfMayhemButton.clicked += OnLoadMinigolfMayhem;
        swaggySnapshotsButton.clicked += OnLoadSwaggySnapshots;
        playerHUBBackButton.clicked += OnPlayerHubBack;
        
        // End screen buttons
        resultScreenContinueButton.clicked += OnResultScreenContinue;
        endScreenRestartButton.clicked += OnRestartGameClick;
        endScreenChangeLevelButton.clicked += OnChangeLevelClick;
        endScreenQuitButton.clicked += OnQuitClick;
        
        // Controller selection buttons
        controllerSelectionReadyButton.clicked += OnControllerSelectionReadyButtonClick;
        controllerSelectionBackButton.clicked += OnControllerSelectionBackButtonClick;
    }

    private void UnregisterButtonCallbacks()
    {
        // Pause menu buttons
        pauseMenuResumeButton.clicked -= OnResumeGameClick;
        pauseMenuRestartButton.clicked -= OnRestartGameClick;
        pauseMenuChangeLevelButton.clicked -= OnChangeLevelClick;
        pauseMenuQuitButton.clicked -= OnQuitClick;

        // Player HUB buttons
        bowlingBattleButton.clicked -= OnLoadBowlingBattle;
        fishingFrenzyButton.clicked -= OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked -= OnLoadJetskiJoyride;
        minigolfMayhemButton.clicked -= OnLoadMinigolfMayhem;
        swaggySnapshotsButton.clicked -= OnLoadSwaggySnapshots;
        playerHUBBackButton.clicked -= OnPlayerHubBack;
        
        // End screen buttons
        resultScreenContinueButton.clicked -= OnResultScreenContinue;
        endScreenRestartButton.clicked -= OnRestartGameClick;
        endScreenChangeLevelButton.clicked -= OnChangeLevelClick;
        endScreenQuitButton.clicked -= OnQuitClick;
        
        // Controller selection buttons
        controllerSelectionReadyButton.clicked -= OnControllerSelectionReadyButtonClick;
        controllerSelectionBackButton.clicked -= OnControllerSelectionBackButtonClick;
    }

    public void ShowPauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        // pauseMenu.schedule.Execute(() => pauseMenuResumeButton.Focus()).StartingIn(0);
        // pauseMenu.schedule.Execute(() => pauseMenuResumeButton.Focus()).StartingIn(50);
        // pauseMenu.schedule.Execute(() => pauseMenuResumeButton.Focus()).StartingIn(100);

        FocusButton(pauseMenuResumeButton);
        Time.timeScale = 0f;
    }
    
    public void HidePauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.None;
        Time.timeScale = 1f;
    }
    
    public void ShowEndScreenUI()
    {
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }
    
    public void HideEndScreenUI()
    {
        resultsScreen.style.display = DisplayStyle.None;
        endScreenUI.style.display = DisplayStyle.None;
    }
    
    private void OnRestartGameClick()
    {
        onRestart.Invoke();
        HideEndScreenUI();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnResumeGameClick()
    {
        onUnpause.Invoke();
    }

    private void OnChangeLevelClick()
    {
        pauseMenu.style.display = DisplayStyle.None;
        endScreenUI.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
    }
    
    private void OnPlayerHubBack()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
    }

    private void OnQuitClick()
    {
        Time.timeScale = 1f;
        LoadSingleScene(SceneNames.MainMenu);
    }

    private void OnResultScreenContinue()
    {
        resultsScreen.style.display = DisplayStyle.None;
        endScreenUI.style.display = DisplayStyle.Flex;
        FocusButton(endScreenRestartButton);
    }

    public void HideControllerSelectionScreen()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
    }
    
    public void OnControllerSelectionReadyButtonClick()
    {
        HideControllerSelectionScreen();
        onGameStart.Invoke();
    }
    
    private void OnControllerSelectionBackButtonClick()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
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

    private void OnLoadBowlingBattle()
    {
        LoadSceneWithLevel(SceneNames.BowlingBattleGame, SceneNames.BowlingBattleLevel);
    }

    private void OnLoadFishingFrenzy()
    {
        LoadSingleScene(SceneNames.FishingFrenzy);
    }

    private void OnLoadJetskiJoyride()
    {
        LoadSingleScene(SceneNames.JetskiJoyrideRace);
    }

    private void OnLoadMinigolfMayhem()
    {
        LoadSceneWithLevel(SceneNames.MinigolfMayhemGame, SceneNames.MinigolfMayhemLevel1);
    }

    private void OnLoadSwaggySnapshots()
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
        ShowResults(currentPlayers.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
    }
    
    public void ShowRaceResultsScreen()
    {
        ShowRaceResults(currentPlayers.Players);
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
    
    private void ShowRaceResults(List<SO_Player> _results)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = resultsModal;

        container.Clear();

        // var ordered = _results
        //     .OrderByDescending(_r => _r.PlayerScore.Value)
        //     .ToList();

        for (int i = 0; i < _results.Count; i++)
        {
            var data = _results[i];
            var row = rowTemplate.CloneTree();

            row.Q<Label>("RankLabel").text = (i + 1).ToString();
            row.Q<Label>("NameLabel").text = data.Name;
            row.Q<Label>("ScoreLabel").text = data.PlayerScore.Value.ToString(); // <---- Hier Zeit eintragen

            // if (i == 0)
            //     row.AddToClassList("winner");
            //
            // if (data.IsNPC)
            //     row.AddToClassList("npc");

            container.Add(row);
        }
    }
    
    private void InitializeSlotElements()
    {
        m_slots = new VisualElement[4];

        for (int i = 0; i < m_slots.Length; i++)
        {
            m_slots[i] = document.rootVisualElement.Q($"slot{i}");
        }
    }
    
    public void OnPlayerJoined(PlayerInput _playerInput)
    {
        if (m_joinedPlayers >= m_slots.Length)
            return;

        var slot = m_slots[m_joinedPlayers];

        slot.RemoveFromClassList("waiting");
        slot.AddToClassList("ready");

        m_joinedPlayers++;
    }
    
    public void OnPlayerLeft(PlayerInput _playerInput)
    {
        if (m_joinedPlayers <= 0) return;
        
        var slot = m_slots[m_joinedPlayers - 1];
        
        slot.RemoveFromClassList("ready");
        slot.AddToClassList("waiting");
        
        m_joinedPlayers--;
    }
    
    private void FocusButton(VisualElement _button) => StartCoroutine(FocusButtonCoroutine(_button));
    
    private IEnumerator FocusButtonCoroutine(VisualElement _button)
    {
        yield return null;
        yield return null;
        _button.Focus();
    }
}
