using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Bowling Ball", menuName = "Scriptable Objects/Bowling Ball", order = 1)]
public class SO_PlayerBowlingBattle : SO_Player
{
    public BallType CurrentBallType;

    public override void InitializePlayer(GameObject _player, SO_Player _playerSO, int _playerIndex)
    {
        var playerController = _player.GetComponent<PlayerControllerBowlingBattle>();
        playerController.BindPlayerSO(_playerSO);
        playerController.ResetComponents();
    }
}