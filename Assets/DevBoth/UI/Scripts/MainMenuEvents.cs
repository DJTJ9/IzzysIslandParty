#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenuEvents : MonoBehaviour
{
    // [SerializeField] private PlayerInputManager playerInputManager;
    // [SerializeField] private JetskiJoyridePlayerJoiner playerJoiner;
    [SerializeField] private SceneCollectionSO sceneCollection;
    [SerializeField] private UnityEvent onLoadBowlingBattle;
    
    private UIDocument document;
    
    [Header("Menus")]
    private VisualElement menusContainer;
    private VisualElement mainMenu;
    private VisualElement settingsMenu;
    private VisualElement playerHub;
    private VisualElement controllerSelectionMenu;
    private VisualElement jetskiJoyrideModusSelectionMenu;

    [Header("Main Menu Buttons")]
    private Button startGameButton;
    private Button settingsButton;
    private Button mainMenuQuitButton;
    
    [Header("Settings Menu Buttons")]
    private Button settingsBackButton;
    
    [Header("Player HUB Buttons")]
    private Button bowlingBattleButton;
    private Button fishingFrenzyButton;
    private Button jetskiJoyrideButton;
    private Button minigolfMayhemButton;
    private Button swaggySnapshotsButton;
    private Button hastyHurdlesButton;
    private Button playerHUBBackButton;

    [Header("Controller Selection Menu")] 
    private Button controllerSelectionReadyButton;
    private Button controllerSelectionBackButton;
    private VisualElement[] m_slots;
    private int m_joinedPlayers = 0;

    [Header("Jetski Joyride Buttons")] 
    private Button jetskiJoyrideRaceButton;
    private Button jetskiJoyrideSlalomButton;
    private Button jetskiJoyrideModusSelectionBackButton;
    
    #region Example Variables
    // private Button button;
    // private List<Button> menuButtons = new List<Button>();
    #endregion
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        
        BindVisualElements();
        BindButtons();
        RegisterButtonCallbacks();
        
        InitializeResultScreenSlots();
        FocusButton(startGameButton);

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

    private void InitializeResultScreenSlots()
    {
        m_slots = new VisualElement[4];

        for (int i = 0; i < m_slots.Length; i++)
        {
            m_slots[i] = document.rootVisualElement.Q($"slot{i}");
        }
    }

    private void OnEnable()
    {
        // playerInputManager.onPlayerJoined += OnPlayerJoined;
    }

       private void OnDisable()
       {
           // HandleOnDisableBeforeSwitchingScene();

           #region Examples
        // button.UnregisterCallback<ClickEvent>(OnStartButtonClick);
        //
        // foreach (var menuButton in menuButtons)
        // {
        //     menuButton.UnregisterCallback<ClickEvent>(OnAllButtonsClicked);
        // }
        #endregion
       }

    private void HandleOnDisableBeforeSwitchingScene()
    {
        UnregisterButtonCallbacks();
        // playerInputManager.onPlayerJoined -= OnPlayerJoined;
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
        menusContainer = document.rootVisualElement.Q("menu-background__container");
        mainMenu = document.rootVisualElement.Q("main-menu__container");
        settingsMenu = document.rootVisualElement.Q("settings-menu__container");
        playerHub = document.rootVisualElement.Q("player-hub__container");
        controllerSelectionMenu = document.rootVisualElement.Q("controller-selection-menu__container");
        jetskiJoyrideModusSelectionMenu = document.rootVisualElement.Q("jetski-joyride-modus-selection__container");
    }
    
    private void BindButtons()
    {
        // Main menu buttons
        startGameButton = document.rootVisualElement.Q("main-menu-play__button") as Button;
        settingsButton = document.rootVisualElement.Q("main-menu-settings__button") as Button;
        mainMenuQuitButton = document.rootVisualElement.Q("main-menu-quit__button") as Button;
        
        // Settings menu buttons
        settingsBackButton = document.rootVisualElement.Q("settings-menu-back__button") as Button;
        
        // Player HUB buttons
        bowlingBattleButton = document.rootVisualElement.Q("play-bowling-battle__button") as Button;
        fishingFrenzyButton = document.rootVisualElement.Q("play-fishing-frenzy__button") as Button;
        jetskiJoyrideButton = document.rootVisualElement.Q("play-jetski-joyride__button") as Button;
        minigolfMayhemButton = document.rootVisualElement.Q("play-minigolf-mayhem__button") as Button;
        swaggySnapshotsButton = document.rootVisualElement.Q("play-swaggy-snapshots__button") as Button;
        hastyHurdlesButton = document.rootVisualElement.Q("play-hasty-hurdles__button") as Button;
        playerHUBBackButton = document.rootVisualElement.Q("player-hub-back__button") as Button;
        
        // Controller selection menu buttons
        controllerSelectionReadyButton = document.rootVisualElement.Q("controller-selection-ready__button") as Button;
        controllerSelectionBackButton = document.rootVisualElement.Q("controller-selection-back__button") as Button;
        
        // Jetski Joyride Modus Selection
        jetskiJoyrideRaceButton = document.rootVisualElement.Q("play-jetski-joyride-race__button") as Button;
        jetskiJoyrideSlalomButton = document.rootVisualElement.Q("play-jetski-joyride-slalom__button") as Button;
        jetskiJoyrideModusSelectionBackButton = document.rootVisualElement.Q("jetski-joyride-modus-selection-back__button") as Button;
    }

    private void RegisterButtonCallbacks()
    {        
        // Main menu buttons
        startGameButton.clicked += OnStartButtonClick;
        settingsButton.clicked += OnSettingsButtonClick;
        mainMenuQuitButton.clicked += OnQuitClick;
        
        // Settings menu buttons
        settingsBackButton.clicked += OnSettingsBackButtonClick;
        
        // Player HUB buttons
        bowlingBattleButton.clicked += OnLoadBowlingBattle;
        fishingFrenzyButton.clicked += OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked += OnLoadJetskiJoyride;
        minigolfMayhemButton.clicked += OnLoadMinigolfMayhem;
        swaggySnapshotsButton.clicked += OnLoadSwaggySnapshots;
        hastyHurdlesButton.clicked += OnLoadHastyHurdles;
        playerHUBBackButton.clicked += OnPlayerHubBack;
        
        // Controller selection buttons
        controllerSelectionReadyButton.clicked += OnControllerSelectionReadyButtonClick;
        controllerSelectionBackButton.clicked += OnControllerSelectionBackButtonClick;

        // Jetski Joyride Modus Selection
        jetskiJoyrideRaceButton.clicked += OnJetskiJoyrideRaceButtonClick;
        jetskiJoyrideSlalomButton.clicked += OnJetskiJoyrideSlalomButtonClick;
        jetskiJoyrideModusSelectionBackButton.clicked += OnJetskiJoyrideModusSelectionBackButtonClick;
    }

    private void OnLoadHastyHurdles()
    {
        LoadGameScene(SceneNames.HastyHurdles);
    }

    private void OnJetskiJoyrideModusSelectionBackButtonClick()
    {
        jetskiJoyrideModusSelectionMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
    }

    private void OnJetskiJoyrideSlalomButtonClick()
    {
        LoadGameScene(SceneNames.JetskiJoyrideSlalom);
    }

    private void OnJetskiJoyrideRaceButtonClick()
    {
        LoadGameScene(SceneNames.JetskiJoyrideRace);
    }

    private void UnregisterButtonCallbacks()
    {
        // Main menu buttons
        startGameButton.clicked -= OnStartButtonClick;
        settingsButton.clicked -= OnSettingsButtonClick;
        mainMenuQuitButton.clicked -= OnQuitClick;
        
        // Settings menu buttons
        settingsBackButton.clicked -= OnSettingsBackButtonClick;

        //Player HUB buttons
        bowlingBattleButton.clicked -= OnLoadBowlingBattle;
        fishingFrenzyButton.clicked -= OnLoadFishingFrenzy;
        jetskiJoyrideButton.clicked -= OnLoadJetskiJoyride;
        minigolfMayhemButton.clicked -= OnLoadMinigolfMayhem;
        swaggySnapshotsButton.clicked -= OnLoadSwaggySnapshots;
        hastyHurdlesButton.clicked -= OnLoadHastyHurdles;
        playerHUBBackButton.clicked -= OnPlayerHubBack;
        
        // Controller selection buttons
        controllerSelectionReadyButton.clicked -= OnControllerSelectionReadyButtonClick;
        controllerSelectionBackButton.clicked -= OnControllerSelectionBackButtonClick;
        
        // Jetski Joyride Modus Selection
        jetskiJoyrideRaceButton.clicked -= OnJetskiJoyrideRaceButtonClick;
        jetskiJoyrideSlalomButton.clicked -= OnJetskiJoyrideSlalomButtonClick;
        jetskiJoyrideModusSelectionBackButton.clicked -= OnJetskiJoyrideModusSelectionBackButtonClick;
    }

    private void OnStartButtonClick()
    {
        mainMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
    }
    
    private void OnControllerSelectionReadyButtonClick()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        // playerJoiner.JoinNPCs();
    }
    
    private void OnControllerSelectionBackButtonClick()
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsButtonClick()
    {
        mainMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsBackButtonClick()
    {
        settingsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }
    
    private void OnPlayerHubBack()
    {
        playerHub.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnQuitClick()
    {
      #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
      #endif
        Application.Quit();
    }
    
    private void OnLoadBowlingBattle()
    {
        onLoadBowlingBattle.Invoke();
        HandleOnDisableBeforeSwitchingScene();
        LoadGameScene(SceneNames.BowlingBattleGame);
    }

    private void OnLoadFishingFrenzy()
    {
        LoadGameScene(SceneNames.FishingFrenzy);
    }

    private void OnLoadJetskiJoyride()
    {
        jetskiJoyrideModusSelectionMenu.style.display = DisplayStyle.Flex;
        playerHub.style.display = DisplayStyle.None;
    }

    private void OnLoadMinigolfMayhem()
    {
        LoadGameSceneWithLevel(SceneNames.MinigolfMayhemGame, SceneNames.MinigolfMayhemLevel1);
    }

    private void OnLoadSwaggySnapshots()
    {
        LoadGameSceneWithLevel(SceneNames.SwaggySnapshotsGame, SceneNames.SwaggySnapshotsLevel);
    }

    private void LoadGameScene(SceneNames _sceneName)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(_sceneName, out var sceneNameFromCollection) ? sceneNameFromCollection : throw new KeyNotFoundException());
    }

    private void LoadGameSceneWithLevel(SceneNames _gameScene, SceneNames _levelScene)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(_gameScene, out var gameSceneName) ? gameSceneName : throw new KeyNotFoundException());
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(_levelScene, out var levelSceneName) ? levelSceneName : throw new KeyNotFoundException(), LoadSceneMode.Additive);
    }

    private void LoadSceneAdditive(SceneNames _sceneName)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(_sceneName, out var sceneNameFromCollection) ? sceneNameFromCollection : throw new KeyNotFoundException(), LoadSceneMode.Additive);
    }
    
    public void OnPlayerJoined(PlayerInput _obj)
    {
        if (m_joinedPlayers >= m_slots.Length)
            return;

        var slot = m_slots[m_joinedPlayers];

        slot.RemoveFromClassList("waiting");
        slot.AddToClassList("ready");

        m_joinedPlayers++;
    }
    
    public void OnPlayerLeft(PlayerInput _obj)
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