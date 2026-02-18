using UnityEngine;
using UnityEngine.Events;

public class GameManagerMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private SO_Placing placingSO;
    [SerializeField] private UnityEvent onGameStart;

    private void Start()
    {
        onGameStart.Invoke();
    }
}