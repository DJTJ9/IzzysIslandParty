using enums;
using ImprovedTimers;
using ScriptableObjects;
using UnityEngine;

namespace Juice
{
    public class IconHandler : MonoBehaviour
    {
        [SerializeField] private MeshRenderer target;
        private GameObject targetObject => target.gameObject;
        
        [SerializeField] private SO_Emotion[] emotions;
        [SerializeField] private float displayIconSeconds = 3f;

        private CountdownTimer iconTimer;

        private void Awake()
        {
            if (targetObject == null)
                Debug.LogError("No target set, please add 'CharacterIconPrefab' and set meshRenderer");

            SetTargetRotation();
            
            iconTimer = new CountdownTimer(displayIconSeconds);
            iconTimer.OnTimerStop += () => { targetObject?.SetActive(false); };
            iconTimer.OnTimerStart += () => { targetObject?.SetActive(true); };
            
            targetObject.SetActive(false);
        }
        
        private void SetTargetRotation()
        {
            Camera mainCamera = Camera.main;
            targetObject.transform.rotation = Quaternion.LookRotation(-mainCamera.transform.up, -mainCamera.transform.forward);
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