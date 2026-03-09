using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraIntro : MonoBehaviour
{
    [SerializeField] private GameObject barrelDistortionVolume;
    [SerializeField] private GameObject cameraOptic;
    
    [SerializeField] private Vector3 firstPosition;
    [SerializeField] private float firstMoveTime;
    [SerializeField] private float firstStopTime;
    [SerializeField] private float rotationTime;
    [SerializeField] private float secondStopTime;
    [SerializeField] private Vector3 secondPosition;
    [SerializeField] private float secondMoveTime;
    [SerializeField] private AnimationCurve cameraUpwardsAnimationCurve,cameraSpinAnimationCurve, cameraForwardAnimationCurve;

    public void StartCameraIntroSequence() => StartCoroutine(StartCameraIntro());
    
    /// <summary>
    /// Plays out the camera's introductory sequence, which includes:
    /// - Moving to a specified position with a smoothing animation curve.
    /// - Pausing and then rotating locally by 900 degrees.
    /// - Moving to a second position with another animation curve.
    /// - Activating visual effects like barrel distortion and camera optics.
    /// - Disabling the game object after finishing the sequence.
    /// </summary>
    private IEnumerator StartCameraIntro()
    {
        yield return transform.DOMove(firstPosition, firstMoveTime).SetEase(cameraUpwardsAnimationCurve).WaitForCompletion();
        yield return new WaitForSeconds(firstStopTime);
        yield return transform.DOLocalRotate(new Vector3(0, 900, 0), rotationTime, RotateMode.LocalAxisAdd).SetEase(cameraSpinAnimationCurve).WaitForCompletion();
        yield return new WaitForSeconds(secondStopTime);
        yield return transform.DOMove(secondPosition, secondMoveTime).SetEase(cameraForwardAnimationCurve).WaitForCompletion();
        barrelDistortionVolume.SetActive(true);
        cameraOptic.SetActive(true);
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}