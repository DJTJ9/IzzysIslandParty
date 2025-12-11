using Sirenix.OdinInspector;
using UnityEngine;

public class CatapultPlateImpulse : MonoBehaviour
{
    [FoldoutGroup("Impulse Settings", expanded: true)]
    [SerializeField] private float upwardsForce = 25f;
    [SerializeField] private float sidewardsForce = 25f;

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.TryGetComponent<Rigidbody>(out var rb)) return;
        
        rb.AddForce(transform.up * upwardsForce, ForceMode.Impulse);
        rb.AddForce(-transform.forward * sidewardsForce, ForceMode.Impulse);
    }
}