using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class LocationService : MonoBehaviour
{
    private static LocationService instance;
    public static LocationService Instance => instance;

    private Dictionary<string, Transform> dynamicTransforms = new();

    public event Action<string, Vector3> OnTransformPositionChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void RegisterTransform(string _key, Transform _transform)
    {
        dynamicTransforms[_key] = _transform;
    }

    public Transform GetTransform(string _key)
    {
        return dynamicTransforms.GetValueOrDefault(_key);
    }

    public void UpdatePosition(string _key, Vector3 _newPosition)
    {
        if (dynamicTransforms.TryGetValue(_key, out var _transform))
        {
            _transform.position = _newPosition;
            OnTransformPositionChanged?.Invoke(_key, _newPosition);
        }
    }
}