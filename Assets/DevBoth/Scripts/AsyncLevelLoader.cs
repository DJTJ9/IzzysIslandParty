using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-1000)]
public class AsyncLevelLoader : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: false)] 
    [SerializeField] private float progressBarSpeed = 0.5f;
    [SerializeField] private int firstLoadingScreenDelay = 300;
    [SerializeField] private int secondLoadingScreenDelay = 500;
    [SerializeField] private int thirdLoadingScreenDelay = 2000;
    [SerializeField] private int endLoadingScreenDelay = 1000;
    
    [SerializeField] private SceneCollectionSO sceneCollection;

    public static event Action<SceneNames> OnSceneChange;

    public static AsyncLevelLoader Instance;

    private const float k_Zero = 0f;
    private const float k_One = 1f;

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

    private void FixedUpdate()
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
            progressBar.value = k_Zero;
            target = k_Zero;
            loadingScreenContainer.style.display = DisplayStyle.Flex;

            await Task.Delay(firstLoadingScreenDelay);

            var scene = SceneManager.LoadSceneAsync(sceneCollection.Scenes.TryGetValue(_sceneName, out var sceneNameFromCollection)
                ? sceneNameFromCollection : throw new KeyNotFoundException());
            
            if (scene == null) return;

            scene.allowSceneActivation = false;

            do
            {
                await Task.Delay(secondLoadingScreenDelay);
                target = Mathf.Clamp01(scene.progress / 0.9f);
            } while (scene.progress < 0.9f);

            target = k_One;
            OnSceneChange?.Invoke(_sceneName);
            await Task.Delay(thirdLoadingScreenDelay);

            scene.allowSceneActivation = true;

            await Task.Delay(endLoadingScreenDelay);
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
            progressBar.value = k_Zero;
            target = k_Zero;

            await Task.Delay(firstLoadingScreenDelay);

            var scene = SceneManager.LoadSceneAsync(currentSceneName);
            if (scene == null) return;

            scene.allowSceneActivation = false;

            do
            {
                await Task.Delay(secondLoadingScreenDelay);
                target = Mathf.Clamp01(scene.progress / 0.9f);
            } while (scene.progress < 0.9f);

            target = k_One;
            await Task.Delay(thirdLoadingScreenDelay);

            scene.allowSceneActivation = true;

            await Task.Delay(endLoadingScreenDelay);
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
        progressBar.value = Mathf.MoveTowards(progressBar.value, target, Time.unscaledDeltaTime * progressBarSpeed);
        progressBar.title = $"{progressBar.value * 100:0}%";
    }
}