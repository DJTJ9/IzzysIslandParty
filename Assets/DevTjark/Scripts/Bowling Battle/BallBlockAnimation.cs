using UnityEngine;

public class BallBlockAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) PlayBlockBallAnimation();
    }
    
    private void PlayBlockBallAnimation()
    {
        animator.SetTrigger("Block");
    }
}
