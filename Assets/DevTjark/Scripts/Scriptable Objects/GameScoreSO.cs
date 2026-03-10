using UnityEngine;

[CreateAssetMenu(fileName = "Game Score", menuName = "Scriptable Objects/Game Score", order = 1)]
public class GameScoreSO : ScriptableObject
{
    public float Value;
    
    private void OnEnable()
    {
        ResetScore();
    }

    public void ResetScore()
    {
        Value = 0f;
    }
}