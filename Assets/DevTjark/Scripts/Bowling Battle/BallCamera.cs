using DependencyInjection;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 offset;
    
    private Quaternion initialRotation;
    
    [Inject] private BallSpawner ballSpawner;
    
    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        SetCameraPosition();
        KeepInitialRotation();
    }
    
    private void SetCameraPosition()
    {
        if (ballSpawner.CurrentBallInstance == null) return;
        transform.position = ballSpawner.CurrentBallInstance.transform.position + offset;
    }

    private void KeepInitialRotation()
    {
        transform.rotation = initialRotation;
    }
}