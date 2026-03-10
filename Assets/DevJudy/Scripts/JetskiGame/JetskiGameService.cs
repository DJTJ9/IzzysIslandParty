using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace JetskiGame
{
    public class JetskiGameService : MonoBehaviour
    {
        [SerializeField] private InputActionMap actionMap;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private float secondsToLevelStart = 3f;
        [SerializeField] private UnityEvent onLevelStartEvent;

        private void Start()
        {
            // !! Might also have to stop a few things happening in start... like pathFinding
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            // Disable controls
            actionMap.Disable();

            for (int i = 0; i < secondsToLevelStart; i++)
            {
                countdownText.text = i.ToString() + "...";

                yield return new WaitForSeconds(1f);
            }

            actionMap.Enable();

            // Start levelTimer and tell NPCs to move
            onLevelStartEvent.Invoke();

            yield return null;
        }
    }
}