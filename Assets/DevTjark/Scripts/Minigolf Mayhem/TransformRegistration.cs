using UnityEngine;

public class TransformRegistration : MonoBehaviour
{
    public string LocationKey;

    private void Awake()
    {
        if (string.IsNullOrEmpty(LocationKey)) return;
        
        LocationService.Instance.RegisterTransform(LocationKey, transform);
    }

    public void SetAndRegisterPlayerLocationKey(string _key)
    {
        LocationKey = _key;
        LocationService.Instance.RegisterTransform(LocationKey, transform);
        LocationService.Instance.RegisterPlayerTransform(LocationKey, transform);
    }
}
