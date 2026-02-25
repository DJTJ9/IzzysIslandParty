using UnityEngine;

namespace JetskiGame.UI
{
    public class JetskiGameUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject timePenaltyObj;

        public void OnPlayerJoined(bool _pvp)
        {
            if (_pvp)
                timePenaltyObj.SetActive(false);
            else
                timePenaltyObj.SetActive(true);
        }
    }
}