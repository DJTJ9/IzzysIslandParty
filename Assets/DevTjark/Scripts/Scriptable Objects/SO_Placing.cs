using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Placing SO", menuName = "Scriptable Objects/Placing", order = 1)]
public class SO_Placing : SerializedScriptableObject
{
    public List<int> playerPlacing = new();

    private void OnEnable()
    {
        playerPlacing.Clear();
    }
    
    public void AddPlayerToPlacingList(int _playerIndex)
    {
        playerPlacing.Add(_playerIndex);
    } 
    
    public int GetIndexOfPlayer(int _playerIndex)
    {
       var place = playerPlacing.IndexOf(_playerIndex);
       return place;
    }
}
