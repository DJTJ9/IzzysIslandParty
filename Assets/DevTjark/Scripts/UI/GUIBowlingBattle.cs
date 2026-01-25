using UnityEngine;

public class GUIBowlingBattle : MonoBehaviour
{
    [SerializeField] private GameObject bowlingBattleUI;
    
    public void ShowUI() => bowlingBattleUI.SetActive(true);
    public void HideUI() => bowlingBattleUI.SetActive(false);
}
