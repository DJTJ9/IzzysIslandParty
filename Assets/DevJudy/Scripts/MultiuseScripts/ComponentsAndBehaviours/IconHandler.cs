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

        [SerializeField] private bool debug;

        private GameObject TargetObject()
        {
            if (debug)
                Debug.Log("Looking for target");

            if (this == null)
                Debug.Log("I am null somehow...");

            if (this.enabled == false)
            {
                enabled = true;
                Debug.Log("I was disabled");
            }

            if (this.gameObject.activeInHierarchy == false)
            {
                Debug.Log("My GO was disabled");
                gameObject.SetActive(true);
            }

            // Just in case
            if (target == null)
            {
                Debug.LogError("Target is null ");
                target = gameObject.GetComponentInChildren<MeshRenderer>();
            }

            return target.gameObject;
        }

        private void Awake()
        {
            if (TargetObject() == null)
                Debug.LogWarning("No target set, please add 'CharacterIconPrefab' and set meshRenderer");
        }

        private void OnDestroy()
        {
            iconTimer = null;
        }

        private void Start()
        {
            if (target == null)
                target = GetComponentInChildren<MeshRenderer>();

            iconTimer = new CountdownTimer(displayIconSeconds);

            if (debug)
                Debug.Log("i am here, in start " + gameObject.transform.parent.parent.parent.name);
            
            iconTimer.OnTimerStop += () => { TargetObject().SetActive(false); };
            iconTimer.OnTimerStart += () => { TargetObject().SetActive(true); };

            SetTargetRotation();

            TargetObject().SetActive(false);
        }

        private void SetTargetRotation()
        {
            Camera mainCamera = Camera.main;
            TargetObject().transform.rotation = Quaternion.LookRotation(-mainCamera.transform.up, -mainCamera.transform.forward);
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