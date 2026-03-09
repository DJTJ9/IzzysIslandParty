using Unity.Cinemachine;
using UnityEngine;

public class MinigolfMayhemCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [HideInInspector] public bool isLookingBack;

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
