using System;
using ImprovedTimers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class NPC_SwaggySnapshots : Controller
{
    private CountdownTimer m_photoTimer;

    private void Awake()
    {
        m_photoTimer = new CountdownTimer(Random.Range(SwaggySnapshotsGameManager.StartMoveDuration, SwaggySnapshotsGameManager.StartMoveDuration + SwaggySnapshotsGameManager.RoundTime));
        m_photoTimer.OnTimerStop += TakePhoto;
    }

    private void Start()
    {
        m_photoTimer.Start();
    }

    private void TakePhoto()
    {
        PlayerControllerSwaggySnapshots.InvokePhotoTaken(GetPlayerIndex());
    }
}
