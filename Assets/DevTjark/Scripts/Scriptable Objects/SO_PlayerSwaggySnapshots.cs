using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Player Swaggy Snapshots", menuName = "Scriptable Objects/Swaggy Snapshots/Player", order = 1)]
public class SO_PlayerSwaggySnapshots : SO_Player
{
    public override void InitializePlayer(GameObject _player, SO_Player _playerSO, int _playerIndex)
    {
        var playerController = _player.GetComponent<PlayerControllerSwaggySnapshots>();
        playerController.SetPlayerIndex(_playerIndex);
        // playerController.BindPlayerSO(_playerSO);
        // playerController.ResetComponents();
    }
}
