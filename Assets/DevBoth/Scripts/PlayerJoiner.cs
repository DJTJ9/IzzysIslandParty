using System.Reflection;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoiner : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;
    
    private bool m_keyboardAssigned;
    
    public Transform[] SpawnPoints;
    public GameObject[] Players;
    
    void Start()
    {
        CreateKeyboardPlayer();
        CreateGamepadPlayers();
    }
    
    private void OnEnable()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        player.GetComponent<PlayerControllerMinigolfMayhem>().Initialize(player.playerIndex);
        
        var cineMachineInput = GetComponentInChildren<CinemachineInputAxisController>();
        cineMachineInput.PlayerIndex = player.playerIndex;
    }

    void CreateKeyboardPlayer()
    {
        if (Keyboard.current == null || m_keyboardAssigned)
            return;

        inputManager.JoinPlayer(
            playerIndex: 0,
            controlScheme: "Keyboard&Mouse",
            pairWithDevice: Keyboard.current
        );

        m_keyboardAssigned = true;
    }

    void CreateGamepadPlayers()
    {
        int playerIndex = 1;

        foreach (var gamepad in Gamepad.all)
        {
            if (playerIndex >= 4)
                break;

            inputManager.JoinPlayer(
                playerIndex: playerIndex,
                controlScheme: "Gamepad",
                pairWithDevice: gamepad
            );

            playerIndex++;
        }
    }
    //
    // private void Awake()
    // {
    //     SpawnPlayer(0);
    //     // Instantiate(Player2, Spawnpoint2.position, Spawnpoint2.rotation);
    //     // Instantiate(Player3, Spawnpoint3.position, Spawnpoint3.rotation);
    // }
    //
    // [Button]
    // public void SpawnPlayer(int playerIndex)
    // {
    //     Instantiate(Players[playerIndex], SpawnPoints[playerIndex].position, SpawnPoints[playerIndex].rotation);
    // }
}
