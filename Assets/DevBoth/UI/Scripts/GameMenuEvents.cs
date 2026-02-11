using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;
using Player;
using Player.Collections;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Button = UnityEngine.UIElements.Button;

public class GameMenuEvents : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;
    [SerializeField] private bool racingGame = false;

    [HideIf("racingGame")]
    [SerializeField] private SO_PlayerCollection currentPlayers;

    [ShowIf("racingGame")]
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayersRacing;

    [SerializeField] private VisualTreeAsset rowTemplate;

    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onRestart;
    [SerializeField] private UnityEvent onLevelLoaded;

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
    private Button hastyHurdlesButton;
    private Button playerHUBBackButton;

    [Header("End Screen Buttons")]
    private Button resultScreenContinueButton;

    private Button endScreenRestartButton;
    private Button endScreenChangeLevelButton;
    private Button endScreenQuitButton;

    [Header("Controller Selection")]
    private Button controllerSelectionReadyButton;

    private Button controllerSelectionBackButton;
    private VisualElement bowlingBattleHeader;
    private VisualElement fishingFrenzyHeader;
    private VisualElement jetskiJoyrideHeader;
    private VisualElement minigolfMayhemHeader;
    private VisualElement swaggySnapshotsHeader;
    private VisualElement hastyHurdlesHeader;
    private VisualElement bowlingBattleInstructions;
    private VisualElement fishingFrenzyInstructions;
    private VisualElement jetskiJoyrideInstructions;
    private VisualElement minigolfMayhemInstructions;
    private VisualElement swaggySnapshotsInstructions;
    private VisualElement hastyHurdlesInstructions;
    private VisualElement[] m_slots;
    private int m_joinedPlayers = 0;

    private bool m_playerJoined = false;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        BindVisualElements();
        BindButtons();
        InitializeSlotElements();
        onLevelLoaded.Invoke();
        FocusButton(controllerSelectionReadyButton);
    }

    private void OnEnable()
    {
        onLevelLoaded.Invoke();
        RegisterButtonCallbacks();
        m_playerJoined = false;
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
        hastyHurdlesButton = document.rootVisualElement.Q("play-hasty-hurdles__button") as Button;
        playerHUBBackButton = document.rootVisualElement.Q("player-hub-back__button") as Button;

        // End screen buttons
        resultScreenContinueButton = document.rootVisualElement.Q("results-screen-continue__button") as Button;
        endScreenRestartButton = document.rootVisualElement.Q("end-screen-menu-restart__button") as Button;
        endScreenChangeLevelButton = document.rootVisualElement.Q("end-screen-menu-change-level__button") as Button;
        endScreenQuitButton = document.rootVisualElement.Q("end-screen-menu-quit__button") as Button;

        // Controller selection menu
        controllerSelectionReadyButton = document.rootVisualElement.Q("controller-selection-ready__button") as Button;
        controllerSelectionBackButton = document.rootVisualElement.Q("controller-selection-back__button") as Button;
        bowlingBattleHeader = document.rootVisualElement.Q("bb-controller-selection-header__container");
        fishingFrenzyHeader = document.rootVisualElement.Q("ff-controller-selection-header__container");
        jetskiJoyrideHeader = document.rootVisualElement.Q("jj-controller-selection-header__container");
        minigolfMayhemHeader = document.rootVisualElement.Q("mm-controller-selection-header__container");
        swaggySnapshotsHeader = document.rootVisualElement.Q("ss-controller-selection-header__container");
        hastyHurdlesHeader = document.rootVisualElement.Q("hh-controller-selection-header__container");
        bowlingBattleInstructions = document.rootVisualElement.Q("bb-instructions__container");
        fishingFrenzyInstructions = document.rootVisualElement.Q("ff-instructions__container");
        jetskiJoyrideInstructions = document.rootVisualElement.Q("jj-instructions__container");
        minigolfMayhemInstructions = document.rootVisualElement.Q("mm-instructions__container");
        swaggySnapshotsInstructions = document.rootVisualElement.Q("ss-instructions__container");
        hastyHurdlesInstructions = document.rootVisualElement.Q("hh-instructions__container");
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
        hastyHurdlesButton.clicked += OnLoadHastyHurdles;
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
        hastyHurdlesButton.clicked -= OnLoadHastyHurdles;
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
        FocusButton(pauseMenuResumeButton);
        Time.timeScale = 0f;
    }

    public void HidePauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.None;
        UnfreezeTimeScale();
    }

    public void ShowEndScreenUI()
    {
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }

    private void HideEndScreenUI()
    {
        resultsScreen.style.display = DisplayStyle.None;
        endScreenUI.style.display = DisplayStyle.None;
    }

    private void OnRestartGameClick()
    {
        onRestart.Invoke();
        HideEndScreenUI();
        FreezeTimeScale();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void OnResumeGameClick()
    {
        HidePauseMenu();
        onUnpause.Invoke();
    }

    private void OnChangeLevelClick()
    {
        pauseMenu.style.display = DisplayStyle.None;
        endScreenUI.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        FocusButton(bowlingBattleButton);
    }

    private void OnPlayerHubBack()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
        FocusButton(pauseMenuResumeButton);
    }

    private void OnQuitClick()
    {
        UnfreezeTimeScale();
        LoadSingleScene(SceneNames.MainMenu);
    }

    private void OnResultScreenContinue()
    {
        resultsScreen.style.display = DisplayStyle.None;
        endScreenUI.style.display = DisplayStyle.Flex;
        FocusButton(endScreenRestartButton);
    }

    public void ShowBowlingBattleStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        bowlingBattleHeader.style.display = DisplayStyle.Flex;
        bowlingBattleInstructions.style.display = DisplayStyle.Flex;
    }

    public void ShowFishingFrenzyStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        fishingFrenzyHeader.style.display = DisplayStyle.Flex;
        fishingFrenzyInstructions.style.display = DisplayStyle.Flex;
    }

    public void ShowJetskiJoyrideStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        jetskiJoyrideHeader.style.display = DisplayStyle.Flex;
        jetskiJoyrideInstructions.style.display = DisplayStyle.Flex;
    }

    public void ShowMinigolfMayhemStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        minigolfMayhemHeader.style.display = DisplayStyle.Flex;
        minigolfMayhemInstructions.style.display = DisplayStyle.Flex;
    }

    public void ShowSwaggySnapshotsStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        swaggySnapshotsHeader.style.display = DisplayStyle.Flex;
        swaggySnapshotsInstructions.style.display = DisplayStyle.Flex;
    }

    public void ShowHastyHurdlesStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        hastyHurdlesHeader.style.display = DisplayStyle.Flex;
        hastyHurdlesInstructions.style.display = DisplayStyle.Flex;
    }

    private void HideControllerSelectionScreen()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
    }

    private void OnControllerSelectionReadyButtonClick()
    {
        if (!m_playerJoined) return;

        UnfreezeTimeScale();
        HideControllerSelectionScreen();
        onGameStart.Invoke();
    }

    private void OnControllerSelectionBackButtonClick()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
        LoadSingleScene(SceneNames.MainMenu);
    }

    private void OnLoadBowlingBattle()
    {
        LoadSingleScene(SceneNames.BowlingBattleGame);
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
        LoadSingleScene(SceneNames.MinigolfMayhemGame);
    }

    private void OnLoadSwaggySnapshots()
    {
        LoadSingleScene(SceneNames.SwaggySnapshotsGame);
    }

    private void OnLoadHastyHurdles()
    {
        LoadSingleScene(SceneNames.HastyHurdles);
    }

    private void LoadSingleScene(SceneNames sceneName)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(sceneName, out var sceneNameFromCollection)
            ? sceneNameFromCollection
            : throw new KeyNotFoundException());
    }

    private void LoadSceneWithLevel(SceneNames gameScene, SceneNames levelScene)
    {
        SceneManager.LoadScene(
            sceneCollection.Scenes.TryGetValue(gameScene, out var gameSceneName) ? gameSceneName : throw new KeyNotFoundException());
        SceneManager.LoadScene(
            sceneCollection.Scenes.TryGetValue(levelScene, out var levelSceneName) ? levelSceneName : throw new KeyNotFoundException(),
            LoadSceneMode.Additive);
    }

    public void ShowResultsScreen()
    {
        ShowResults(currentPlayers.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }

    public void ShowRaceResultsScreen()
    {
        ShowRaceResults(currentPlayersRacing.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }

    private void ShowResults(List<SO_Player> _results)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = resultsModal;

        container.Clear();

        var ordered = _results
            .OrderByDescending(_r => _r.PlayerScore.Value)
            .ToList();

        var currentRank = 1;
        var previousScore = float.MinValue;

        for (var i = 0; i < ordered.Count; i++)
        {
            var data = ordered[i];
            var row = rowTemplate.CloneTree();

            var currentScore = data.PlayerScore.Value;

            if (i > 0 && currentScore != previousScore)
            {
                currentRank = i + 1;
            }

            row.Q<Label>("RankLabel").text = $"{currentRank}";
            row.Q<Label>("NameLabel").text = data.Name;
            row.Q<Label>("ScoreLabel").text = data.PlayerScore.Value.ToString();

            previousScore = currentScore;

            // if (i == 0)
            //     row.AddToClassList("winner");
            //
            // if (data.IsNPC)
            //     row.AddToClassList("npc");

            container.Add(row);
        }
    }

    private void ShowRaceResults(List<SO_PlayerRacingGames> _results)
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var container = resultsModal;

        container.Clear();

        var ordered = _results
            .OrderByDescending(_r => _r.PlayerScore.Value)
            .ToList();

        ordered.Reverse();

        for (int i = 0; i < ordered.Count; i++)
        {
            var data = ordered[i];
            var row = rowTemplate.CloneTree();

            row.Q<Label>("RankLabel").text = (i + 1).ToString();
            row.Q<Label>("NameLabel").text = data.Name;

            if (!string.IsNullOrEmpty(data.Time))
                row.Q<Label>("ScoreLabel").text = data.PlayerScore.Value.ToString(); // <---- Hier Zeit eintragen
            else
                row.Q<Label>("ScoreLabel").text = "";
            
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

        m_playerJoined = true;
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

    private void HideAllGameHeadersAndInstructions()
    {
        bowlingBattleHeader.style.display = DisplayStyle.None;
        fishingFrenzyHeader.style.display = DisplayStyle.None;
        jetskiJoyrideHeader.style.display = DisplayStyle.None;
        minigolfMayhemHeader.style.display = DisplayStyle.None;
        swaggySnapshotsHeader.style.display = DisplayStyle.None;
        hastyHurdlesHeader.style.display = DisplayStyle.None;
        bowlingBattleInstructions.style.display = DisplayStyle.None;
        fishingFrenzyInstructions.style.display = DisplayStyle.None;
        jetskiJoyrideInstructions.style.display = DisplayStyle.None;
        minigolfMayhemInstructions.style.display = DisplayStyle.None;
        swaggySnapshotsInstructions.style.display = DisplayStyle.None;
        hastyHurdlesInstructions.style.display = DisplayStyle.None;
    }

    private void FreezeTimeScale() => Time.timeScale = 0;
    private void UnfreezeTimeScale() => Time.timeScale = 1;
}