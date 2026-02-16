using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUIMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private RigidbodyMovement rigidbodyMovement;
    [SerializeField] private Image shootForceBar;
    [SerializeField] private TMP_Text shootForceText;


    private void Start()
    {
        shootForceBar.fillAmount = 0;
    }

    private void Update()
    {
        var normalizedForce = (rigidbodyMovement.CurrentShootForce - rigidbodyMovement.minShootForce) / (rigidbodyMovement.maxShootForce - rigidbodyMovement.minShootForce);
        shootForceBar.fillAmount = Mathf.Clamp01(normalizedForce);

        shootForceText.text = $"{Mathf.RoundToInt(normalizedForce * 100).ToString()}%";
    }
}