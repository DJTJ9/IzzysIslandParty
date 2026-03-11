using ScriptableObjects;
using Unity.Cinemachine;
using UnityEngine;

public class MinigolfMayhemCameraController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float defaultMouseSensitivity = 10f;
    [SerializeField] private float gamepadSensitivityMultiplier = 10f;
    
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private SO_FloatVariable mouseSensitivityX;
    [SerializeField] private SO_FloatVariable mouseSensitivityY;
    
    [HideInInspector] public bool isLookingBack;
    
    private CinemachineInputAxisController inputAxisController;
    private PlayerControllerMinigolfMayhem playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerControllerMinigolfMayhem>();
        inputAxisController = transform.parent.GetComponentInChildren<CinemachineInputAxisController>();
        SetMouseSensitivity();
    }

    private void OnEnable()
    {
        GameMenuEvents.OnMouseXSettingsChanged += OnMouseXSensitivityChanged;
        GameMenuEvents.OnMouseYSettingsChanged += OnMouseYSensitivityChanged;
    }
    
    private void OnDisable()
    {
        GameMenuEvents.OnMouseXSettingsChanged -= OnMouseXSensitivityChanged;
        GameMenuEvents.OnMouseYSettingsChanged -= OnMouseYSensitivityChanged;   
    }

    private void SetMouseSensitivity()
    {
        foreach (var c in inputAxisController.Controllers)
        {
            switch (c.Name)
            {
                case "Look Orbit X":
                    if (playerController.m_isGamepad)
                    {
                        c.Input.Gain = defaultMouseSensitivity * gamepadSensitivityMultiplier * mouseSensitivityX.Value;
                    }
                    else
                    {
                        c.Input.Gain = defaultMouseSensitivity * mouseSensitivityX.Value;
                    }
                    break;
                case "Look Orbit Y":
                    if (playerController.m_isGamepad)
                    {
                        c.Input.Gain = -defaultMouseSensitivity * gamepadSensitivityMultiplier * mouseSensitivityY.Value;
                    }
                    else
                    {
                        c.Input.Gain = -defaultMouseSensitivity * mouseSensitivityY.Value;
                    }
                    break;
            }
        }
    }

    private void OnMouseXSensitivityChanged(float _value)
    {
        mouseSensitivityX.Value = _value;
        
        foreach (var c in inputAxisController.Controllers)
        {
            switch (c.Name)
            {
                case "Look Orbit X":
                    c.Input.Gain = defaultMouseSensitivity * mouseSensitivityX.Value;
                    break;
            }
        }
    }
    
    private void OnMouseYSensitivityChanged(float _value)
    {
        mouseSensitivityY.Value = _value;
        
        foreach (var c in inputAxisController.Controllers)
        {
            switch (c.Name)
            {
                case "Look Orbit Y":
                    c.Input.Gain = -defaultMouseSensitivity * mouseSensitivityY.Value;
                    break;
            }
        }
    }

    /// <summary>
    /// Switches the camera to a rear view by rotating it 180 degrees horizontally.
    /// Prevents multiple rear view activations if already enabled.
    /// </summary>
    public void EnableRearView()
    {
        if (isLookingBack) return;
        orbitalFollow.HorizontalAxis.Value += 180f;
        isLookingBack = true;
    }

    /// <summary>
    /// Resets the camera to its original view by rotating it back 180 degrees horizontally.
    /// </summary>
    public void DisableRearView()
    {
        orbitalFollow.HorizontalAxis.Value -= 180f;
        isLookingBack = false;
    }
}
