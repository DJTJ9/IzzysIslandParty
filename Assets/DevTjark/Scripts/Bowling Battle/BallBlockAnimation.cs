using UnityEngine;

public class BallBlockAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) PlayBlockBallAnimation();
    }
    
    /// <summary>
    /// Plays the block ball animation by activating the corresponding trigger
    /// in the animator.
    /// </summary>
    private void PlayBlockBallAnimation()
    {
        animator.SetTrigger("Block");
    }
}
