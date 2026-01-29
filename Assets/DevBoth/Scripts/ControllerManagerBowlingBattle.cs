using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManagerBowlingBattle : MonoBehaviour
{
    [SerializeField] private SO_PlayerInputs connectedPlayers;

    private PlayerInput playerInput;

    public void OnPlayerJoin(PlayerInput _playerInput)
    {
        connectedPlayers.PlayerInputs.Add(_playerInput);
    }

    public PlayerInput GetPlayerInput() => playerInput;
}
