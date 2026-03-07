using System;
using ImprovedTimers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class NPC_SwaggySnapshots : Controller
{
    private PhotoCapture photoCapture;
    private CountdownTimer m_photoTimer;

    private void Awake()
    {
        photoCapture = GetComponent<PhotoCapture>();
        
        m_photoTimer = new CountdownTimer(Random.Range(SwaggySnapshotsGameManager.StartMoveDuration, SwaggySnapshotsGameManager.StartMoveDuration + SwaggySnapshotsGameManager.RoundTime));
        m_photoTimer.OnTimerStop += TakePhoto;
    }

    private void Start()
    {
        m_photoTimer.Start();
    }

    private void TakePhoto()
    {
        Debug.Log($"NPC {GetPlayerIndex()} hat ein Foto gemacht!");
        PlayerControllerSwaggySnapshots.InvokePhotoTaken(GetPlayerIndex());
    }
}
