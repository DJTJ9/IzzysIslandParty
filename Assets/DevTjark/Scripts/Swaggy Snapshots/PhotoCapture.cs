using System.Collections;
using Audio;
using enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [FoldoutGroup("Photo Objects", expanded: true)]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    private RectTransform photoFrameRectTransform;
    
    [FoldoutGroup("Score Label", expanded: true)]
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text scoreNumber;
    [SerializeField] private TMP_Text happyScoreLabel;
    [SerializeField] private TMP_Text faceScoreLabel;
    [SerializeField] private TMP_Text coolMoveScoreLabel;
    
    [FoldoutGroup("Flash Light Effect", expanded: true)]
    [SerializeField] private GameObject flashLight;
    [SerializeField] private float flashLightDuration = 0.2f;
    
    [FoldoutGroup("Photo Fade Effect", expanded: true)]
    [SerializeField] private Animator fadingAnimator;
    [SerializeField] private float fadeInSpeed = 1f;

    [SerializeField] private SO_SwaggySnapshotsPlayerCollection currentPlayersSO;
    private Controller controller;
    
    private Texture2D m_screenCapture;
    private bool m_canTakePhoto;
    private bool m_photoTaken;
    
    private readonly int m_fadeInAnimationHash = Animator.StringToHash("PhotoFadeIn");
    
    private void Start()
    {
        controller = GetComponent<Controller>();
        photoFrameRectTransform = photoFrame.GetComponent<RectTransform>();
        m_screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }
    
    /// <summary>
    /// Updates the score labels on the screen with the current player's score 
    /// and other related values like happy score, facing camera score, and cool move score.
    /// </summary>
    private void Update()
    {
        if (currentPlayersSO.Players[controller.PlayerIndex] == null) return;
        scoreNumber.text = currentPlayersSO.Players[controller.PlayerIndex].PlayerScore.Value.ToString("0");
        happyScoreLabel.text = $"+{currentPlayersSO.Players[controller.PlayerIndex].HappyFaceScore}";
        faceScoreLabel.text = $"+{currentPlayersSO.Players[controller.PlayerIndex].FacingCameraScore}";
        coolMoveScoreLabel.text = $"+{currentPlayersSO.Players[controller.PlayerIndex].CoolDanceMoveScore}";
    }

    private void OnEnable()
    {
        PlayerControllerSwaggySnapshots.onTakePhoto += TakePhoto;
    }
    
    private void OnDisable()
    {
        PlayerControllerSwaggySnapshots.onTakePhoto -= TakePhoto;
    }

    /// <summary>
    /// Starts the process of capturing a photo for the player at the specified index 
    /// when the photo-taking event is triggered.
    /// </summary>
    /// <param name="_playerIndex">The index of the player taking the photo.</param>
    private void TakePhoto(int _playerIndex)
    {
        if (_playerIndex != controller.GetPlayerIndex()) return;
        StartCoroutine(CaptureScreenshot());
    }
    
    /// <summary>
    /// Returns whether the player is currently allowed to take a photo.
    /// </summary>
    public bool CanTakePhoto() => !m_photoTaken && m_canTakePhoto;

    /// <summary>
    /// Captures a screenshot of the game screen, applies it to the `m_screenCapture` texture, 
    /// and plays associated visual and audio effects like flashlight and sound.
    /// </summary>
    private IEnumerator CaptureScreenshot()
    {
        if (m_photoTaken || !m_canTakePhoto) yield break;
        m_photoTaken = true;
        
        PlayCameraSound();
        ShowFlashLight();
        
        yield return new WaitForEndOfFrame();
        
        var rect = new Rect(0, 0, Screen.width, Screen.height);
        m_screenCapture.ReadPixels(rect, 0, 0, false);
        m_screenCapture.Apply();
    }

    /// <summary>
    /// Starts the fade-in animation for the photo display area, controlled by 
    /// the defined fade-in speed and animation hash.
    /// </summary>
    private void FadeInPhoto()
    {
        fadingAnimator.speed = fadeInSpeed;
        fadingAnimator.Play(m_fadeInAnimationHash);
    }
    
    /// <summary>
    /// Displays the captured screenshot on the photo frame area and positions it at 
    /// the player's spawn point, applying a fade-in effect.
    /// </summary>
    public void ShowScreenshot()
    {
        photoFrameRectTransform.anchoredPosition = currentPlayersSO.Players[controller.PlayerIndex].SpawnPoint;
        Sprite photoSprite = Sprite.Create(m_screenCapture, new Rect(0, 0, m_screenCapture.width, m_screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        photoDisplayArea.sprite = photoSprite;
        
        photoFrame.SetActive(true);
        FadeInPhoto();
    }

    /// <summary>
    /// Displays and hides the flashlight effect briefly to simulate the camera flash.
    /// </summary>
    private IEnumerator FlashLightEffect()
    {
        flashLight.SetActive(true);
        yield return new WaitForSeconds(flashLightDuration);
        flashLight.SetActive(false);
    }
    
    /// <summary>
    /// Starts the flashlight effect by triggering the coroutine.
    /// </summary>
    public void ShowFlashLight() => StartCoroutine(FlashLightEffect());
    
    /// <summary>
    /// Plays the camera click sound effect when a photo is taken.
    /// </summary>
    private void PlayCameraSound()
    {
        AudioService.Instance.PlaySimpleSound(AudioCollection.Instance.LevelSoundsDictionary.LevelAudios["CameraClick"], EAudioType.SFX);
    }
    
    /// <summary>
    /// Hides the photo screenshot display and resets the photo-taking flag.
    /// </summary>
    public void HideScreenshot()
    {
        m_photoTaken = false;
        photoFrame.SetActive(false);
    } 
    
    /// <summary>
    /// Sets whether the player can take a photo by updating the `m_canTakePhoto` flag.
    /// </summary>
    /// <param name="_canTakePhoto">Indicates if photo-taking is allowed.</param>
    public void SetCanTakePhoto(bool _canTakePhoto) => m_canTakePhoto = _canTakePhoto;
}
