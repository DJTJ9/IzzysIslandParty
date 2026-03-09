#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] private SO_FloatVariable mouseSensitivityX;
    [SerializeField] private SO_FloatVariable mouseSensitivityY;
    [SerializeField] private SceneCollectionSO sceneCollection;
    [SerializeField] private UnityEvent onGameStart;

    private UIDocument document;
    private VisualElement root;

    [Header("Menus")]
    private VisualElement menusContainer;

    private VisualElement mainMenu;
    private VisualElement settingsMenu;
    private VisualElement playerHub;
    private VisualElement jetskiJoyrideModusSelectionMenu;
    private VisualElement minigolfMayhemModusSelectionMenu;

    [Header("Main Menu Buttons")]
    private Button startGameButton;
    private Button settingsButton;
    private Button mainMenuQuitButton;

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
    private Button minigolfMayhemButton;
    private Button swaggySnapshotsButton;
    private Button hastyHurdlesButton;
    private Button playerHUBBackButton;

    [Header("Jetski Joyride Buttons")]
    private Button jetskiJoyrideRaceButton;
    private Button jetskiJoyrideSlalomButton;
    private Button jetskiJoyrideModusSelectionBackButton;
    
    [Header("Minigolf Mayhem Buttons")]
    private Button minigolfMayhemClassicButton;
    private Button minigolfMayhemRaceButton;
    private Button minigolfMayhemModusSelectionBackButton;

    #region Example Variables

    // private Button button;
    // private List<Button> menuButtons = new List<Button>();

    #endregion

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;

        #region Examples

        // button = document.rootVisualElement.Q("StartGameButton") as Button;
        // button?.RegisterCallback<ClickEvent>(OnStartButtonClick);
        //
        // menuButtons = document.rootVisualElement.Query<Button>().ToList();
        // foreach (var menuButton in menuButtons)
        // {
        //     menuButton.RegisterCallback<ClickEvent>(OnAllButtonsClicked);
        // }

        #endregion
    }

    private void OnEnable()
    {
        BindVisualElements();
        BindButtons();
        RegisterButtonCallbacks();
        SetUpPlayerHUBNavigation();
        AsyncLevelLoader.OnSceneChange += OpenMainMenu;
                
        mouseSensitivityX.Value = mouseSensitivitySlider.value;
        mouseSensitivityY.Value = mouseSensitivitySlider.value;

        FocusButton(startGameButton);

        onGameStart.Invoke();
    }

    private void OnDisable()
    {
        UnregisterButtonCallbacks();
        AsyncLevelLoader.OnSceneChange -= OpenMainMenu;

        #region Examples

        // button.UnregisterCallback<ClickEvent>(OnStartButtonClick);
        //
        // foreach (var menuButton in menuButtons)
        // {
        //     menuButton.UnregisterCallback<ClickEvent>(OnAllButtonsClicked);
        // }

        #endregion
    }


    #region Example Methods

    // private void OnStartButtonClick(ClickEvent _evt)
    // {
    //     Debug.Log("Play Game Button Clicked");
    // }
    //
    // private void OnAllButtonsClicked(ClickEvent _evt)
    // {
    //     Debug.Log("One Of The Menu Buttons Clicked");
    // }

    #endregion

    private void BindVisualElements()
    {
        menusContainer = root.Q("menu-background__container");
        mainMenu = root.Q("main-menu__container");
        settingsMenu = root.Q("settings-menu__container");
        playerHub = root.Q("player-hub__container");
        jetskiJoyrideModusSelectionMenu = root.Q("jetski-joyride-modus-selection__container");
        minigolfMayhemModusSelectionMenu = root.Q("minigolf-mayhem-modus-selection__container");
    }

    private void BindButtons()
    {
        // Main menu buttons
        startGameButton = root.Q("main-menu-play__button") as Button;
        settingsButton = root.Q("main-menu-settings__button") as Button;
        mainMenuQuitButton = root.Q("main-menu-quit__button") as Button;

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
        minigolfMayhemButton = root.Q("play-minigolf-mayhem__button") as Button;
        swaggySnapshotsButton = root.Q("play-swaggy-snapshots__button") as Button;
        hastyHurdlesButton = root.Q("play-hasty-hurdles__button") as Button;
        playerHUBBackButton = root.Q("player-hub-back__button") as Button;

        // Jetski Joyride Modus Selection buttons
        jetskiJoyrideRaceButton = root.Q("play-jetski-joyride-race__button") as Button;
        jetskiJoyrideSlalomButton = root.Q("play-jetski-joyride-slalom__button") as Button;
        jetskiJoyrideModusSelectionBackButton = root.Q("jetski-joyride-modus-selection-back__button") as Button;
        
        // Minigolf Mayhem Modus Selection buttons
        minigolfMayhemClassicButton = root.Q("play-minigolf-mayhem-classic__button") as Button;
        minigolfMayhemRaceButton = root.Q("play-minigolf-mayhem-race__button") as Button;
        minigolfMayhemModusSelectionBackButton = root.Q("minigolf-mayhem-modus-selection-back__button") as Button;
    }

    private void RegisterButtonCallbacks()
    {
        // Main menu buttons
        startGameButton.clicked += OnStartButtonClick;
        settingsButton.clicked += OnSettingsButtonClick;
        mainMenuQuitButton.clicked += OnQuitClick;

        // Settings menu buttons
        mouseSensitivitySlider.RegisterValueChangedCallback(OnMouseSensitivityChange);
        masterVolumeSlider.RegisterValueChangedCallback(OnMasterVolumeChange);
        musicVolumeSlider.RegisterValueChangedCallback(OnMusicVolumeChange);
        soundFXVolumeSlider.RegisterValueChangedCallback(OnSoundFXVolumeChange);
        settingsBackButton.clicked += OnSettingsBackButtonClick;

        // Player HUB buttons
        bowlingBattleButton.clicked += OnLoadBowlingBattle;
        fishingFrenzyButton.clicked += OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked += OnLoadJetskiJoyride;
        minigolfMayhemButton.clicked += OnLoadMinigolfMayhem;
        swaggySnapshotsButton.clicked += OnLoadSwaggySnapshots;
        hastyHurdlesButton.clicked += OnLoadHastyHurdles;
        playerHUBBackButton.clicked += OnPlayerHubBack;

        // Jetski Joyride Modus Selection
        jetskiJoyrideRaceButton.clicked += OnJetskiJoyrideRaceButtonClick;
        jetskiJoyrideSlalomButton.clicked += OnJetskiJoyrideSlalomButtonClick;
        jetskiJoyrideModusSelectionBackButton.clicked += OnJetskiJoyrideModusSelectionBackButtonClick;
        
        // Minigolf Mayhem Modus Selection
        minigolfMayhemClassicButton.clicked += OnMinigolfMayhemClassicButtonClick;
        minigolfMayhemRaceButton.clicked += OnMinigolfMayhemRaceButtonClick;
        minigolfMayhemModusSelectionBackButton.clicked += OnMinigolfMayhemModusSelectionBackButtonClick;
    }

    private void UnregisterButtonCallbacks()
    {
        // Main menu buttons
        startGameButton.clicked -= OnStartButtonClick;
        settingsButton.clicked -= OnSettingsButtonClick;
        mainMenuQuitButton.clicked -= OnQuitClick;

        // Settings menu buttons
        mouseSensitivitySlider.UnregisterValueChangedCallback(OnMouseSensitivityChange);
        masterVolumeSlider.UnregisterValueChangedCallback(OnMasterVolumeChange);
        musicVolumeSlider.UnregisterValueChangedCallback(OnMusicVolumeChange);
        soundFXVolumeSlider.UnregisterValueChangedCallback(OnSoundFXVolumeChange);
        settingsBackButton.clicked -= OnSettingsBackButtonClick;

        //Player HUB buttons
        bowlingBattleButton.clicked -= OnLoadBowlingBattle;
        fishingFrenzyButton.clicked -= OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked -= OnLoadJetskiJoyride;
        minigolfMayhemButton.clicked -= OnLoadMinigolfMayhem;
        swaggySnapshotsButton.clicked -= OnLoadSwaggySnapshots;
        hastyHurdlesButton.clicked -= OnLoadHastyHurdles;
        playerHUBBackButton.clicked -= OnPlayerHubBack;

        // Jetski Joyride Modus Selection
        jetskiJoyrideRaceButton.clicked -= OnJetskiJoyrideRaceButtonClick;
        jetskiJoyrideSlalomButton.clicked -= OnJetskiJoyrideSlalomButtonClick;
        jetskiJoyrideModusSelectionBackButton.clicked -= OnJetskiJoyrideModusSelectionBackButtonClick;
        
        // Minigolf Mayhem Modus Selection
        minigolfMayhemClassicButton.clicked -= OnMinigolfMayhemClassicButtonClick;
        minigolfMayhemRaceButton.clicked -= OnMinigolfMayhemRaceButtonClick;
        minigolfMayhemModusSelectionBackButton.clicked -= OnMinigolfMayhemModusSelectionBackButtonClick;
    }

    private void OnStartButtonClick()
    {
        mainMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        FocusButton(bowlingBattleButton);
    }

    private void OnSettingsButtonClick()
    {
        mainMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
        FocusButton(mouseSensitivitySlider);
    }

    private void OnSettingsBackButtonClick()
    {
        settingsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
        FocusButton(settingsButton);
    }

    private void OnPlayerHubBack()
    {
        playerHub.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
        FocusButton(startGameButton);
    }

    private void OnQuitClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnMouseSensitivityChange(ChangeEvent<float> _evt)
    {
        mouseSensitivityX.Value = _evt.newValue;
        mouseSensitivityY.Value = _evt.newValue;
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

    private void OnLoadBowlingBattle()
    {
        LoadGameScene(SceneNames.BowlingBattle);
    }

    private void OnLoadFishingFrenzy()
    {
        LoadGameScene(SceneNames.FishingFrenzy);
    }
    
    private void OnLoadHastyHurdles()
    {
        LoadGameScene(SceneNames.HastyHurdles);
    }

    private void OnLoadJetskiJoyride()
    {
        jetskiJoyrideModusSelectionMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
        FocusButton(jetskiJoyrideRaceButton);
    }

    private void OnJetskiJoyrideSlalomButtonClick()
    {
        LoadGameScene(SceneNames.JetskiJoyrideSlalom);
    }

    private void OnJetskiJoyrideRaceButtonClick()
    {
        LoadGameScene(SceneNames.JetskiJoyrideRace);
    }

    private void OnJetskiJoyrideModusSelectionBackButtonClick()
    {
        jetskiJoyrideModusSelectionMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        FocusButton(jetskiJoyrideButton);  
    }

    private void OnLoadMinigolfMayhem()
    {
        minigolfMayhemModusSelectionMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
        FocusButton(minigolfMayhemClassicButton);
    }
    
    private void OnMinigolfMayhemClassicButtonClick()
    {
        LoadGameScene(SceneNames.MinigolfMayhemClassic);
    }
    
    private void OnMinigolfMayhemRaceButtonClick()
    {
        LoadGameScene(SceneNames.MinigolfMayhemRace);
    }
    
    private void OnMinigolfMayhemModusSelectionBackButtonClick()
    {
        minigolfMayhemModusSelectionMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        FocusButton(minigolfMayhemButton);
    }

    private void OnLoadSwaggySnapshots()
    {
        LoadGameScene(SceneNames.SwaggySnapshots);
    }

    private void LoadGameScene(SceneNames _sceneName)
    {
        menusContainer.style.display = DisplayStyle.None;
        AsyncLevelLoader.Instance.LoadScene(_sceneName);
    }

    private void OpenMainMenu(SceneNames _sceneName)
    {
        if (_sceneName != SceneNames.MainMenu) return;
        
        menusContainer.style.display = DisplayStyle.Flex;
    }

    private void FocusButton(VisualElement _button) => StartCoroutine(FocusButtonCoroutine(_button));

    private IEnumerator FocusButtonCoroutine(VisualElement _button)
    {
        yield return null;
        yield return null;
        _button.Focus();
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
}