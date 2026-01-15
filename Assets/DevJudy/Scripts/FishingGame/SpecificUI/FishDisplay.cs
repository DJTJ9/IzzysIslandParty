using enums;
using ScriptableObjects;
using TMPro;
using UnityEngine;


public class FishDisplay : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject fishUIRenderer;

    [SerializeField] private TextMeshProUGUI fishNameText;
    [SerializeField] private TextMeshProUGUI fishSizeText;
    [SerializeField] private TextMeshProUGUI fishWeightText;

    [SerializeField] private GameObject fishDisplayPanel;


    private void Awake()
    {
        if (fishUIRenderer == null)
            Debug.LogError("FishUIRenderer is null");
        else
            ClearDisplayParentObject();
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
        // Change input map and set delta time to zero !!

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
        fishDisplayPanel.SetActive(false);
    }

    private void ClearDisplayParentObject()
    {
        int childCount = fishUIRenderer.transform.childCount;

        for (int i = childCount; i > 0; i--)
            DestroyImmediate(fishUIRenderer.transform.GetChild(i - 1).gameObject);
    }
}