using UnityEngine;

public class GenericGameEvent<T> : ScriptableObject {
    // private readonly List<GenericGameEventListener<T>> listeners = new List<GenericGameEventListener<T>>();
    //
    // public void Raise(T param) {
    //     for (var i = listeners.Count - 1; i >= 0; i--) listeners[i].OnEventRaised(param);
    // }
    //
    // public void RegisterListener(GenericGameEventListener<T> listener) {
    //     listeners.Add(listener);
    // }
    //
    // public void UnregisterListener(GenericGameEventListener<T> listener) {
    //     listeners.Remove(listener);
    // }
}
