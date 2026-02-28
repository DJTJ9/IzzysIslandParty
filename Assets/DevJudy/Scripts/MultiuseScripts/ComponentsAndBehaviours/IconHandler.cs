using enums;
using ImprovedTimers;
using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Juice
{
    public class IconHandler : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] targets;
        [SerializeField] private SO_Emotion[] emotions;
        [SerializeField] private float displayIconSeconds = 3f;
        private int currentEmotionIndex = -1;

        private CountdownTimer iconTimer;

        [Header("Camera")]
        [SerializeField] private bool rotateToCamera = true;

        [ShowIf("rotateToCamera")]
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            targets = new MeshRenderer[emotions.Length];

            if (!targets[0])
                targets[0] = GetComponentInChildren<MeshRenderer>();
            else
                Debug.LogWarning("No target GO set, please add 'CharacterIconPrefab' and set meshRenderer");
        }

        private void Start()
        {
            if (emotions == null)
                return;

            for (int i = 0; i < emotions.Length; i++)
            {
                if (targets[i] == null)
                {
                    targets[i] = new GameObject().AddComponent<MeshRenderer>();
                    var meshFilter = targets[0].gameObject.GetComponent<MeshFilter>();
                    
                    targets[i].gameObject.AddComponent<MeshFilter>().mesh = meshFilter.sharedMesh;
                    targets[i].transform.SetParent(this.transform);
                    
                    targets[i].transform.position = targets[0].transform.position;
                    targets[i].transform.rotation = targets[0].transform.rotation;
                    targets[i].transform.localScale = targets[0].transform.localScale;
                }

                targets[i].material = emotions[i].IconMaterial;
                targets[i].name = emotions[i].name;
            }

            iconTimer = new CountdownTimer(displayIconSeconds);

            iconTimer.OnTimerStart += EnableMeshRenderer;
            iconTimer.OnTimerStop += DisableMeshRenderer;

            for (int i = 0; i < emotions.Length; i++)
            {
                targets[i].gameObject.SetActive(false);
            }
        }

        private void EnableMeshRenderer()
        {
            if (!targets[currentEmotionIndex])
                return;

            if (rotateToCamera && mainCamera)
                targets[currentEmotionIndex].gameObject.transform.LookAt(mainCamera.transform.position);

            targets[currentEmotionIndex].gameObject.SetActive(true);
        }

        private void DisableMeshRenderer()
        {
            if (!targets[currentEmotionIndex].gameObject)
                return;
            
            targets[currentEmotionIndex].gameObject.SetActive(false);
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

        public void DisplayIcon(EEmotion _emotion)
        {
            Debug.Log( gameObject.transform.parent.parent.name + " Displaying icon " + _emotion);
            if (iconTimer.IsRunning)
                iconTimer.Stop();
            
            if (TryGetEmotion(_emotion))
                iconTimer.Start();
            else
                Debug.LogError($"Emotion {_emotion} not registered");
        }

        private bool TryGetEmotion(EEmotion _emotion)
        {
            currentEmotionIndex = -1;

            for (int i = 0; i < emotions.Length; i++)
            {
                if (emotions[i].Emotion == _emotion)
                {
                    currentEmotionIndex = i;
                    return true;
                }
            }

            return false;
        }

        private void FixedUpdate()
        {
            if (iconTimer.IsRunning)
                iconTimer.Tick(Time.deltaTime);
        }
    }
}