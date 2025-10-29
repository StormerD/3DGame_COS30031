using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TextureEventObject", menuName = "EventObjects/Texture Event")]
public class TextureEventObject : ScriptableObject
{
    private event UnityAction<Texture2D> OnEventRaised;

    public void RaiseEvent(Texture2D tex) => OnEventRaised?.Invoke(tex);

    public void RegisterListener(UnityAction<Texture2D> listener) => OnEventRaised += listener;
    public void UnregisterListener(UnityAction<Texture2D> listener) => OnEventRaised -= listener;
}
