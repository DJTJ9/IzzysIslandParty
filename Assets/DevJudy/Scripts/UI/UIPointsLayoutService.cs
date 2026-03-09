using UnityEngine;

namespace UIScripts
{
    public class UIPointsLayoutService : MonoBehaviour
    {
        [SerializeField] private RectTransform pointsObjectRT;
        [SerializeField] private RectTransform pointsObjRTEvenPlayers;

        public void SetPointsButtonRect(int _playerIndex)
        {
            if (_playerIndex == 1 || _playerIndex == 3)
            {
                Debug.Log("Changing anchored pos");
                pointsObjectRT.anchoredPosition = pointsObjRTEvenPlayers.anchoredPosition;
            }
        }
    }
}
