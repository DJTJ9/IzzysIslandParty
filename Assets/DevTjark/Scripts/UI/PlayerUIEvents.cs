using DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIEvents : MonoBehaviour
{
    [SerializeField] private BowlingBallCollectionSO ballCollectionSO;
    
    private UIDocument document;
    
    private VisualElement leftPlayerUI;
    private VisualElement middlePlayerUI;
    private VisualElement rightPlayerUI;
    private VisualElement endScreenUI;
    
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
        leftPlayerUI = document.rootVisualElement.Q("player-ui-left__container");
        middlePlayerUI = document.rootVisualElement.Q("player-ui-middle__container");
        rightPlayerUI = document.rootVisualElement.Q("player-ui-right__container");
        endScreenUI = document.rootVisualElement.Q("end-screen-menu__container");
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

    private void SpawnBall(BowlingBallSO _ballSO)
    {
        ballSpawner.SpawnBall(_ballSO);
    }
    
    public void ShowLeftPlayerUI()
    {
        leftPlayerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideLeftPlayerUI()
    {
        leftPlayerUI.style.display = DisplayStyle.None;
    }
    
    public void ShowMiddlePlayerUI()
    {
        middlePlayerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideMiddlePlayerUI()
    {
        middlePlayerUI.style.display = DisplayStyle.None;
    }
    
    public void ShowRightPlayerUI()
    {
        rightPlayerUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideRightPlayerUI()
    {
        rightPlayerUI.style.display = DisplayStyle.None;
    }
    
    public void ShowEndscreenUI()
    {
        endScreenUI.style.display = DisplayStyle.Flex;
    }
    
    public void HideEndscreenUI()
    {
        endScreenUI.style.display = DisplayStyle.None;
    }
}
