using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PlayerUIMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private UIDocument document;

    private VisualElement playerUI;
    private VisualElement leftPlayerUI;
    private VisualElement middlePlayerUI;
    private VisualElement rightPlayerUI;

    private void Awake()
    {
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
}
