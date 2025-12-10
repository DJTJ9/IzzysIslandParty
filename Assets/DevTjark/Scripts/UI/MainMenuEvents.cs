using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument document;

    private Button button;
    
    private List<Button> menuButtons = new List<Button>();
    
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        
        button = document.rootVisualElement.Q("StartGameButton") as Button;
        button?.RegisterCallback<ClickEvent>(OnPlayGameClick);
        
        menuButtons = document.rootVisualElement.Query<Button>().ToList();
        foreach (var menuButton in menuButtons)
        {
            menuButton.RegisterCallback<ClickEvent>(OnAllButtonsClicked);
        }
    }

    private void OnDisable()
    {
        button.UnregisterCallback<ClickEvent>(OnPlayGameClick);

        foreach (var menuButton in menuButtons)
        {
            menuButton.UnregisterCallback<ClickEvent>(OnAllButtonsClicked);
        }
    }

    private void OnPlayGameClick(ClickEvent _evt)
    {
        Debug.Log("Play Game Button Clicked");
    }

    private void OnAllButtonsClicked(ClickEvent _evt)
    {
        Debug.Log("One Of The Menu Buttons Clicked");
    }
}