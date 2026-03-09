using System;
using UnityEngine;

public class PinReseter : MonoBehaviour
{
    private Vector3 startPosition;
    
    private Rigidbody rb;

    private void Awake()
    {
        startPosition = transform.position;
        
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Resets the pin's position, rotation, and velocity back to its starting state.
    /// </summary>
    public void ResetPinPosition()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }
}