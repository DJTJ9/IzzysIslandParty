using UnityEngine;

public class CameraMountFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = player.position + offset;
        transform.rotation = Quaternion.identity;
    }
}
