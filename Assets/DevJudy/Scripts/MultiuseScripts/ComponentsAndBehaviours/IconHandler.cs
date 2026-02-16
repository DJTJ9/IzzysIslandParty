using enums;
using ImprovedTimers;
using ScriptableObjects;
using UnityEngine;

namespace Juice
{
    public class IconHandler : MonoBehaviour
    {
        [SerializeField] private MeshRenderer target;

        [SerializeField] private SO_Emotion[] emotions;
        [SerializeField] private float displayIconSeconds = 3f;

        private CountdownTimer iconTimer;

        private void Awake()
        {
            if (!target)
                target = GetComponentInChildren<MeshRenderer>();

            if (!target.gameObject)
                Debug.LogWarning("No target GO set, please add 'CharacterIconPrefab' and set meshRenderer");
        }

        private void Start()
        {
            iconTimer = new CountdownTimer(displayIconSeconds);

            iconTimer.OnTimerStart += EnableMeshRenderer;
            iconTimer.OnTimerStop += DisableMeshRenderer;

            SetTargetRotation();

            target.gameObject.SetActive(false);
        }
        
        private void EnableMeshRenderer()
        {
            if (!target.gameObject)
                return;
            
            target.gameObject.SetActive(true);
        }

        private void DisableMeshRenderer()
        {
            if (!target.gameObject)
                return;
            
            target.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            iconTimer.OnTimerStart -= EnableMeshRenderer;
            iconTimer.OnTimerStop -= DisableMeshRenderer;
            
            iconTimer = null;
        }

        private void OnDestroy()
        {
            iconTimer = null;
        }

        private void SetTargetRotation()
        {
            Camera mainCamera = Camera.main;
            target.gameObject.transform.rotation = Quaternion.LookRotation(-mainCamera.transform.up, -mainCamera.transform.forward);
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

        private void FixedUpdate()
        {
            if (iconTimer.IsRunning)
                iconTimer.Tick(Time.deltaTime);
        }
    }
}