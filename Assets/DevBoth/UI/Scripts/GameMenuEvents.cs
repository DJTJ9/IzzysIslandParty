using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Linq;
using Player;
using Player.Collections;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Button = UnityEngine.UIElements.Button;

public class GameMenuEvents : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;
    [SerializeField] private bool racingGame;
    [SerializeField] private bool swaggySnapshots;
    
    [FoldoutGroup("Mouse Sensitivity", expanded: false)]
    [SerializeField] private SO_FloatVariable mouseSensitivityX;
    [SerializeField] private SO_FloatVariable mouseSensitivityY;

    [HideIf("racingGame"), HideIf("swaggySnapshots")]
    [SerializeField] private SO_PlayerCollection currentPlayers;

    [ShowIf("racingGame")]
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayersRacing;
    
    [ShowIf("swaggySnapshots")]
    [SerializeField] private SO_SwaggySnapshotsPlayerCollection currentPlayersSwaggySnapshots;

    [SerializeField] private VisualTreeAsset rowTemplate;

    private UIDocument document;
    private VisualElement root;

    [Header("Menus")]
    private VisualElement pauseMenu;
    private VisualElement settingsMenu;
    private VisualElement playerHub;
    private VisualElement jetskiJoyrideModusSelection;
    private VisualElement minigolfMayhemModusSelection;
    private VisualElement resultsScreen;
    private VisualElement resultsModal;
    private VisualElement endScreenUI;
    private VisualElement controllerSelectionMenu;

    [Header("Pause Menu Buttons")]
    private Button pauseMenuResumeButton;
    private Button pauseMenuRestartButton;
    private Button pauseMenuChangeLevelButton;
    private Button pauseMenuSettingsButton;
    private Button pauseMenuQuitButton;

    [Header("Settings Menu Elements")]
    private Slider mouseSensitivitySlider;
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider soundFXVolumeSlider;
    private Button settingsBackButton;
    
    [Header("Player HUB Buttons")]
    private Button bowlingBattleButton;
    private Button fishingFrenzyButton;
    private Button jetskiJoyrideButton;
    private Button jetskiJoyrideRaceButton;
    private Button jetskiJoyrideSlalomButton;
    private Button jetskiJoyrideModusSelectionBackButton;
    private Button minigolfMayhemButton;
    private Button minigolfMayhemClassicButton;
    private Button minigolfMayhemRaceButton;
    private Button minigolfMayhemModusSelectionBackButton;
    private Button swaggySnapshotsButton;
    private Button hastyHurdlesButton;
    private Button playerHUBBackButton;

    [Header("End Screen Buttons")]
    private Button resultScreenContinueButton;
    private Button endScreenRestartButton;
    private Button endScreenChangeLevelButton;
    private Button endScreenQuitButton;

    #region ControllerSelectionMenu

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

    #endregion

    [FoldoutGroup("Events", expanded: false)]
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onRestart;
    [SerializeField] private UnityEvent onLevelLoaded;
    
    public static event Action<float> OnMouseSettingsChanged;

    private int m_joinedPlayers;
    private bool m_playerJoined;

    private bool sorted;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        BindVisualElements();
        BindButtons();
        InitializeSlotElements();
        FocusButton(controllerSelectionReadyButton);

        onLevelLoaded.Invoke();
    }

    private void OnEnable()
    {
        RegisterButtonCallbacks();
        SetUpPlayerHUBNavigation();
        m_playerJoined = false;
        StartCoroutine(ShowOnlyJoinInstruction());

        // onLevelLoaded.Invoke();
    }

    private void OnDisable()
    {
        UnregisterButtonCallbacks();
        StopAllCoroutines();
    }

    private void BindVisualElements()
    {
        pauseMenu = root.Q("pause-menu__container");
        settingsMenu = root.Q("settings-menu__container");
        playerHub = root.Q("player-hub__container");
        jetskiJoyrideModusSelection = root.Q("jetski-joyride-modus-selection__container");
        minigolfMayhemModusSelection = root.Q("minigolf-mayhem-modus-selection__container");
        resultsScreen = root.Q("results-screen-and-buttons__container");
        resultsModal = root.Q("results-screen__container");
        endScreenUI = root.Q("end-screen-menu__container");
        controllerSelectionMenu = root.Q("controller-selection-menu__container");
    }

    private void BindButtons()
    {
        // Pause menu buttons
        pauseMenuResumeButton = root.Q("pause-menu-resume__button") as Button;
        pauseMenuRestartButton = root.Q("pause-menu-restart__button") as Button;
        pauseMenuChangeLevelButton = root.Q("pause-menu-change-level__button") as Button;
        pauseMenuSettingsButton = root.Q("pause-menu-settings__button") as Button;
        pauseMenuQuitButton = root.Q("pause-menu-quit__button") as Button;
        
        // Settings menu elements
        mouseSensitivitySlider = root.Q<Slider>("settings-mouse-sensitivity__slider");
        masterVolumeSlider = root.Q<Slider>("settings-master-volume__slider");
        musicVolumeSlider = root.Q<Slider>("settings-music-volume__slider");
        soundFXVolumeSlider = root.Q<Slider>("settings-soundfx-volume__slider");
        settingsBackButton = root.Q("settings-menu-back__button") as Button;

        // Player HUB buttons
        bowlingBattleButton = root.Q("play-bowling-battle__button") as Button;
        fishingFrenzyButton = root.Q("play-fishing-frenzy__button") as Button;
        jetskiJoyrideButton = root.Q("play-jetski-joyride__button") as Button;
        jetskiJoyrideRaceButton = root.Q("play-jetski-joyride-race__button") as Button;
        jetskiJoyrideSlalomButton = root.Q("play-jetski-joyride-slalom__button") as Button;
        jetskiJoyrideModusSelectionBackButton = root.Q("jetski-joyride-modus-selection-back__button") as Button;
        minigolfMayhemButton = root.Q("play-minigolf-mayhem__button") as Button;
        minigolfMayhemClassicButton = root.Q("play-minigolf-mayhem-classic__button") as Button;
        minigolfMayhemRaceButton = root.Q("play-minigolf-mayhem-race__button") as Button;
        minigolfMayhemModusSelectionBackButton = root.Q("minigolf-mayhem-modus-selection-back__button") as Button;
        swaggySnapshotsButton = root.Q("play-swaggy-snapshots__button") as Button;
        hastyHurdlesButton = root.Q("play-hasty-hurdles__button") as Button;
        playerHUBBackButton = root.Q("player-hub-back__button") as Button;

        // End screen buttons
        resultScreenContinueButton = root.Q("results-screen-continue__button") as Button;
        endScreenRestartButton = root.Q("end-screen-menu-restart__button") as Button;
        endScreenChangeLevelButton = root.Q("end-screen-menu-change-level__button") as Button;
        endScreenQuitButton = root.Q("end-screen-menu-quit__button") as Button;

        // Controller selection menu
        controllerSelectionReadyButton = root.Q("controller-selection-ready__button") as Button;
        controllerSelectionBackButton = root.Q("controller-selection-back__button") as Button;
        bowlingBattleHeader = root.Q("bb-controller-selection-header__container");
        fishingFrenzyHeader = root.Q("ff-controller-selection-header__container");
        jetskiJoyrideRaceHeader = root.Q("jj-race-controller-selection-header__container");
        jetskiJoyrideSlalomHeader = root.Q("jj-slalom-controller-selection-header__container");
        minigolfMayhemClassicHeader = root.Q("mm-classic-controller-selection-header__container");
        minigolfMayhemRaceHeader = root.Q("mm-race-controller-selection-header__container");
        swaggySnapshotsHeader = root.Q("ss-controller-selection-header__container");
        hastyHurdlesHeader = root.Q("hh-controller-selection-header__container");
        bowlingBattleInstructions = root.Q("bb-controls__container");
        fishingFrenzyInstructions = root.Q("ff-controls__container");
        jetskiJoyrideRaceInstructions = root.Q("jj-race-controls__container");
        jetskiJoyrideSlalomInstructions = root.Q("jj-slalom-controls__container");
        minigolfMayhemClassicInstructions = root.Q("mm-classic-controls__container");
        minigolfMayhemRaceInstructions = root.Q("mm-race-controls__container");
        swaggySnapshotsInstructions = root.Q("ss-controls__container");
        hastyHurdlesInstructions = root.Q("hh-controls__container");
        bowlingBattlePreviewImage = root.Q("bb-game-preview__image");
        fishingFrenzyPreviewImage = root.Q("ff-game-preview__image");
        jetskiJoyrideRacePreviewImage = root.Q("jj-race-game-preview__image");
        jetskiJoyrideSlalomPreviewImage = root.Q("jj-slalom-game-preview__image");
        minigolfMayhemClassicPreviewImage = root.Q("mm-classic-game-preview__image");
        minigolfMayhemRacePreviewImage = root.Q("mm-race-game-preview__image");
        swaggySnapshotsPreviewImage = root.Q("ss-game-preview__image");
        hastyHurdlesPreviewImage = root.Q("hh-game-preview__image");
        bowlingBattleInstructionsText = root.Q("bb-game-instruction-text__label");
        fishingFrenzyInstructionsText = root.Q("ff-game-instruction-text__label");
        jetskiJoyrideRaceInstructionsText = root.Q("jj-race-game-instruction-text__label");
        jetskiJoyrideSlalomInstructionsText = root.Q("jj-slalom-game-instruction-text__label");
        minigolfMayhemClassicInstructionsText = root.Q("mm-classic-game-instruction-text__label");
        minigolfMayhemRaceInstructionsText = root.Q("mm-race-game-instruction-text__label");
        swaggySnapshotsInstructionsText = root.Q("ss-game-instruction-text__label");
        hastyHurdlesInstructionsText = root.Q("hh-game-instruction-text__label");
        joinInstruction = root.Q("join-instruction");
        startGameInstruction = root.Q("start-game-instruction");
    }

    private void RegisterButtonCallbacks()
    {
        // Pause menu buttons
        pauseMenuResumeButton.clicked += OnResumeGameClick;
        pauseMenuRestartButton.clicked += OnRestartGameClick;
        pauseMenuChangeLevelButton.clicked += OnChangeLevelClick;
        pauseMenuSettingsButton.clicked += OnSettingsButtonClick;
        pauseMenuQuitButton.clicked += OnQuitClick;
        
        // Settings menu buttons
        mouseSensitivitySlider.RegisterValueChangedCallback(OnMouseSensitivityChange);
        masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChange);
        musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChange);
        soundFXVolumeSlider.RegisterValueChangedCallback(OnSoundFXVolumeChange);
        settingsBackButton.clicked += OnSettingsBackButtonClick;

        // Player HUB buttons
        bowlingBattleButton.clicked += OnLoadBowlingBattle;
        fishingFrenzyButton.clicked += OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked += OnSelectJetskiJoyride;
        jetskiJoyrideRaceButton.clicked += OnLoadJetskiJoyrideRace;
        jetskiJoyrideSlalomButton.clicked += OnLoadJetskiJoyrideSlalom;
        jetskiJoyrideModusSelectionBackButton.clicked += OnJetskiJoyrideBack;
        minigolfMayhemButton.clicked += OnSelectMinigolfMayhem;
        minigolfMayhemClassicButton.clicked += OnLoadMinigolfMayhemClassic;
        minigolfMayhemRaceButton.clicked += OnLoadMinigolfMayhemRace;
        minigolfMayhemModusSelectionBackButton.clicked += OnMinigolfMayhemBack;
        swaggySnapshotsButton.clicked += OnLoadSwaggySnapshots;
        hastyHurdlesButton.clicked += OnLoadHastyHurdles;
        playerHUBBackButton.clicked += OnPlayerHubBack;

        // End screen buttons
        resultScreenContinueButton.clicked += OnResultScreenContinue;
        endScreenRestartButton.clicked += OnRestartGameClick;
        endScreenChangeLevelButton.clicked += OnChangeLevelClick;
        endScreenQuitButton.clicked += OnQuitClick;

        // // Controller selection buttons
        // controllerSelectionReadyButton.clicked += OnControllerSelectionReady;
        // controllerSelectionBackButton.clicked += OnControllerSelectionBackButtonClick;
    }

    private void UnregisterButtonCallbacks()
    {
        // Pause menu buttons
        pauseMenuResumeButton.clicked -= OnResumeGameClick;
        pauseMenuRestartButton.clicked -= OnRestartGameClick;
        pauseMenuChangeLevelButton.clicked -= OnChangeLevelClick;
        pauseMenuSettingsButton.clicked -= OnSettingsButtonClick;
        pauseMenuQuitButton.clicked -= OnQuitClick;
        
        // Settings menu buttons
        mouseSensitivitySlider.UnregisterValueChangedCallback(OnMouseSensitivityChange);
        masterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChange);
        musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChange);
        soundFXVolumeSlider.UnregisterValueChangedCallback(OnSoundFXVolumeChange);
        settingsBackButton.clicked -= OnSettingsBackButtonClick;

        // Player HUB buttons
        bowlingBattleButton.clicked -= OnLoadBowlingBattle;
        fishingFrenzyButton.clicked -= OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked -= OnSelectJetskiJoyride;
        jetskiJoyrideRaceButton.clicked -= OnLoadJetskiJoyrideRace;
        jetskiJoyrideSlalomButton.clicked -= OnLoadJetskiJoyrideSlalom;
        jetskiJoyrideModusSelectionBackButton.clicked -= OnJetskiJoyrideBack;
        minigolfMayhemButton.clicked -= OnSelectMinigolfMayhem;
        minigolfMayhemClassicButton.clicked -= OnLoadMinigolfMayhemClassic;
        minigolfMayhemRaceButton.clicked -= OnLoadMinigolfMayhemRace;
        minigolfMayhemModusSelectionBackButton.clicked -= OnMinigolfMayhemBack;
        swaggySnapshotsButton.clicked -= OnLoadSwaggySnapshots;
        hastyHurdlesButton.clicked -= OnLoadHastyHurdles;
        playerHUBBackButton.clicked -= OnPlayerHubBack;

        // End screen buttons
        resultScreenContinueButton.clicked -= OnResultScreenContinue;
        endScreenRestartButton.clicked -= OnRestartGameClick;
        endScreenChangeLevelButton.clicked -= OnChangeLevelClick;
        endScreenQuitButton.clicked -= OnQuitClick;

        // // Controller selection buttons
        // controllerSelectionReadyButton.clicked -= OnControllerSelectionReady;
        // controllerSelectionBackButton.clicked -= OnControllerSelectionBackButtonClick;
    }

    public void ShowPauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        FocusButton(pauseMenuResumeButton);
        FreezeTimeScale();
    }

    public void HidePauseMenu()
    {
        pauseMenu.style.display = DisplayStyle.None;
        UnfreezeTimeScale();
    }
    
    private void OnMouseSensitivityChange(ChangeEvent<float> _evt)
    {
        mouseSensitivityX.Value = _evt.newValue;
        mouseSensitivityY.Value = _evt.newValue;
        OnMouseSettingsChanged?.Invoke(_evt.newValue);
    }
    
    private void OnSettingsBackButtonClick()
    {
        settingsMenu.style.display = DisplayStyle.None;
        pauseMenu.style.display = DisplayStyle.Flex;
        FocusButton(pauseMenuSettingsButton);
    }
    
    private void OnMasterVolumeChange(ChangeEvent<float> _evt)
    {
        SoundMixerManager.Instance.SetMasterVolume(_evt.newValue);
    }
    
    private void OnMusicVolumeChange(ChangeEvent<float> _evt)
    {
        SoundMixerManager.Instance.SetMusicVolume(_evt.newValue);
    }
    
    private void OnSoundFXVolumeChange(ChangeEvent<float> _evt)
    {
        SoundMixerManager.Instance.SetSoundFXVolume(_evt.newValue);
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
        AsyncLevelLoader.Instance.RestartLevel();
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
    
    private void OnSettingsButtonClick()
    {
        pauseMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
        FocusButton(mouseSensitivitySlider);
    }

    private void OnPlayerHubBack()
    {
        pauseMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
        FocusButton(pauseMenuResumeButton);
    }

    private void OnQuitClick()
    {
        pauseMenu.style.display = DisplayStyle.None;
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
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.BowlingBattle, out var clip)
            ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowFishingFrenzyStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        fishingFrenzyHeader.style.display = DisplayStyle.Flex;
        fishingFrenzyInstructions.style.display = DisplayStyle.Flex;
        fishingFrenzyInstructionsText.style.display = DisplayStyle.Flex;
        fishingFrenzyPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.FishingFrenzy, out var clip)
            ? clip : throw new KeyNotFoundException());
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
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.JetskiJoyrideRace, out var clip)
            ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowJetskiJoyrideSlalomStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        jetskiJoyrideSlalomHeader.style.display = DisplayStyle.Flex;
        jetskiJoyrideSlalomInstructions.style.display = DisplayStyle.Flex;
        jetskiJoyrideSlalomInstructionsText.style.display = DisplayStyle.Flex;
        jetskiJoyrideSlalomPreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.JetskiJoyrideSlalom, out var clip)
            ? clip : throw new KeyNotFoundException());
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
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.MinigolfMayhemClassic, out var clip)
            ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    public void ShowMinigolfMayhemRaceStartScreen()
    {
        FreezeTimeScale();
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
        HideAllGameHeadersAndInstructions();
        FocusButton(controllerSelectionReadyButton);
        minigolfMayhemRaceHeader.style.display = DisplayStyle.Flex;
        minigolfMayhemRaceInstructions.style.display = DisplayStyle.Flex;
        minigolfMayhemRaceInstructionsText.style.display = DisplayStyle.Flex;
        minigolfMayhemRacePreviewImage.style.display = DisplayStyle.Flex;
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.MinigolfMayhemRace, out var clip)
            ? clip : throw new KeyNotFoundException());
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
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.SwaggySnapshots, out var clip)
            ? clip : throw new KeyNotFoundException());
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
        uiToolkitVideo.SetVideoClip(gamePreviewClipsSO.PreviewClips.TryGetValue(GamePreviewClips.HastyHurdles, out var clip)
            ? clip : throw new KeyNotFoundException());
        uiToolkitVideo.PlayVideo();
    }

    private void HideControllerSelectionScreen()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
    }

    public void OnControllerSelectionReady()
    {
        if (!m_playerJoined) return;

        StopCoroutine(ShowStartAndJoinInstruction());
        StopCoroutine(ShowJoinAndStartInstruction());
        uiToolkitVideo.StopVideo();
        UnfreezeTimeScale();
        HideControllerSelectionScreen();
        // onGameStart.Invoke();
    }

    // private void OnControllerSelectionBackButtonClick()
    // {
    //     controllerSelectionMenu.style.display = DisplayStyle.None;
    //     LoadSingleScene(SceneNames.MainMenu);
    // }

    private void OnLoadBowlingBattle()
    {
        LoadSingleScene(SceneNames.BowlingBattle);
    }

    private void OnLoadFishingFrenzy()
    {
        LoadSingleScene(SceneNames.FishingFrenzy);
    }

    private void OnSelectJetskiJoyride()
    {
        jetskiJoyrideModusSelection.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
        FocusButton(jetskiJoyrideRaceButton);
    }

    private void OnLoadJetskiJoyrideRace()
    {
        LoadSingleScene(SceneNames.JetskiJoyrideRace);
    }
    
    private void OnLoadJetskiJoyrideSlalom()
    {
        LoadSingleScene(SceneNames.JetskiJoyrideSlalom);
    }

    private void OnJetskiJoyrideBack()
    {
        jetskiJoyrideModusSelection.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        FocusButton(bowlingBattleButton);
    }

    private void OnSelectMinigolfMayhem()
    {
        minigolfMayhemModusSelection.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
        FocusButton(minigolfMayhemClassicButton);
    }

    private void OnLoadMinigolfMayhemClassic()
    {
        LoadSingleScene(SceneNames.MinigolfMayhemClassic);
    }

    private void OnLoadMinigolfMayhemRace()
    {
        LoadSingleScene(SceneNames.MinigolfMayhemRace);
    }

    private void OnLoadSwaggySnapshots()
    {
        LoadSingleScene(SceneNames.SwaggySnapshots);
    }

    private void OnMinigolfMayhemBack()
    {
        minigolfMayhemModusSelection.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        FocusButton(bowlingBattleButton);
    }

    private void OnLoadHastyHurdles()
    {
        LoadSingleScene(SceneNames.HastyHurdles);
    }

    private void LoadSingleScene(SceneNames _sceneName)
    {
        playerHub.style.display = DisplayStyle.None;
        AsyncLevelLoader.Instance.LoadScene(_sceneName);
    }

    public void ShowResultsScreen()
    {
        ShowResults(currentPlayers.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }

    public void ShowSwaggySnapshotsResultScreen()
    {
        ShowSwaggySnapshotsResults(currentPlayersSwaggySnapshots.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }

    public void ShowMinigolfClassicResultsScreen()
    {
        ShowMinigolfClassicResults(currentPlayersRacing.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }
    
    public void ShowMinigolfRaceResultsScreen()
    {
        ShowMinigolfRaceResults(currentPlayersRacing.Players);
        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);
    }

    public void ShowRaceResultsScreen()
    {
        ShowRaceResults(currentPlayersRacing.Players);

        StartCoroutine(ShowRaceResultsWhenSorted());
    }

    private IEnumerator ShowRaceResultsWhenSorted()
    {
        yield return new WaitUntil(() => sorted);

        resultsScreen.style.display = DisplayStyle.Flex;
        FocusButton(resultScreenContinueButton);

        yield return null;
    }

    private void ShowResults(List<SO_Player> _results)
    {
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
    
    private void ShowSwaggySnapshotsResults(List<SO_PlayerSwaggySnapshots> _results)
    {
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
    
    private void ShowMinigolfClassicResults(List<SO_PlayerRacingGames> _results)
    {
        var container = resultsModal;

        container.Clear();

        var ordered = _results
            .OrderBy(_r => _r.PlayerScore.Value)
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

    private void ShowMinigolfRaceResults(List<SO_PlayerRacingGames> _results)
    {
        var container = resultsModal;

        container.Clear();

        var ordered = _results
            .OrderBy(_r => _r.TimeValue)
            .ToList();

        var currentRank = 1;
        var previousTime = float.MinValue;

        for (var i = 0; i < ordered.Count; i++)
        {
            var data = ordered[i];
            var row = rowTemplate.CloneTree();

            var currentTime = data.TimeValue;

            if (i > 0 && currentTime != previousTime)
            {
                currentRank = i + 1;
            }

            row.Q<Label>("RankLabel").text = $"{currentRank}";
            row.Q<Label>("NameLabel").text = data.Name;
            row.Q<Label>("ScoreLabel").text = data.Time;

            previousTime = currentTime;

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
        sorted = false;
        
        var container = resultsModal;

        container.Clear();

        var ordered = _results.OrderByDescending(_r => _r.PlayerScore.Value).ToList();

        ordered.Reverse();

        for (int i = 0; i < ordered.Count; i++)
        {
            var data = ordered[i];
            var row = rowTemplate.CloneTree();

            row.Q<Label>("RankLabel").text = (i + 1).ToString();
            row.Q<Label>("NameLabel").text = data.Name;

            if (!string.IsNullOrEmpty(data.Time))
                row.Q<Label>("ScoreLabel").text = data.Time;
            else
                row.Q<Label>("ScoreLabel").text = "";

            container.Add(row);
        }
        sorted = true;
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
            yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1, 0, 1), new Vector3(1, 1, 1), animDuration);
        }

        yield return new WaitForSecondsRealtime(holdDuration);

        yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1, 1, 1), new Vector3(1, 0, 1), animDuration);

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
            yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1, 0, 1), new Vector3(1, 1, 1), animDuration);
        }

        yield return new WaitForSecondsRealtime(holdDuration);

        yield return AnimateScaleCoroutine(joinInstruction, new Vector3(1, 1, 1), new Vector3(1, 0, 1), animDuration);

        joinInstruction.style.display = DisplayStyle.None;

        StartCoroutine(ShowStartAndJoinInstruction());
    }

    private IEnumerator ShowStartAndJoinInstruction()
    {
        startGameInstruction.style.display = DisplayStyle.Flex;

        yield return AnimateScaleCoroutine(startGameInstruction, new Vector3(1, 0, 1), new Vector3(1, 1, 1), animDuration);

        yield return new WaitForSecondsRealtime(holdDuration);

        yield return AnimateScaleCoroutine(startGameInstruction, new Vector3(1, 1, 1), new Vector3(1, 0, 1), animDuration);

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
    
    private void SetUpPlayerHUBNavigation()
    {
        bowlingBattleButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(jetskiJoyrideButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(playerHUBBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(hastyHurdlesButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(fishingFrenzyButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        fishingFrenzyButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(minigolfMayhemButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(playerHUBBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(bowlingBattleButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(hastyHurdlesButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        hastyHurdlesButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(swaggySnapshotsButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(playerHUBBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(fishingFrenzyButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(bowlingBattleButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        jetskiJoyrideButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(playerHUBBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(bowlingBattleButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(swaggySnapshotsButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(minigolfMayhemButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        minigolfMayhemButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(playerHUBBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(fishingFrenzyButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(jetskiJoyrideButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(swaggySnapshotsButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        swaggySnapshotsButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(playerHUBBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(hastyHurdlesButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(minigolfMayhemButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(jetskiJoyrideButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        playerHUBBackButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(fishingFrenzyButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(minigolfMayhemButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(jetskiJoyrideButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(swaggySnapshotsButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        jetskiJoyrideRaceButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(jetskiJoyrideModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(jetskiJoyrideModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(jetskiJoyrideSlalomButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(jetskiJoyrideSlalomButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        jetskiJoyrideSlalomButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(jetskiJoyrideModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(jetskiJoyrideModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(jetskiJoyrideRaceButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(jetskiJoyrideRaceButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        jetskiJoyrideModusSelectionBackButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(jetskiJoyrideSlalomButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(jetskiJoyrideRaceButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(jetskiJoyrideRaceButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(jetskiJoyrideSlalomButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        minigolfMayhemClassicButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(minigolfMayhemModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(minigolfMayhemModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(minigolfMayhemRaceButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(minigolfMayhemRaceButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        minigolfMayhemRaceButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(minigolfMayhemModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(minigolfMayhemModusSelectionBackButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(minigolfMayhemClassicButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(minigolfMayhemClassicButton);
                    _evt.StopPropagation();
                    break;
            }
        });
        
        minigolfMayhemModusSelectionBackButton.RegisterCallback<NavigationMoveEvent>(_evt =>
        {
            switch (_evt.direction)
            {
                case NavigationMoveEvent.Direction.Down:
                    FocusButton(minigolfMayhemRaceButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Up:
                    FocusButton(minigolfMayhemClassicButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Left:
                    FocusButton(minigolfMayhemClassicButton);
                    _evt.StopPropagation();
                    break;
                case NavigationMoveEvent.Direction.Right:
                    FocusButton(minigolfMayhemRaceButton);
                    _evt.StopPropagation();
                    break;
            }
        });
    }

    private void FreezeTimeScale() => Time.timeScale = 0;
    private void UnfreezeTimeScale() => Time.timeScale = 1;
}