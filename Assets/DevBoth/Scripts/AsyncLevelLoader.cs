using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AsyncLevelLoader : MonoBehaviour
{
    // [Header("Menu Screens"), SerializeField]
    // private GameObject mainMenu;
    //
    // [SerializeField]
    // private GameObject mainMenuImage;
    //
    // [SerializeField]
    // private GameObject loadingScreen;
    //
    // [Header("Fade Image"), SerializeField]
    // private Image loadingScreenImage;
    //
    // [SerializeField]
    // private float duration = 1f;

    // [Header("Progress Bar"), SerializeField]

    [SerializeField] private SceneCollectionSO sceneCollection;

    public static event Action<SceneNames> OnSceneChange;

    public static AsyncLevelLoader Instance;

    private const float zero = 0f;
    private const float one = 1f;

    private UIDocument uiDocument;
    private VisualElement loadingScreenContainer;
    private ProgressBar progressBar;

    private float target;

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

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        loadingScreenContainer = root.Q<VisualElement>("loading-screen__container");
        progressBar = root.Q<ProgressBar>("loading-screen__progress_bar");
    }

    private void Update()
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value, target, Time.deltaTime * 0.5f);
        progressBar.title = $"{progressBar.value * 100:0}%";
    }

    public async void LoadScene(SceneNames _sceneName)
    {
        try
        {
            loadingScreenContainer.style.display = DisplayStyle.Flex;
            progressBar.value = zero;
            target = zero;

            // SceneManager.LoadScene(sceneCollection.Scenes.TryGetValue(SceneNames.MainMenu, out var mainMenuSceneName) ? mainMenuSceneName : throw new KeyNotFoundException());

            await Task.Delay(300);

            var scene = SceneManager.LoadSceneAsync(sceneCollection.Scenes.TryGetValue(_sceneName, out var sceneNameFromCollection)
                ? sceneNameFromCollection
                : throw new KeyNotFoundException());
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

    // public void FadeOutLoadingScreen()
    // {
    //     StartCoroutine(FadeOutLoadingScreenCoroutine());
    // }
    //
    // private IEnumerator FadeOutLoadingScreenCoroutine()
    // {
    //     if (loadingScreenImage == null || duration <= 0f)
    //         yield break;
    //
    //     progressBar.gameObject.SetActive(false);
    //
    //     var color = loadingScreenImage.color;
    //     color.a = 1f; 
    //     loadingScreenImage.color = color;
    //
    //     float elapsed = 0f;
    //     while (elapsed < duration)
    //     {
    //         elapsed += Time.deltaTime;
    //         float t = Mathf.Clamp01(elapsed / duration);
    //         color.a = Mathf.Lerp(1f, 0f, t);
    //         loadingScreenImage.color = color;
    //         yield return null;
    //     }
    //
    //     color.a = 0f;
    //     loadingScreenImage.color = color;
    //     loadingScreen.SetActive(false);
    // }
    //
    // public void SetTimeScale(float timeScale)
    // {
    //     Time.timeScale = timeScale;
    // }
}