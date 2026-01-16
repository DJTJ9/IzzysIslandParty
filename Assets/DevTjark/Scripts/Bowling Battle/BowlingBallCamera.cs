using DependencyInjection;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    
    private void Update()
    {
        SetCameraPosition();
    }
    
    private void SetCameraPosition()
    {
        if (target == null) return;
        transform.position = target.transform.position + offset;
    }
}