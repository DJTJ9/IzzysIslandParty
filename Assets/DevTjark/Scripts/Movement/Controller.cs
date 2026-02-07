using UnityEngine;

public class Controller : MonoBehaviour
{
    public int PlayerIndex;

    public int GetPlayerIndex() => PlayerIndex;
    
    public void SetPlayerIndex(int _playerIndex) => PlayerIndex = _playerIndex;
}
