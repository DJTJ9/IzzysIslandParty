using TMPro;
using UnityEngine;

public class GUI_SwaggySnapshots : MonoBehaviour
{
    [SerializeField] private GameObject timer;
    [SerializeField] private TMP_Text timerLabel;
    
    private void Update()
    {
        timerLabel.text = SwaggySnapshotsGameManager.RoundTime.ToString("0");
    }
    
    public void ShowTimer() => timer.SetActive(true);
    
    public void HideTimer() => timer.SetActive(false);
}
