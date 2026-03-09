using UnityEngine;

public class CameraMountFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    private void LateUpdate()
    {
        transform.position = player.position + offset;
        transform.rotation = Quaternion.identity;
    }
}
