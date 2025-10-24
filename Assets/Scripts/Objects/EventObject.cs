using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventObject", menuName = "Scriptable Objects/EventObject")]
public class EventObject : ScriptableObject
{
    private event Action onEventRaised;

    public void RaiseEvent() => onEventRaised?.Invoke();

    public void RegisterListener(Action listener) => onEventRaised += listener;
    public void UnregisterListener(Action listener) => onEventRaised -= listener;
}
