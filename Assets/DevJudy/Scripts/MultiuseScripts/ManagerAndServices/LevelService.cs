using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LevelService : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelCountdownText;
    [SerializeField] private int secondsToStartLevel;

    [SerializeField] private UnityEvent onLevelStart;
    private void Start()
    {
        // inputActions.disbale
        StartCoroutine(LevelStart());
    }

    private void OnCoroutineOver()
    {
        StopCoroutine(LevelStart());
        
        // inputActions.enable
        onLevelStart.Invoke();
    }

    private IEnumerator LevelStart()
    {
        levelCountdownText.enabled = true;
        
        for (int i = secondsToStartLevel; i > 0; i--)
        {
            levelCountdownText.text = i.ToString() + "...";
            
            yield return new WaitForSecondsRealtime(1f);
        }
        
        levelCountdownText.text = "START";
        
        yield return new WaitForSecondsRealtime(1f);
        
        levelCountdownText.enabled = false;
        
        OnCoroutineOver();
        
        yield return null;
    }
}