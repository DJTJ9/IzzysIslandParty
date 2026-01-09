#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuEvents : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;
    
    private UIDocument document;
    
    [Header("Menus")]
    private VisualElement mainMenu;
    private VisualElement settingsMenu;
    private VisualElement playerHub;

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
        
        #region Examples
        // button = document.rootVisualElement.Q("StartGameButton") as Button;
        // button?.RegisterCallback<ClickEvent>(OnPlayGameClick);
        //
        // menuButtons = document.rootVisualElement.Query<Button>().ToList();
        // foreach (var menuButton in menuButtons)
        // {
        //     menuButton.RegisterCallback<ClickEvent>(OnAllButtonsClicked);
        // }
        #endregion
    }

    private void OnDisable()
    {
        UnregisterButtonCallbacks();
        
        #region Examples
        // button.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        //
        // foreach (var menuButton in menuButtons)
        // {
        //     menuButton.UnregisterCallback<ClickEvent>(OnAllButtonsClicked);
        // }
        #endregion
    }

    #region Example Methods
    // private void OnPlayGameClick(ClickEvent _evt)
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
        mainMenu = document.rootVisualElement.Q("main-menu__container");
        settingsMenu = document.rootVisualElement.Q("settings-menu__container");
        playerHub = document.rootVisualElement.Q("player-hub__container");
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
    }

    private void RegisterButtonCallbacks()
    {        
        // Main menu buttons
        startGameButton?.RegisterCallback<ClickEvent>(OnPlayGameClick);
        settingsButton?.RegisterCallback<ClickEvent>(OnSettingsButtonClick);
        mainMenuQuitButton?.RegisterCallback<ClickEvent>(OnQuitClick);
        
        // Settings menu buttons
        settingsBackButton?.RegisterCallback<ClickEvent>(OnSettingsBackButtonClick);
        
        //Player HUB buttons
        bowlingBattleButton?.RegisterCallback<ClickEvent>(OnLoadBowlingBattle);
        fishingFrenzyButton?.RegisterCallback<ClickEvent>(OnLoadFishingFrenzy);
        jetskiJoyrideButton?.RegisterCallback<ClickEvent>(OnLoadJetskiJoyride);
        minigolfMayhemButton?.RegisterCallback<ClickEvent>(OnLoadMinigolfMayhem);
        swaggySnapshotsButton?.RegisterCallback<ClickEvent>(OnLoadSwaggySnapshots);
        playerHUBBackButton?.RegisterCallback<ClickEvent>(OnPlayerHubBack);
    }

    private void UnregisterButtonCallbacks()
    {
        // Main menu buttons
        startGameButton?.UnregisterCallback<ClickEvent>(OnPlayGameClick);
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
    }

    private void OnPlayGameClick(ClickEvent _evt)
    {
        mainMenu.style.display = DisplayStyle.None;
        playerHub.style.display = DisplayStyle.Flex;
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
        LoadGameSceneWithLevel(SceneNames.BowlingBattleGame, SceneNames.BowlingBattleLevel);
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

    private void LoadGameScene(SceneNames sceneName)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(sceneName, out var sceneNameFromCollection) ? sceneNameFromCollection : throw new KeyNotFoundException());
    }

    private void LoadGameSceneWithLevel(SceneNames gameScene, SceneNames levelScene)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(gameScene, out var gameSceneName) ? gameSceneName : throw new KeyNotFoundException());
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(levelScene, out var levelSceneName) ? levelSceneName : throw new KeyNotFoundException(), LoadSceneMode.Additive);
    }
}