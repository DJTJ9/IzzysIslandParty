using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInputs", menuName = "Scriptable Objects/Player Inputs")]
public class SO_PlayerInputs : SerializedScriptableObject
{
    public List<PlayerInput> PlayerInputs;
}
