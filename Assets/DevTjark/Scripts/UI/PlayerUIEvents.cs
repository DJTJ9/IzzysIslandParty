using DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIEvents : MonoBehaviour
{
    [SerializeField] private BowlingBallCollectionSO ballCollectionSO;
    
    private UIDocument document;
    
    private VisualElement playerUI;
    private VisualElement leftPlayerUI;
    private VisualElement middlePlayerUI;
    private VisualElement rightPlayerUI;
    
    private Button basketBallButton;
    private Button baseBallButton;
    private Button footBallButton;
    
    [Inject] private BallSpawner ballSpawner;

    private void Awake()
    {
            document = GetComponent<UIDocument>();
            
            BindElements();
    }

    private void BindElements()
    {
        BindVisualElements();
        BindButtonsWithEvents();
    }

    private void BindVisualElements()
    {
        playerUI = document.rootVisualElement.Q("player-ui__container");
        leftPlayerUI = document.rootVisualElement.Q("player-ui-left__container");
        middlePlayerUI = document.rootVisualElement.Q("player-ui-middle__container");
        rightPlayerUI = document.rootVisualElement.Q("player-ui-right__container");
    }

    private void BindButtonsWithEvents()
    {
        baseBallButton = document.rootVisualElement.Q("ball-selector-baseball__button") as Button;
        baseBallButton?.RegisterCallback<ClickEvent>
            (_evt => SpawnBall(ballCollectionSO.BowlingBalls[BallType.Baseball]));
        
        basketBallButton = document.rootVisualElement.Q("ball-selector-basketball__button") as Button;
        basketBallButton?.RegisterCallback<ClickEvent>
            (_evt => SpawnBall(ballCollectionSO.BowlingBalls[BallType.Basketball]));
        
        footBallButton = document.rootVisualElement.Q("ball-selector-football__button") as Button;
        footBallButton?.RegisterCallback<ClickEvent>
            (_evt => SpawnBall(ballCollectionSO.BowlingBalls[BallType.Football]));
    }

    public void ShowPlayerUI()
    {
        playerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HidePlayerUI()
    {
        playerUI.style.display = DisplayStyle.None;
    }
    
    public void ShowLeftPlayerUI()
    {
        playerUI.style.display = DisplayStyle.Flex;
        leftPlayerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideLeftPlayerUI()
    {
        leftPlayerUI.style.display = DisplayStyle.None;
    }
    
    public void ShowMiddlePlayerUI()
    {
        playerUI.style.display = DisplayStyle.Flex;
        middlePlayerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideMiddlePlayerUI()
    {
        middlePlayerUI.style.display = DisplayStyle.None;
    }
    
    public void ShowRightPlayerUI()
    {
        playerUI.style.display = DisplayStyle.Flex;
        rightPlayerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideRightPlayerUI()
    {
        rightPlayerUI.style.display = DisplayStyle.None;
    }

    private void SpawnBall(BowlingBallSO _ballSO)
    {
        ballSpawner.SpawnBall(_ballSO);
    }
}
