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
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private UIToolkitVideo uiToolkitVideo;
    [SerializeField] private RenderTexture videoTexture;
    [SerializeField] private SO_GamePreviewClips gamePreviewClipsSO;
    private Button controllerSelectionReadyButton;
    private Button controllerSelectionBackButton;
    private VisualElement bowlingBattleHeader;
    private VisualElement bowlingBattleInstructions;
    private VisualElement bowlingBattlePreviewImage;
    private VisualElement bowlingBattleInstructionsText;
    private VisualElement fishingFrenzyHeader;
    private VisualElement fishingFrenzyInstructions;
    private VisualElement fishingFrenzyPreviewImage;
    private VisualElement fishingFrenzyInstructionsText;
    private VisualElement jetskiJoyrideRaceHeader;
    private VisualElement jetskiJoyrideRaceInstructions;
    private VisualElement jetskiJoyrideRacePreviewImage;
    private VisualElement jetskiJoyrideRaceInstructionsText;
    private VisualElement jetskiJoyrideSlalomHeader;
    private VisualElement jetskiJoyrideSlalomInstructions;
    private VisualElement jetskiJoyrideSlalomPreviewImage;
    private VisualElement jetskiJoyrideSlalomInstructionsText;
    private VisualElement minigolfMayhemClassicHeader;
    private VisualElement minigolfMayhemClassicInstructions;
    private VisualElement minigolfMayhemClassicPreviewImage;
    private VisualElement minigolfMayhemClassicInstructionsText;
    private VisualElement minigolfMayhemRaceHeader;
    private VisualElement minigolfMayhemRaceInstructions;
    private VisualElement minigolfMayhemRacePreviewImage;
    private VisualElement minigolfMayhemRaceInstructionsText;
    private VisualElement swaggySnapshotsHeader;
    private VisualElement swaggySnapshotsInstructions;
    private VisualElement swaggySnapshotsPreviewImage;
    private VisualElement swaggySnapshotsInstructionsText;
    private VisualElement hastyHurdlesHeader;
    private VisualElement hastyHurdlesInstructions;
    private VisualElement hastyHurdlesPreviewImage;
    private VisualElement hastyHurdlesInstructionsText;
    private VisualElement[] m_slots;
    private VisualElement joinInstruction;
    private VisualElement startGameInstruction;
    
    private int m_joinedPlayers = 0;
    private bool m_playerJoined = false;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        BindVisualElements();
        BindButtons();
        InitializeSlotElements();
        FocusButton(controllerSelectionReadyButton);
        
        onLevelLoaded.Invoke();
    }

    private void OnEnable()
    {
        RegisterButtonCallbacks();
        m_playerJoined = false;
        StartCoroutine(ShowOnlyJoinInstruction());
        
        onLevelLoaded.Invoke();
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
        jetskiJoyrideRaceHeader = document.rootVisualElement.Q("jj-race-controller-selection-header__container");
        jetskiJoyrideSlalomHeader = document.rootVisualElement.Q("jj-slalom-controller-selection-header__container");
        minigolfMayhemClassicHeader = document.rootVisualElement.Q("mm-classic-controller-selection-header__container");
        minigolfMayhemRaceHeader = document.rootVisualElement.Q("mm-race-controller-selection-header__container");
        swaggySnapshotsHeader = document.rootVisualElement.Q("ss-controller-selection-header__container");
        hastyHurdlesHeader = document.rootVisualElement.Q("hh-controller-selection-header__container");
        bowlingBattleInstructions = document.rootVisualElement.Q("bb-controls__container");
        fishingFrenzyInstructions = document.rootVisualElement.Q("ff-controls__container");
        jetskiJoyrideRaceInstructions = document.rootVisualElement.Q("jj-race-controls__container");
        jetskiJoyrideSlalomInstructions = document.rootVisualElement.Q("jj-slalom-controls__container");
        minigolfMayhemClassicInstructions = document.rootVisualElement.Q("mm-classic-controls__container");
        minigolfMayhemRaceInstructions = document.rootVisualElement.Q("mm-race-controls__container");
        swaggySnapshotsInstructions = document.rootVisualElement.Q("ss-controls__container");
        hastyHurdlesInstructions = document.rootVisualElement.Q("hh-controls__container");
        bowlingBattlePreviewImage = document.rootVisualElement.Q("bb-game-preview__image");
        fishingFrenzyPreviewImage = document.rootVisualElement.Q("ff-game-preview__image");
        jetskiJoyrideRacePreviewImage = document.rootVisualElement.Q("jj-race-game-preview__image");
        jetskiJoyrideSlalomPreviewImage = document.rootVisualElement.Q("jj-slalom-game-preview__image");
        minigolfMayhemClassicPreviewImage = document.rootVisualElement.Q("mm-classic-game-preview__image");
        minigolfMayhemRacePreviewImage = document.rootVisualElement.Q("mm-race-game-preview__image");
        swaggySnapshotsPreviewImage = document.rootVisualElement.Q("ss-game-preview__image");
        hastyHurdlesPreviewImage = document.rootVisualElement.Q("hh-game-preview__image");
        bowlingBattleInstructionsText = document.rootVisualElement.Q("bb-game-instruction-text__label");
        fishingFrenzyInstructionsText = document.rootVisualElement.Q("ff-game-instruction-text__label");
        jetskiJoyrideRaceInstructionsText = document.rootVisualElement.Q("jj-race-game-instruction-text__label");
        jetskiJoyrideSlalomInstructionsText = document.rootVisualElement.Q("jj-slalom-game-instruction-text__label");
        minigolfMayhemClassicInstructionsText = document.rootVisualElement.Q("mm-classic-game-instruction-text__label");
        minigolfMayhemRaceInstructionsText = document.rootVisualElement.Q("mm-race-game-instruction-text__label");
        swaggySnapshotsInstructionsText = document.rootVisualElement.Q("ss-game-instruction-text__label");
        hastyHurdlesInstructionsText = document.rootVisualElement.Q("hh-game-instruction-text__label");
        joinInstruction = document.rootVisualElement.Q("join-instruction");
        startGameInstruction = document.rootVisualElement.Q("start-game-instruction");
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
        bowlingBattleInstructionsText.style.display = DisplayStyle.Flex;
        bowlingBattlePreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.BowlingBattle, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowFishingFrenzyStartScreen()
    {
        Debug.Log("ShowFishingFrenzyStartScreen");
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        fishingFrenzyHeader.style.display = DisplayStyle.Flex;
        fishingFrenzyInstructions.style.display = DisplayStyle.Flex;
        fishingFrenzyInstructionsText.style.display = DisplayStyle.Flex;
        fishingFrenzyPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.FishingFrenzy, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowJetskiJoyrideRaceStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        jetskiJoyrideRaceHeader.style.display = DisplayStyle.Flex;
        jetskiJoyrideRaceInstructions.style.display = DisplayStyle.Flex;
        jetskiJoyrideRaceInstructionsText.style.display = DisplayStyle.Flex;
        jetskiJoyrideRacePreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.JetskiJoyrideRace, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }
    
    public void ShowJetskiJoyrideSlalomStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        jetskiJoyrideRaceHeader.style.display = DisplayStyle.Flex;
        jetskiJoyrideRaceInstructions.style.display = DisplayStyle.Flex;
        jetskiJoyrideRaceInstructionsText.style.display = DisplayStyle.Flex;
        jetskiJoyrideRacePreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.JetskiJoyrideSlalom, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowMinigolfMayhemClassicStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        minigolfMayhemClassicHeader.style.display = DisplayStyle.Flex;
        minigolfMayhemClassicInstructions.style.display = DisplayStyle.Flex;
        minigolfMayhemClassicInstructionsText.style.display = DisplayStyle.Flex;
        minigolfMayhemClassicPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.MinigolfMayhemClassic, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }
    
    public void ShowMinigolfMayhemRaceStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        minigolfMayhemClassicHeader.style.display = DisplayStyle.Flex;
        minigolfMayhemClassicInstructions.style.display = DisplayStyle.Flex;
        minigolfMayhemClassicInstructionsText.style.display = DisplayStyle.Flex;
        minigolfMayhemClassicPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.MinigolfMayhemRace, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowSwaggySnapshotsStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        swaggySnapshotsHeader.style.display = DisplayStyle.Flex;
        swaggySnapshotsInstructions.style.display = DisplayStyle.Flex;
        swaggySnapshotsInstructionsText.style.display = DisplayStyle.Flex;
        swaggySnapshotsPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.SwaggySnapshots, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowHastyHurdlesStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        hastyHurdlesHeader.style.display = DisplayStyle.Flex;
        hastyHurdlesInstructions.style.display = DisplayStyle.Flex;
        hastyHurdlesInstructionsText.style.display = DisplayStyle.Flex;
        hastyHurdlesPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.HastyHurdles, out var clip) ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    private void HideControllerSelectionScreen()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
    }

    private void OnControllerSelectionReadyButtonClick()
    {
        if (!m_playerJoined) return;

        StopCoroutine(ShowStartAndJoinInstruction());
        StopCoroutine(ShowJoinAndStartInstruction());
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
        jetskiJoyrideRaceHeader.style.display = DisplayStyle.None;
        minigolfMayhemClassicHeader.style.display = DisplayStyle.None;
        swaggySnapshotsHeader.style.display = DisplayStyle.None;
        hastyHurdlesHeader.style.display = DisplayStyle.None;
        bowlingBattleInstructions.style.display = DisplayStyle.None;
        fishingFrenzyInstructions.style.display = DisplayStyle.None;
        jetskiJoyrideRaceInstructions.style.display = DisplayStyle.None;
        minigolfMayhemClassicInstructions.style.display = DisplayStyle.None;
        swaggySnapshotsInstructions.style.display = DisplayStyle.None;
        hastyHurdlesInstructions.style.display = DisplayStyle.None;
        bowlingBattlePreviewImage.style.display = DisplayStyle.None;
        fishingFrenzyPreviewImage.style.display = DisplayStyle.None;
        jetskiJoyrideRacePreviewImage.style.display = DisplayStyle.None;
        minigolfMayhemClassicPreviewImage.style.display = DisplayStyle.None;
        swaggySnapshotsPreviewImage.style.display = DisplayStyle.None;
        hastyHurdlesPreviewImage.style.display = DisplayStyle.None;
        bowlingBattleInstructionsText.style.display = DisplayStyle.None;
        fishingFrenzyInstructionsText.style.display = DisplayStyle.None;
        jetskiJoyrideRaceInstructionsText.style.display = DisplayStyle.None;
        minigolfMayhemClassicInstructionsText.style.display = DisplayStyle.None;
        swaggySnapshotsInstructionsText.style.display = DisplayStyle.None;
        hastyHurdlesInstructionsText.style.display = DisplayStyle.None;
    }
    
    private IEnumerator ShowOnlyJoinInstruction()
    {
        joinInstruction.style.display = DisplayStyle.Flex;

        if (joinInstruction.style.scale.value.value.y <= 1f)
        {
            yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1,0,1), new Vector3(1,1,1), animDuration);
        }

        yield return new WaitForSecondsRealtime(holdDuration);

        yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1,1,1), new Vector3(1,0,1), animDuration);

        joinInstruction.style.display = DisplayStyle.None;

        if (m_playerJoined)
        {
            StopCoroutine(ShowOnlyJoinInstruction());
            StartCoroutine(ShowStartAndJoinInstruction());
            yield break;
        }
        
        StartCoroutine(ShowOnlyJoinInstruction());
    }
    
    private IEnumerator ShowJoinAndStartInstruction()
    {
        joinInstruction.style.display = DisplayStyle.Flex;

        if (joinInstruction.style.scale.value.value.y <= 1f)
        {
            yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1,0,1), new Vector3(1,1,1), animDuration);
        }

        yield return new WaitForSecondsRealtime(holdDuration);

        yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1,1,1), new Vector3(1,0,1), animDuration);

        joinInstruction.style.display = DisplayStyle.None;

        StartCoroutine(ShowStartAndJoinInstruction());
    }

    private IEnumerator ShowStartAndJoinInstruction()
    {
        startGameInstruction.style.display = DisplayStyle.Flex;

        yield return AnimateScaleCoroutine(startGameInstruction, new Vector3(1,0,1), new Vector3(1,1,1), animDuration);

        yield return new WaitForSecondsRealtime(holdDuration);

        yield return AnimateScaleCoroutine(startGameInstruction, new Vector3(1,1,1), new Vector3(1,0,1), animDuration);

        startGameInstruction.style.display = DisplayStyle.None;

        StartCoroutine(ShowJoinAndStartInstruction());
    }

    private IEnumerator AnimateScaleCoroutine(VisualElement element, Vector3 from, Vector3 to, float duration)
    {
        var elapsed = 0f;

        element.style.scale = new Scale(from);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / duration);

            t = Mathf.SmoothStep(0f, 1f, t);

            Vector3 current = Vector3.Lerp(from, to, t);
            element.style.scale = new Scale(current);

            yield return null;
        }

        element.style.scale = new Scale(to);
    }

    private void FreezeTimeScale() => Time.timeScale = 0;
    private void UnfreezeTimeScale() => Time.timeScale = 1;
}