using System.Collections.Generic;
using enums;
using FishingGame.QuickTimeEvents;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/SO_QTEDictionary")]
public class SO_QTEDictionary : SerializedScriptableObject
{
    public Dictionary<EQuickTimeEvent, QuickTimeEvent> CatchEvents = new Dictionary<EQuickTimeEvent, QuickTimeEvent>();
}
