using UnityEngine;

public class Respawner : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var rb = other.GetComponent<Rigidbody>();
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            other.transform.position = respawnPoint.position;
        }
    }
}
