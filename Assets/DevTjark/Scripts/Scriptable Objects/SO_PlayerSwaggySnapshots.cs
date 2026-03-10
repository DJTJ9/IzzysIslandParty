using UnityEngine;

[CreateAssetMenu(fileName = "Player Swaggy Snapshots", menuName = "Scriptable Objects/Swaggy Snapshots/Player", order = 1)]
public class SO_PlayerSwaggySnapshots : SO_Player
{
    public float HappyFaceScore;
    public float FacingCameraScore;
    public float CoolDanceMoveScore;

    private void OnEnable()
    {
        ResetScoreValues();
    }

    public override void InitializePlayer(GameObject _player, SO_Player _playerSO, int _playerIndex)
    {
        var playerController = _player.GetComponent<PlayerControllerSwaggySnapshots>();
        playerController.SetPlayerIndex(_playerIndex);
    }

    public void ResetScoreValues()
    {
        HappyFaceScore = 0;
        FacingCameraScore = 0;
        CoolDanceMoveScore = 0;
    }
}
