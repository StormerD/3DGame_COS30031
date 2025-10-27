using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicEventObject", menuName = "EventObjects/Basic Event")]
public class BasicEventObject : ScriptableObject
{
    private event Action OnEventRaised;

    public void RaiseEvent() => OnEventRaised?.Invoke();

    public void RegisterListener(Action listener) => OnEventRaised += listener;
    public void UnregisterListener(Action listener) => OnEventRaised -= listener;
}
