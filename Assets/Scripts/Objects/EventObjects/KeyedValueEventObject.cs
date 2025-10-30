using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "KeyedValueEventObject", menuName = "EventObjects/Keyed float event")]
public class KeyedValueEventObject : ScriptableObject
{
    private event UnityAction<string, float> OnEventRaised;

    public void RaiseEvent(string key, float to) => OnEventRaised?.Invoke(key, to);

    public void RegisterListener(UnityAction<string, float> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<string, float> listener) => OnEventRaised -= listener;
}
