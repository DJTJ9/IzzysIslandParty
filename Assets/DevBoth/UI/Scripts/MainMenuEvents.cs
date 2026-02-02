#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private PlayerJoiner playerJoiner;
    [SerializeField] private SceneCollectionSO sceneCollection;
    
    private UIDocument document;
    
    [Header("Menus")]
    private VisualElement menusContainer;
    private VisualElement mainMenu;
    private VisualElement settingsMenu;
    private VisualElement playerHub;
    private VisualElement controllerSelectionMenu;

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
    private Button playerHUBBackButton;

    [Header("Controller Selection Menu")] 
    private Button controllerSelectionReadyButton;
    private Button controllerSelectionBackButton;
    private VisualElement[] m_slots;
    private int m_joinedPlayers = 0;
    
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
        
        m_slots = new VisualElement[4];

        for (int i = 0; i < m_slots.Length; i++)
        {
            m_slots[i] = document.rootVisualElement.Q($"slot{i}");
        }
        
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
    }
    
    private void BindButtons()
    {
        // Main menu buttons
        startGameButton = document.rootVisualElement.Q("main-menu-play__button") as Button;
        settingsButton = document.rootVisualElement.Q("main-menu-settings__button") as Button;
        mainMenuQuitButton = document.rootVisualElement.Q("main-menu-quit__button") as Button;
        
        // Settings menu buttons
        settingsBackButton = document.rootVisualElement.Q("settings-menu-back__button") as Button;
        
        //Player HUB buttons
        bowlingBattleButton = document.rootVisualElement.Q("play-bowling-battle__button") as Button;
        fishingFrenzyButton = document.rootVisualElement.Q("play-fishing-frenzy__button") as Button;
        jetskiJoyrideButton = document.rootVisualElement.Q("play-jetski-joyride__button") as Button;
        minigolfMayhemButton = document.rootVisualElement.Q("play-minigolf-mayhem__button") as Button;
        swaggySnapshotsButton = document.rootVisualElement.Q("play-swaggy-snapshots__button") as Button;
        playerHUBBackButton = document.rootVisualElement.Q("player-hub-back__button") as Button;
        
        //Controller selection menu buttons
        controllerSelectionReadyButton = document.rootVisualElement.Q("controller-selection-ready__button") as Button;
        controllerSelectionBackButton = document.rootVisualElement.Q("controller-selection-back__button") as Button;
    }

    private void RegisterButtonCallbacks()
    {        
        // Main menu buttons
        startGameButton?.RegisterCallback<ClickEvent>(OnStartButtonClick);
        settingsButton?.RegisterCallback<ClickEvent>(OnSettingsButtonClick);
        mainMenuQuitButton?.RegisterCallback<ClickEvent>(OnQuitClick);
        
        // Settings menu buttons
        settingsBackButton?.RegisterCallback<ClickEvent>(OnSettingsBackButtonClick);
        
        // Player HUB buttons
        bowlingBattleButton?.RegisterCallback<ClickEvent>(OnLoadBowlingBattle);
        fishingFrenzyButton?.RegisterCallback<ClickEvent>(OnLoadFishingFrenzy);
        jetskiJoyrideButton?.RegisterCallback<ClickEvent>(OnLoadJetskiJoyride);
        minigolfMayhemButton?.RegisterCallback<ClickEvent>(OnLoadMinigolfMayhem);
        swaggySnapshotsButton?.RegisterCallback<ClickEvent>(OnLoadSwaggySnapshots);
        playerHUBBackButton?.RegisterCallback<ClickEvent>(OnPlayerHubBack);
        
        // Controller selection buttons
        controllerSelectionReadyButton?.RegisterCallback<ClickEvent>(OnControllerSelectionReadyButtonClick);
        controllerSelectionBackButton?.RegisterCallback<ClickEvent>(OnControllerSelectionBackButtonClick);
    }

    private void UnregisterButtonCallbacks()
    {
        // Main menu buttons
        startGameButton?.UnregisterCallback<ClickEvent>(OnStartButtonClick);
        settingsButton?.UnregisterCallback<ClickEvent>(OnSettingsButtonClick);
        mainMenuQuitButton?.UnregisterCallback<ClickEvent>(OnQuitClick);
        
        // Settings menu buttons
        settingsBackButton?.UnregisterCallback<ClickEvent>(OnSettingsBackButtonClick);

        //Player HUB buttons
        bowlingBattleButton?.UnregisterCallback<ClickEvent>(OnLoadBowlingBattle);
        fishingFrenzyButton?.UnregisterCallback<ClickEvent>(OnLoadFishingFrenzy);
        jetskiJoyrideButton?.UnregisterCallback<ClickEvent>(OnLoadJetskiJoyride);
        minigolfMayhemButton?.UnregisterCallback<ClickEvent>(OnLoadMinigolfMayhem);
        swaggySnapshotsButton?.UnregisterCallback<ClickEvent>(OnLoadSwaggySnapshots);
        playerHUBBackButton?.UnregisterCallback<ClickEvent>(OnPlayerHubBack);
        
        // Controller selection buttons
        controllerSelectionReadyButton?.UnregisterCallback<ClickEvent>(OnControllerSelectionReadyButtonClick);
        controllerSelectionBackButton?.UnregisterCallback<ClickEvent>(OnControllerSelectionBackButtonClick);
    }

    private void OnStartButtonClick(ClickEvent _evt)
    {
        mainMenu.style.display = DisplayStyle.None;
        controllerSelectionMenu.style.display = DisplayStyle.Flex;
    }
    
    private void OnControllerSelectionReadyButtonClick(ClickEvent _evt)
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
        playerJoiner.JoinNPCsBB();
    }
    
    private void OnControllerSelectionBackButtonClick(ClickEvent _evt)
    {
        controllerSelectionMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsButtonClick(ClickEvent _evt)
    {
        mainMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    private void OnSettingsBackButtonClick(ClickEvent _evt)
    {
        settingsMenu.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }
    
    private void OnPlayerHubBack(ClickEvent _evt)
    {
        playerHub.style.display = DisplayStyle.None;
        mainMenu.style.display = DisplayStyle.Flex;
    }

    private void OnQuitClick(ClickEvent _evt)
    {
      #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
      #endif
        Application.Quit();
    }
    
    private void OnLoadBowlingBattle(ClickEvent _evt)
    {
        HandleOnDisableBeforeSwitchingScene();
        LoadGameScene(SceneNames.BowlingBattleGame);
    }

    private void OnLoadFishingFrenzy(ClickEvent _evt)
    {
        LoadGameScene(SceneNames.FishingFrenzy);
    }

    private void OnLoadJetskiJoyride(ClickEvent _evt)
    {
        LoadGameScene(SceneNames.JetskiJoyride);
    }

    private void OnLoadMinigolfMayhem(ClickEvent _evt)
    {
        LoadGameSceneWithLevel(SceneNames.MinigolfMayhemGame, SceneNames.MinigolfMayhemLevel1);
    }

    private void OnLoadSwaggySnapshots(ClickEvent _evt)
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
}