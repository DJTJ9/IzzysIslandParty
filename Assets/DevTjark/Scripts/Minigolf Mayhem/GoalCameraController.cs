using Unity.Cinemachine;
using UnityEngine;

public class GoalCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow followCamera;
    private CinemachineCamera goalCamera;
    
    public void Init(CinemachineCamera _goalCamera)
    {
        goalCamera = _goalCamera;
    }

    public void SetGoalCameraValues()
    {
        followCamera.HorizontalAxis.Value = -88f;
        followCamera.VerticalAxis.Value = 20f;
    }
}
