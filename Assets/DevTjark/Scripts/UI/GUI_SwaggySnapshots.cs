using TMPro;
using UnityEngine;

public class GUI_SwaggySnapshots : MonoBehaviour
{
    [SerializeField] private GameObject timer;
    [SerializeField] private TMP_Text timerLabel;
    
    /// <summary>
    /// Updates the timer label every frame to display the current round time
    /// from the game manager.
    /// </summary>
    private void Update()
    {
        timerLabel.text = SwaggySnapshotsGameManager.RoundTime.ToString("0");
    }
    
    /// <summary>
    /// Displays the timer by enabling the corresponding UI element.
    /// </summary>
    public void ShowTimer() => timer.SetActive(true);
    
    /// <summary>
    /// Hides the timer by disabling the corresponding UI element.
    /// </summary>
    public void HideTimer() => timer.SetActive(false);
}
