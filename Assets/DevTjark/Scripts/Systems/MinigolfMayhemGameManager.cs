using System;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfMayhemGameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onGameStart;

    private void Awake()
    {
        onGameStart.Invoke();
    }

    private void Update()
    {
#if !UNITY_EDITOR
// <-- Timers.Tick() here
#endif
    }
}
