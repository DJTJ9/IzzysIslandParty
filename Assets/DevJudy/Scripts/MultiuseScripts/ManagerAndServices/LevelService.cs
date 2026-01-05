using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelService : MonoBehaviour
{
    private const string levelStartText = "START";
    private const string dotsText = "...";
    
    [SerializeField] private TextMeshProUGUI levelCountdownText;
    [SerializeField] private int secondsToStartLevel;

    [SerializeField] private UnityEvent onLevelStart;
    [SerializeField] private UnityEvent onLevelEnd;

    private void Start()
    {
        StartCoroutine(LevelStart());
    }

    private void OnCoroutineOver()
    {
        StopCoroutine(LevelStart());
        
        onLevelStart.Invoke();
    }
    
    private IEnumerator LevelStart()
    {
        levelCountdownText.enabled = true;
        
        for (int i = secondsToStartLevel; i > 0; i--)
        {
            levelCountdownText.text = i.ToString() + dotsText;
            
            yield return new WaitForSecondsRealtime(1f);
        }
        
        levelCountdownText.text = levelStartText;
        
        yield return new WaitForSecondsRealtime(1f);
        
        levelCountdownText.enabled = false;
        
        OnCoroutineOver();
        
        yield return null;
    }

    public void EndLevel()
    {
        onLevelEnd.Invoke();
    }
}