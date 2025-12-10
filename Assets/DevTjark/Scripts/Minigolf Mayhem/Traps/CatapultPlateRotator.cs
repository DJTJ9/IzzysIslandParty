using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CatapultPlateRotator : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float upwardRotationTime = 0.2f;
    [SerializeField] private float downwardRotationTime = 0.8f;
    [SerializeField] private float pauseTimeBetweenRotations = 0.5f;
    void Start()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOLocalRotate(new Vector3(-90f, 0f, 0f), upwardRotationTime)  
                .SetEase(Ease.OutQuad))
            .AppendInterval(pauseTimeBetweenRotations)                               
            .Append(transform.DOLocalRotate(Vector3.zero, downwardRotationTime) 
                .SetEase(Ease.InOutQuad))
            .AppendInterval(pauseTimeBetweenRotations)                                       
            .SetLoops(-1);
    }
}