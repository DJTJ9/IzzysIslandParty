using UnityEngine;

public class GenericGameEventListener<T> : MonoBehaviour {
    // #region Serialized Fields
    //
    // [SerializeField] private GameEvent<T> gameEvent;
    // [SerializeField] protected UnityEvent<T> response;
    //
    // #endregion
    //
    // #region Event Functions
    //
    // protected virtual void OnEnable() {
    //     gameEvent?.RegisterListener(this);
    // }
    //
    // protected virtual void OnDisable() {
    //     gameEvent?.UnregisterListener(this);
    // }
    //
    // #endregion
    //
    // public void OnEventRaised(T param) {
    //     response.Invoke(param);
    // }
}
