using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private SO_PlayerInputs connectedPlayers;


    public void OnPlayerJoin(PlayerInput _playerInput)
    {
        connectedPlayers.PlayerInputs.Add(_playerInput);
        // _playerInput.gameObject.GetComponent<DontDestroyOnLoad>().PlayerIndex = connectedPlayers.PlayerInputs.Count - 1;
    }
    
    public void OnPlayerLeave(PlayerInput _playerInput) => connectedPlayers.PlayerInputs.Remove(_playerInput);
}
