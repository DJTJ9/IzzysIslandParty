using System;
using ImprovedTimers;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPC_SwaggySnapshots : Controller
{
    [SerializeField] private float photoEndMargin = 0.5f;

    private PhotoCapture photoCapture;
    private CountdownTimer m_photoTimer;

    /// <summary>
    /// Initializes a countdown timer for NPC photo-taking, with a random duration
    /// based on game configuration, and sets up the callback to trigger taking a photo.
    /// </summary>
    private void Awake()
    {
        photoCapture = GetComponent<PhotoCapture>();

        var photoWindowStart = SwaggySnapshotsGameManager.StartMoveDuration;
        var photoWindowEnd = SwaggySnapshotsGameManager.StartMoveDuration + SwaggySnapshotsGameManager.RoundTime - photoEndMargin;
        m_photoTimer = new CountdownTimer(Random.Range(photoWindowStart, photoWindowEnd), autoTick: true);
        m_photoTimer.OnTimerStop += TakePhoto;
    }

    private void Start()
    {
        m_photoTimer.Start();
    }

    private void OnDestroy()
    {
        m_photoTimer.Dispose();
    }

    /// <summary>
    /// Invokes the photo-taking event for the NPC, using the player's index
    /// to notify the relevant controllers.
    /// </summary>
    private void TakePhoto()
    {
        if (!photoCapture.CanTakePhoto()) return;

        PlayerControllerSwaggySnapshots.InvokePhotoTaken(GetPlayerIndex());
        photoCapture.ShowFlashLight();
    }
}
