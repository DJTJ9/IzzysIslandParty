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
        // Change input map and set delta time to zero

        _fish.PrefabReference.SetActive(true);
        fishDisplayPanel.SetActive(true);

        fishNameText.text = _fish.FishName;

        // Die random ranges hier als consts haben oder den fishen mitgeben?
        fishSizeText.text = $"Size: {10}cm "; // + randomSize according to fish species
        fishWeightText.text = $"Weight: {0.2}kg"; // + randomWeight according to fish species
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