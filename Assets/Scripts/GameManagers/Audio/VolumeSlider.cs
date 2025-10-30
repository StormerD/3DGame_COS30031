using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string mixerVolumeChannelParameter;
    [SerializeField] private KeyedValueEventObject volumeChangeStream;
    public void OnVolumeChanged(float vol)
    {
        volumeChangeStream.RaiseEvent(mixerVolumeChannelParameter, vol);
    }
}