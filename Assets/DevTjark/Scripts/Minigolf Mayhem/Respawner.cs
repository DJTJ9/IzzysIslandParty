using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            _rb.linearVelocity = new Vector3(0, 0, 0);
            _other.transform.position = respawnPoint.position;
        }
    }
}