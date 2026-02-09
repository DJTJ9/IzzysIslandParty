using System.Collections.Generic;
using enums;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private GameObject stopFishDisplayButton;

        [Header("Rendering: ")]
        [SerializeField] private Vector3 fishUIRendererPosition = new Vector3(-0.56f, -50f, -42.87f);
        private GameObject fishUIRenderer;
        private GameObject fishUIRendererParent;
        private GameObject currentFishShown;

        [Header("Input: ")]
        [SerializeField] private PlayerInput playerInput;

        public void SetupFishDisplay(int _playerIndex)
        {
            if (playerInput == null)
                Debug.LogError("inputActionAsset is null");

            fishUIRendererParent = GameObject.FindGameObjectWithTag("FishUIRenderer");
            InstantiateFishUIRenderer(_playerIndex);

            if (fishDisplayPanel == null)
                Debug.LogError("No FishDisplayPanel found");
            else
                fishDisplayPanel.SetActive(false);
        }

        private void InstantiateFishUIRenderer(int _playerIndex)
        {
            var emptyObj = new GameObject();
            var instantiatedObj = Instantiate(emptyObj).gameObject;
            
            fishUIRenderer = instantiatedObj;
            fishUIRenderer.name = "FishUIRendererObjects";
            fishUIRenderer.transform.SetParent(fishUIRendererParent.transform);
            fishUIRenderer.transform.localPosition = fishUIRendererPosition; // Multiplay the transform (or an addition) with the playerIndex
            fishUIRenderer.transform.rotation = new Quaternion(0f, 90f, 0f, 0f);
            fishUIRenderer.layer = fishUIRendererParent.layer;
            
            DestroyImmediate(emptyObj, true);
        }

        public GameObject SpawnInFishPrefabs(SO_Fish _fish)
        {
            GameObject obj = Instantiate(_fish.Prefab, fishUIRenderer.transform);
            obj.layer = fishUIRenderer.gameObject.layer;
            obj.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            foreach (Transform child in obj.transform)
                child.gameObject.layer = obj.layer;

            obj.SetActive(false);

            return obj;
        }

        public void DisplayFish(SO_Fish _fish, int _playerIndex)
        {
            playerInput.SwitchCurrentActionMap(uiActionMap);

            Time.timeScale = 0f;

            currentFishShown = _fish.PrefabReferences[_playerIndex].gameObject;

            currentFishShown.SetActive(true);
            fishDisplayPanel.SetActive(true);

            fishNameText.text = _fish.FishName;

            fishSizeText.text = $"Size: {_fish.GetRandomFromRange(_fish.SizeRange)} {GeSizeMeasureUnit(_fish.FishType)}";
            fishWeightText.text = $"Weight: {_fish.GetRandomFromRange(_fish.WeightRange)} {GeWeightMeasureUnit(_fish.FishType)}";
        }

        private string GeSizeMeasureUnit(EFish _fishType)
        {
            if (_fishType == EFish.BluefinTuna)
                return "m";

            return "cm";
        }

        private string GeWeightMeasureUnit(EFish _fishType)
        {
            if (_fishType == EFish.BluefinTuna)
                return "kg";

            return "g";
        }

        public void StopDisplayFish()
        {
            if (playerInput.enabled)
                playerInput.SwitchCurrentActionMap(fishingActionMap);

            fishDisplayPanel.SetActive(false);
            
            Time.timeScale = 1f;
            
            currentFishShown.SetActive(false);
            currentFishShown = null;
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