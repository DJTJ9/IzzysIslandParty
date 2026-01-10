using enums;
using ImprovedTimers;
using ScriptableObjects;
using UnityEngine;

namespace Juice
{
    public class IconHandler : MonoBehaviour
    {
        private static IconHandler instance;
        public static IconHandler Instance => instance;

        [SerializeField] private MeshRenderer target;
        private GameObject targetObject => target.gameObject;
        [SerializeField] private SO_Emotion[] emotions;
        [SerializeField] private float displayIconSeconds = 3f;

        private CountdownTimer iconTimer;

        private IconHandler()
        {
            instance = this;
        }

        private void Awake()
        {
            if (targetObject == null)
                Debug.LogError("No target set, please add 'CharacterIconPrefab' and set meshRenderer");

            iconTimer = new CountdownTimer(displayIconSeconds);
            iconTimer.OnTimerStop += () => { targetObject?.SetActive(false); };
            iconTimer.OnTimerStart += () => { targetObject?.SetActive(true); };
        }
        
        public void DisplayIcon(EEmotion _emotion)
        {
            if (TryGetEmotion(_emotion, out Material iconMaterial))
            {
                target.material = iconMaterial;
                iconTimer.Start();
            }
            else
                Debug.LogError($"Emotion {_emotion} not registered");
        }

        private bool TryGetEmotion(EEmotion _emotion, out Material _iconMaterial)
        {
            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i].Emotion == _emotion)
                {
                    _iconMaterial = emotions[i].IconMaterial;
                    return true;
                }
            }

            _iconMaterial = null;
            return false;
        }
    }
}