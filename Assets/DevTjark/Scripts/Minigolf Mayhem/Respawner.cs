using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    /// <summary>
    /// Detects when a player enters the trigger zone. Resets the player's velocity
    /// and positions them at the defined respawn point.
    /// </summary>
    /// <param name="_other">The collider that entered the trigger zone.</param>
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