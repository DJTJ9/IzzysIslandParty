using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class KeepButtonSelected : MonoBehaviour
{
    [SerializeField] private MultiplayerEventSystem eventSystem;
    
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Reselect);
    }

    private void Reselect()
    {
        StartCoroutine(ReselectNextFrame());
    }

    private IEnumerator ReselectNextFrame()
    {
        yield return null;
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
