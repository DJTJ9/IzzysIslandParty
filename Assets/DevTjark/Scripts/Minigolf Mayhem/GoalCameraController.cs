using Unity.Cinemachine;
using UnityEngine;

public class GoalCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera followCamera;
    private CinemachineCamera goalCamera;
    
    public void Init(CinemachineCamera _goalCamera)
    {
        goalCamera = _goalCamera;
    }

    public void SwitchToGoalCamera()
    {
        followCamera.Priority = 0;
        goalCamera.Priority = 10;
    }
}
