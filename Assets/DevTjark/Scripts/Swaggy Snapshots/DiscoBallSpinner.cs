using DG.Tweening;
using UnityEngine;

public class DiscoBallSpinner : MonoBehaviour
{
    [SerializeField] private float rotationTime = 3f;
    
    private void Start()
    {
        StartSpinning();
    }

    /// <summary>
    /// Initiates continuous spinning of the disco ball with a complete 360-degree rotation 
    /// over the specified duration, set to loop infinitely with linear easing.
    /// </summary>
    private void StartSpinning()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), rotationTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
    }
}
