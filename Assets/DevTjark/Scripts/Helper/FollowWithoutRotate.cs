using UnityEngine;

public class FollowWithoutRotate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    

    private void Update()
    {
        SetPosition();
    }
    
    private void SetPosition()
    {
        transform.position = target.transform.position + offset;
    }
}