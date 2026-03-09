using System;
using DependencyInjection;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField, Range(0.01f, 1f)] private float smoothSpeed = 0.1f;

    private void Update()
    {
        SetCameraPosition();
    }

    /// <summary>
    /// Calculates and sets the camera's position based on the target's position
    /// and a specified offset, with a smoothing transition applied.
    /// </summary>
    private void SetCameraPosition()
    {
        if (target == null) return;
    
        Vector3 targetPosition = target.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}