using ImprovedTimers;
using Random = UnityEngine.Random;

public class NPC_SwaggySnapshots : Controller
{
    private PhotoCapture photoCapture;
    private CountdownTimer m_photoTimer;

    /// <summary>
    /// Initializes a countdown timer for NPC photo-taking, with a random duration
    /// based on game configuration, and sets up the callback to trigger taking a photo.
    /// </summary>
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

    /// <summary>
    /// Invokes the photo-taking event for the NPC, using the player's index
    /// to notify the relevant controllers.
    /// </summary>
    private void TakePhoto()
    {
        PlayerControllerSwaggySnapshots.InvokePhotoTaken(GetPlayerIndex());
        photoCapture.ShowFlashLight();
    }
}
