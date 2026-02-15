using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUIMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private RigidbodyMovement rigidbodyMovement;
    [SerializeField] private Image shootForceBar;


    private void Start()
    {
        shootForceBar.fillAmount = 0;
    }

    private void Update()
    {
        // Calculate normalized value between 0 and 1
        float normalizedForce = (rigidbodyMovement.CurrentShootForce - rigidbodyMovement.minShootForce) /
                                (rigidbodyMovement.maxShootForce - rigidbodyMovement.minShootForce);

        // Ensure the value stays between 0 and 1
        shootForceBar.fillAmount = Mathf.Clamp01(normalizedForce);
    }
}