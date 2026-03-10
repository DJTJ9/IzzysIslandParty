using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public int PlayerIndex;
    protected bool pauseInputEnabled;
    protected PlayerInput playerInput;
    protected bool m_isActive = true;

    private void OnEnable()
    {
        EnableController();
    }

    private void OnDisable()
    {
        DisableController();
    }

    public int GetPlayerIndex() => PlayerIndex;
    
    public void SetPlayerIndex(int _playerIndex) => PlayerIndex = _playerIndex;

    public virtual void OnNPCJoined(SO_PlayerRacingGames _player) { }

    public void SwitchToUIInputMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public virtual void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    public void EnableController() => m_isActive = true;
    public void DisableController() => m_isActive = false;
    public void EnablePauseInput() => pauseInputEnabled = true;
    public void DisablePauseInput() => pauseInputEnabled = false;
}
