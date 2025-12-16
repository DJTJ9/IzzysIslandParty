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

    private void SpinSpinner()
    {
        var  yRotationAngle = clockwise ? -360f : 360f;
        
        transform.DORotate( new Vector3( 0f,  yRotationAngle, 0f ), spinDuration, RotateMode.LocalAxisAdd)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }
}