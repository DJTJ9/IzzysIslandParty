using enums;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class FishDisplay : MonoBehaviour
{
    private const string fishingActionMap = "FishingGame";
    private const string uiActionMap = "FishingGameUI";

    [Header("UI")]
    [SerializeField] private GameObject fishUIRenderer;

    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI fishSizeText;
    [SerializeField] private TextMeshProUGUI fishWeightText;
    [SerializeField] private GameObject fishDisplayPanel;
    [SerializeField] private GameObject stopFishDisplayButton;
    
    [Header("Input")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private EventSystem eventSystem;
    
    private void Awake()
    {
        if (fishUIRenderer == null)
            Debug.LogError("FishUIRenderer is null");
        else
            ClearDisplayParentObject();
         
        if (playerInput == null)
            Debug.LogError("inputActionAsset is null");
        
        if  (eventSystem == null)
            Debug.LogError("eventSystem is null");
    }

    private void Start()
    {
        if (fishDisplayPanel == null)
            Debug.LogError("No FishDisplayPanel found");
        else
            fishDisplayPanel.SetActive(false);
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

    public void DisplayFish(SO_Fish _fish)
    {
        playerInput.SwitchCurrentActionMap(uiActionMap);
        
        Time.timeScale = 0f;
        
        _fish.PrefabReference.SetActive(true);
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
        playerInput.SwitchCurrentActionMap(fishingActionMap);
        
        fishDisplayPanel.SetActive(false);
        
        Time.timeScale = 1f;

        foreach (Transform child in fishUIRenderer.transform)
            child.gameObject.SetActive(false);
    }

    private void ClearDisplayParentObject()
    {
        int childCount = fishUIRenderer.transform.childCount;

        for (int i = childCount; i > 0; i--)
            DestroyImmediate(fishUIRenderer.transform.GetChild(i - 1).gameObject);
    }
}