using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [FoldoutGroup("Photo Objects", expanded: true)]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    
    [FoldoutGroup("Flash Light Effect", expanded: true)]
    [SerializeField] private GameObject flashLight;
    [SerializeField] private float flashLightDuration = 0.2f;
    
    [FoldoutGroup("Photo Fade Effect", expanded: true)]
    [SerializeField] private Animator fadingAnimator;
    [SerializeField] private float fadeInSpeed = 1f;
    
    [SerializeField] private UnityEvent onPhotoTaken;
    
    private Texture2D m_screenCapture;
    private bool m_photoTaken;
    
    private readonly int m_fadeInAnimationHash = Animator.StringToHash("PhotoFadeIn");
    
    private void Start()
    {
        m_screenCapture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    [Button]
    private void TakePhoto() => StartCoroutine(CaptureScreenshot());
    
    private IEnumerator CaptureScreenshot()
    {
        if (m_photoTaken) yield break;
        m_photoTaken = true;
        
        onPhotoTaken.Invoke();
        StartCoroutine(FlashLightEffect());
        
        yield return new WaitForEndOfFrame();
        
        var rect = new Rect(0, 0, Screen.width, Screen.height);
        m_screenCapture.ReadPixels(rect, 0, 0, false);
        m_screenCapture.Apply();
        ShowScreenshot();
        fadingAnimator.speed = fadeInSpeed;
        fadingAnimator.Play(m_fadeInAnimationHash);
    }

    private void ShowScreenshot()
    {
        Sprite photoSprite = Sprite.Create(m_screenCapture, new Rect(0, 0, m_screenCapture.width, m_screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        photoDisplayArea.sprite = photoSprite;
        
        photoFrame.SetActive(true);
    }

    private IEnumerator FlashLightEffect()
    {
        flashLight.SetActive(true);
        yield return new WaitForSeconds(flashLightDuration);
        flashLight.SetActive(false);
    }

    [Button]
    private void HideScreenshot()
    {
        m_photoTaken = false;
        photoFrame.SetActive(false);
    } 
}
