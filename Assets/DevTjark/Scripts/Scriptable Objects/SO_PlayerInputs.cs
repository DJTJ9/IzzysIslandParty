using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Player Inputs", menuName = "Scriptable Objects/Player Inputs")]
public class SO_PlayerInputs : SerializedScriptableObject
{
    public List<PlayerInput> PlayerInputs = new List<PlayerInput>();
    
    void OnDisable() => PlayerInputs.Clear();
}
