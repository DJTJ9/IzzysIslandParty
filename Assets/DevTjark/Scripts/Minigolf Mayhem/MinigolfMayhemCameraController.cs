using Unity.Cinemachine;
using UnityEngine;

public class MinigolfMayhemCameraController : MonoBehaviour
{
    [HideInInspector] public bool isLookingBack;
    
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    public void EnableRearView()
    {
        if (isLookingBack) return;
        orbitalFollow.HorizontalAxis.Value += 180f;
        isLookingBack = true;
    }

    public void DisableRearView()
    {
        orbitalFollow.HorizontalAxis.Value -= 180f;
        isLookingBack = false;
    }
}
