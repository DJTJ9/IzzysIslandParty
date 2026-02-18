using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationService : MonoBehaviour
{
    private static LocationService instance;
    public static LocationService Instance => instance;

    private Dictionary<string, Transform> dynamicTransforms = new Dictionary<string, Transform>();

    public event Action<string, Vector3> OnTransformPositionChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RegisterTransform(string key, Transform transform)
    {
        dynamicTransforms[key] = transform;
    }

    public Transform GetTransform(string key)
    {
        return dynamicTransforms.GetValueOrDefault(key);
    }

    public void UpdatePosition(string key, Vector3 newPosition)
    {
        if (dynamicTransforms.TryGetValue(key, out var transform))
        {
            transform.position = newPosition;
            OnTransformPositionChanged?.Invoke(key, newPosition);
        }
    }
}