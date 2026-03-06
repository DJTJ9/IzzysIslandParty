using System.Collections.Generic;
using enums;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FishingGame.Display
{
    public class FishDisplay : MonoBehaviour
    {
        private const string fishingActionMap = "FishingGame";
        private const string uiActionMap = "FishingGameUI";

        [Header("UI: ")]
        [SerializeField] private TextMeshProUGUI fishNameText;
        [SerializeField] private TextMeshProUGUI fishSizeText;
        [SerializeField] private TextMeshProUGUI fishWeightText;
        [SerializeField] private GameObject fishDisplayPanel;
        [SerializeField] private Image stopFishDisplayButton;

        [Header("Rendering: ")]
        [SerializeField] private RawImage fishDisplayImage;
        [SerializeField] private Camera renderTextureCamera;
        [SerializeField] private List<Vector3> fishUIRendererPositions;
        [SerializeField] private List<RenderTexture> renderTextures;
        private GameObject fishUIRenderer;
        private GameObject fishUIRendererParent;
        private GameObject currentFishShown;

        [Header("Input: ")]
        [SerializeField] private PlayerInput playerInput;

        public void SetupFishDisplay(int _playerIndex)
        {
            
            if (renderTextures.Count > _playerIndex)
            {
                fishDisplayImage.texture = renderTextures[_playerIndex];
                renderTextureCamera.targetTexture = renderTextures[_playerIndex];
            }
            
            fishUIRendererParent = GameObject.FindGameObjectWithTag("FishUIRenderer");
            InstantiateFishUIRenderer(_playerIndex);

            if (fishDisplayPanel == null)
                Debug.LogError("No FishDisplayPanel found");
            else
                fishDisplayPanel.SetActive(false);
        }

        public void SetFishDisplayCloseButton(Sprite _closeFishDisplayButton)
        {
            stopFishDisplayButton.sprite = _closeFishDisplayButton;
        }

        private void InstantiateFishUIRenderer(int _playerIndex)
        {
            var emptyObj = new GameObject();
            var instantiatedObj = Instantiate(emptyObj).gameObject;
            
            fishUIRenderer = instantiatedObj;
            fishUIRenderer.name = "FishUIRendererObjects";
            fishUIRenderer.transform.SetParent(fishUIRendererParent.transform);
            fishUIRenderer.transform.localPosition = fishUIRendererPositions[_playerIndex];
            fishUIRenderer.layer = fishUIRendererParent.layer;
            
            DestroyImmediate(emptyObj, true);
        }

        public GameObject SpawnInFishPrefabs(SO_Fish _fish)
        {
            GameObject obj = Instantiate(_fish.Prefab, fishUIRenderer.transform);
            obj.layer = fishUIRenderer.gameObject.layer;

            foreach (Transform child in obj.transform)
                child.gameObject.layer = obj.layer;

            obj.SetActive(false);

            return obj;
        }

        public void DisplayFish(SO_Fish _fish, int _playerIndex)
        {
            playerInput.SwitchCurrentActionMap(uiActionMap);

            currentFishShown = _fish.PrefabReferences[_playerIndex].gameObject;

            currentFishShown.SetActive(true);
            fishDisplayPanel.SetActive(true);

            fishNameText.text = _fish.FishName;

            fishSizeText.text = $"Size: {_fish.GetRandomFromRange(_fish.SizeRange)} {GetSizeMeasureUnit(_fish.FishType)}";
            fishWeightText.text = $"Weight: {_fish.GetRandomFromRange(_fish.WeightRange)} {GetWeightMeasureUnit(_fish.FishType)}";
        }

        private string GetSizeMeasureUnit(EFish _fishType)
        {
            if (_fishType == EFish.BluefinTuna)
                return "m";

            return "cm";
        }

        private string GetWeightMeasureUnit(EFish _fishType)
        {
            if (_fishType == EFish.BluefinTuna)
                return "kg";

            return "g";
        }

        public void StopDisplayFish()
        {
            fishDisplayPanel.SetActive(false);
            
            currentFishShown.SetActive(false);
            currentFishShown = null;
            
            if (playerInput.enabled)
                playerInput.SwitchCurrentActionMap(fishingActionMap);
        }

        public void ClearPrefabReferences(List<SO_Fish> _fishList)
        {
            for (int i = 0; i < _fishList.Count; i++)
            {
                _fishList[i].PrefabReferences.Clear();
            }
        }
    }
}