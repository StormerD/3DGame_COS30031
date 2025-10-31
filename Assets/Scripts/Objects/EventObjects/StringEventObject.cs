using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StringEventObject", menuName = "EventObjects/String Event")]
public class StringEventObject : ScriptableObject
{
    private event UnityAction<string> OnEventRaised;

    public void RaiseEvent(string s) => OnEventRaised?.Invoke(s);

    public void RegisterListener(UnityAction<string> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<string> listener) => OnEventRaised -= listener;
}

