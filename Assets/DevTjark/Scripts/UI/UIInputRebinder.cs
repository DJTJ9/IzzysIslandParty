using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class UIInputRebinder : MonoBehaviour
{
    [SerializeField] private InputSystemUIInputModule uiModule;
    [SerializeField] private InputActionAsset uiActions;

    private void Start()
    {
        RebindUIInput();
    }

    public void RebindUIInput()
    {
        // uiModule.actionsAsset = null;
        uiModule.actionsAsset = uiActions;
    }
}
