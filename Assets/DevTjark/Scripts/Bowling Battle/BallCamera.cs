// using DependencyInjection;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 offset;
    
    // [Inject] private BallSpawner ballSpawner;

    private void Update()
    {
        SetCameraPosition();
    }
    
    private void SetCameraPosition()
    {
        // if (ballSpawner.CurrentBallInstance == null) return;
        // transform.position = ballSpawner.CurrentBallInstance.transform.position + offset;
    }
}