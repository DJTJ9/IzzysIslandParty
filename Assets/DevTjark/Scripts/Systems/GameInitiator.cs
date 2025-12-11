using DependencyInjection;
using UnityEngine;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameInitiator : MonoBehaviour
{
    [FoldoutGroup("Scene Objects", expanded: true)]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Light mainDirectionalLight;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Volume globalVolume;
    [FoldoutGroup("Game Logic", expanded: true)]
    [SerializeField] private Injector injector;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private BallMovement ballMovement;
    [SerializeField] private UIDocument playerUI;
    [SerializeField] private GameObject bowlingPins;
    [SerializeField] private GameObject gameManager;
    

    //TODO: * Make start method async to control initialisation steps order
    private void Start()
    {
        SceneManager.LoadScene("BowlingBattleLevel", LoadSceneMode.Additive);
        BindObjects();
        InjectDependencies();
        PrepareGame();
    }

    private void BindObjects()
    {
        mainCamera = Instantiate(mainCamera);
        mainDirectionalLight = Instantiate(mainDirectionalLight);
        eventSystem = Instantiate(eventSystem);
        globalVolume = Instantiate(globalVolume);
        
        ballSpawner = Instantiate(ballSpawner);
        playerUI = Instantiate(playerUI);
        bowlingPins = Instantiate(bowlingPins);
        
        gameManager = Instantiate(gameManager);
    }

    private void InjectDependencies()
    {
        injector = Instantiate(injector);
    }

    private void CreateObjects()
    {
    }

    private void PrepareGame()
    {
        // ballMovement.GameStartConfiguration();
    }
}