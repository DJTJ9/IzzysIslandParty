using Unity.Cinemachine;
using UnityEngine;

public class GoalCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineOrbitalFollow followCamera;

    /// <summary>
    /// Adjusts the goal camera's horizontal and vertical axis values
    /// to predefined angles, ensuring an optimal viewing position in finish hole.
    /// </summary>
    public void SetGoalCameraValues()
    {
        followCamera.HorizontalAxis.Value = -88f;
        followCamera.VerticalAxis.Value = 20f;
    }
}
