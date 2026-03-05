using DG.Tweening;
using UnityEngine;

public class DiscoBallSpinner : MonoBehaviour
{
    [SerializeField] private float rotationTime = 3f;
    
    private void Start()
    {
        StartSpinning();
    }

    private void StartSpinning()
    {
        transform.DOLocalRotate(new Vector3(0, 360, 0), rotationTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
    }
}
