using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class PauseMenuEvents : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;
    
    [SerializeField] private UnityEvent OnUnpause;
    
    private UIDocument document;
    
    [Header("Menus")]
    private VisualElement pauseMenu;
    private VisualElement playerHub;
    
    [Header("Pause Menu Buttons")]
    private Button resumeButton;
    private Button restartButton;
    private Button changeLevelButton;
    private Button pauseMenuQuitButton;
    
    [Header("Player HUB Buttons")]
    private Button bowlingBattleButton;
    private Button fishingFrenzyButton;
    private Button jetskiJoyrideButton;
    private Button minigolfMayhemButton;
    private Button swaggySnapshotsButton;
    private Button playerHUBBackButton;
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        
        BindVisualElements();
        BindButtons();
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
    }
    
    private void BindButtons()
    {
        // Pause menu buttons
        resumeButton = document.rootVisualElement.Q("pause-menu-resume__button") as Button;
        restartButton = document.rootVisualElement.Q("pause-menu-restart__button") as Button;
        changeLevelButton = document.rootVisualElement.Q("pause-menu-change-level__button") as Button;
        pauseMenuQuitButton = document.rootVisualElement.Q("pause-menu-quit__button") as Button;
        
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
        // Pause menu buttons
        resumeButton?.RegisterCallback<ClickEvent>(OnResumeGameClick);
        restartButton?.RegisterCallback<ClickEvent>(OnRestartGameClick);
        changeLevelButton?.RegisterCallback<ClickEvent>(OnChangeLevelClick);
        pauseMenuQuitButton?.RegisterCallback<ClickEvent>(OnQuitClick);
        
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
        // Pause menu buttons
        resumeButton?.UnregisterCallback<ClickEvent>(OnResumeGameClick);
        restartButton?.UnregisterCallback<ClickEvent>(OnRestartGameClick);
        changeLevelButton?.UnregisterCallback<ClickEvent>(OnChangeLevelClick);
        pauseMenuQuitButton?.UnregisterCallback<ClickEvent>(OnQuitClick);

        //Player HUB buttons
        bowlingBattleButton?.UnregisterCallback<ClickEvent>(OnLoadBowlingBattle);
        fishingFrenzyButton?.UnregisterCallback<ClickEvent>(OnLoadFishingFrenzy);
        jetskiJoyrideButton?.UnregisterCallback<ClickEvent>(OnLoadJetskiJoyride);
        minigolfMayhemButton?.UnregisterCallback<ClickEvent>(OnLoadMinigolfMayhem);
        swaggySnapshotsButton?.UnregisterCallback<ClickEvent>(OnLoadSwaggySnapshots);
        playerHUBBackButton?.UnregisterCallback<ClickEvent>(OnPlayerHubBack);
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
    
    private void OnRestartGameClick(ClickEvent _evt)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    private void OnResumeGameClick(ClickEvent _evt)
    {
        OnUnpause.Invoke();
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
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.MainMenu, out var sceneName) ? sceneName : throw new KeyNotFoundException());
    }
    
    private void OnLoadBowlingBattle(ClickEvent _evt)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.BowlingBattle, out var sceneName) ? sceneName : throw new KeyNotFoundException());
    }

    private void OnLoadFishingFrenzy(ClickEvent _evt)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.FishingFrenzy, out var sceneName) ? sceneName : throw new KeyNotFoundException());
    }

    private void OnLoadJetskiJoyride(ClickEvent _evt)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.JetskiJoyride, out var sceneName) ? sceneName : throw new KeyNotFoundException());
    }

    private void OnLoadMinigolfMayhem(ClickEvent _evt)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.MinigolfMayhem, out var sceneName) ? sceneName : throw new KeyNotFoundException());
    }

    private void OnLoadSwaggySnapshots(ClickEvent _evt)
    {
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.SwaggySnapshots, out var sceneName) ? sceneName : throw new KeyNotFoundException());
    }
}
