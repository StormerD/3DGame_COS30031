using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntEventObject", menuName = "EventObjects/Int Event")]
public class IntEventObject : ScriptableObject
{
    private event UnityAction<int> OnEventRaised;

    public void RaiseEvent(int integer) => OnEventRaised?.Invoke(integer);

    public void RegisterListener(UnityAction<int> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<int> listener) => OnEventRaised -= listener;
}
