using UnityEngine;

public class BallBlocker : MonoBehaviour
{
    [SerializeField] private float returnForce = 30f;
    [SerializeField] private Transform returnPoint;

   
    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Player")) return;

        var rb = other.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        // rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce((returnPoint.position - transform.position).normalized * returnForce, ForceMode.Impulse);

        // if (other.TryGetComponent(out Rigidbody rb))
        // {
        //     rb.AddForce(Vector3.back * returnForce);
        // }
    }

}