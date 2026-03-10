using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-1000)]
public class AsyncLevelLoader : MonoBehaviour
{
    [SerializeField] private SceneCollectionSO sceneCollection;

    public static event Action<SceneNames> OnSceneChange;

    public static AsyncLevelLoader Instance;

    private const float zero = 0f;
    private const float one = 1f;

    private UIDocument uiDocument;
    private VisualElement loadingScreenContainer;
    private ProgressBar progressBar;

    private float target;

    /// <summary>
    /// Initializes the singleton instance of `AsyncLevelLoader`, ensuring it persists across scenes.
    /// If another instance already exists, it destroys the duplicate.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Prepares the loading screen elements from the `UIDocument`,
    /// including the progress bar and container for managing the loading UI.
    /// </summary>
    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        loadingScreenContainer = root.Q<VisualElement>("loading-screen__container");
        progressBar = root.Q<ProgressBar>("loading-screen__progress_bar");
    }

    private void Update()
    {
        UpdateLoadingScreenBar();
    }

    /// <summary>
    /// Loads a scene asynchronously by its name with a loading screen. 
    /// Waits until the scene is ready before displaying it, ensuring fluid transitions.
    /// </summary>
    /// <param name="_sceneName">The scene to load.</param>
    public async void LoadScene(SceneNames _sceneName)
    {
        try
        {
            loadingScreenContainer.style.display = DisplayStyle.Flex;
            progressBar.value = zero;
            target = zero;

            await Task.Delay(300);

            var scene = SceneManager.LoadSceneAsync(sceneCollection.Scenes.TryGetValue(_sceneName, out var sceneNameFromCollection)
                ? sceneNameFromCollection : throw new KeyNotFoundException());
            
            if (scene == null) return;

            scene.allowSceneActivation = false;

            do
            {
                await Task.Delay(500);
                target = Mathf.Clamp01(scene.progress / 0.9f);
            } while (scene.progress < 0.9f);

            target = one;
            OnSceneChange?.Invoke(_sceneName);
            await Task.Delay(2000);

            // FadeOutLoadingScreen();
            scene.allowSceneActivation = true;

            await Task.Delay(1000);
            loadingScreenContainer.style.display = DisplayStyle.None;
        }
        catch (Exception e)
        {
            throw new Exception($"{e}");
        }
    }

    /// <summary>
    /// Restarts the current level asynchronously with a loading screen, ensuring a seamless reload process.
    /// Returns to the main menu in case of an issue while handling the scene reinitialization.
    /// </summary>
    public async void RestartLevel()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.MainMenu, out var mainMenuSceneName) ? mainMenuSceneName : throw new KeyNotFoundException());
        
        try
        {
            loadingScreenContainer.style.display = DisplayStyle.Flex;
            progressBar.value = zero;
            target = zero;

            await Task.Delay(300);

            var scene = SceneManager.LoadSceneAsync(currentSceneName);
            if (scene == null) return;

            scene.allowSceneActivation = false;

            do
            {
                await Task.Delay(500);
                target = Mathf.Clamp01(scene.progress / 0.9f);
            } while (scene.progress < 0.9f);

            target = one;
            await Task.Delay(2000);

            scene.allowSceneActivation = true;

            await Task.Delay(1000);
            loadingScreenContainer.style.display = DisplayStyle.None;
        }
        catch (Exception e)
        {
            throw new Exception($"{e}");
        }
    }

    /// <summary>
    /// Gradually updates the loading progress bar's value and visual title
    /// to reflect the current stage of the scene loading process.
    /// </summary>
    private void UpdateLoadingScreenBar()
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value, target, Time.unscaledDeltaTime * 0.5f);
        progressBar.title = $"{progressBar.value * 100:0}%";
    }
}