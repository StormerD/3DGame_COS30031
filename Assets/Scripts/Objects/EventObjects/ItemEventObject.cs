using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItemEventObject", menuName = "EventObjects/Item Event")]
public class ItemEventObject : ScriptableObject
{
    private event UnityAction<IItem> OnEventRaised;

    public void RaiseEvent(IItem tex) => OnEventRaised?.Invoke(tex);

    public void RegisterListener(UnityAction<IItem> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<IItem> listener) => OnEventRaised -= listener;
}
