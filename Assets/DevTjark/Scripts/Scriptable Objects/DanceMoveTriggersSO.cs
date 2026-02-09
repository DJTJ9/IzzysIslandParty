using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Dance Move Triggers", menuName = "Scriptable Objects/Swaggy Snapshots/Dance Move Triggers", order = 1)]
public class DanceMoveTriggersSO : SerializedScriptableObject
{
    public Dictionary<DanceMoveTriggers, string> DanceMoveTriggersDictionary = new Dictionary<DanceMoveTriggers, string>();

    private DanceMoveTriggers m_lastKey;
    private bool m_hasLastKey; 
    
    public string GetRandomDanceMove()
    {
        if (DanceMoveTriggersDictionary == null || DanceMoveTriggersDictionary.Count == 0)
            throw new System.InvalidOperationException("Dictionary is empty.");
        
        if (DanceMoveTriggersDictionary.Count == 1)
            return DanceMoveTriggersDictionary.Values.First();

        DanceMoveTriggers newKey;

        do
        {
            var index = Random.Range(0, DanceMoveTriggersDictionary.Count);
            newKey = DanceMoveTriggersDictionary.ElementAt(index).Key;
        }
        while (m_hasLastKey && newKey == m_lastKey);
        
        m_lastKey = newKey;
        m_hasLastKey = true;
        
        return DanceMoveTriggersDictionary[newKey];
    }
}
