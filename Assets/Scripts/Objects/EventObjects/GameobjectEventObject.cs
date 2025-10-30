using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameobjectEventObject", menuName = "EventObjects/GameObject Event")]
public class GameobjectEventObject : ScriptableObject
{
    private event UnityAction<GameObject> OnEventRaised;

    public void RaiseEvent(GameObject tex) => OnEventRaised?.Invoke(tex);

    public void RegisterListener(UnityAction<GameObject> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<GameObject> listener) => OnEventRaised -= listener;
}
