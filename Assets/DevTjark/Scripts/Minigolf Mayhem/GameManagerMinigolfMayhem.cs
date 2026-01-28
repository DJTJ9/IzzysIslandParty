using UnityEngine;
using UnityEngine.Events;

public class GameManagerMinigolfMayhem : MonoBehaviour
{
    [SerializeField] private UnityEvent onGameStart;

    private void Start()
    {
        onGameStart.Invoke();
    }
}