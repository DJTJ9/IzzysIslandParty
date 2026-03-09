using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [FoldoutGroup("Spinner Settings", expanded: true)]
    [SerializeField] private float spinDuration = 1f;
    [SerializeField] private bool clockwise = true;
    
    private void FixedUpdate()
    {
        SpinSpinner();
    }

    /// <summary>
    /// Rotates the spinner along the Y-axis with a specified duration and direction (clockwise or counterclockwise),
    /// using a smooth infinite looping animation.
    /// </summary>
    private void SpinSpinner()
    {
        var  yRotationAngle = clockwise ? -360f : 360f;
        
        transform.DORotate( new Vector3( 0f,  yRotationAngle, 0f ), spinDuration, RotateMode.LocalAxisAdd)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}