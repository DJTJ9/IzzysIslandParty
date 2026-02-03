using UnityEngine;
using UnityEngine.InputSystem;

public class StartCamera : MonoBehaviour
{
    public void DisableStartCamera(PlayerInput _playerInput) => gameObject.SetActive(false);
}
