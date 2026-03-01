using Unity.Cinemachine;
using UnityEngine;

public class MinigolfMayhemCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    public void EnableRearView()
    {
        orbitalFollow.HorizontalAxis.Value += 180f;
    }
}
