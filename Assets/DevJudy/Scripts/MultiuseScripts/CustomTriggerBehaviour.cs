using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CustomTriggerBehaviour : MonoBehaviour
{
    public LayerMask collisionLayer;
    
    public event Action<Collider> EnteredTriggerAction;
    public event Action<Collider> ExitedTriggerAction;


    private void Awake()
    {
        var component = GetComponent<Collider>();
        component.isTrigger = true;
    }
    
    public void OnTriggerEnter(Collider _other)
    {
        if (_other.gameObject.layer.CompareTo(collisionLayer) != 0)
            EnteredTriggerAction?.Invoke(_other);
    }

    public void OnTriggerExit(Collider _other)
    {
        if (_other.gameObject.layer == collisionLayer.value)
            ExitedTriggerAction?.Invoke(_other);
    }
}