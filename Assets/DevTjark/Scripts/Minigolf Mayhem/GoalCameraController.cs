using Unity.Cinemachine;
using UnityEngine;

public class GoalCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow followCamera;

    public void SetGoalCameraValues()
    {
        followCamera.HorizontalAxis.Value = -88f;
        followCamera.VerticalAxis.Value = 20f;
    }
}
