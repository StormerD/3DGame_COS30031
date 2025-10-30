using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private string mixerVolumeChannelParameter;
    [SerializeField] private KeyedValueEventObject volumeChangeStream;
    [SerializeField] private KeyedValueEventObject setInitialVolumeValuesStream;
    [SerializeField] private BasicEventObject requestInitialVolumeValuesStream;

    private Slider slider;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }
    void OnEnable()
    {
        setInitialVolumeValuesStream.RegisterListener(SomeVolumeChanged);
    }
    void OnDisable()
    {
        setInitialVolumeValuesStream.UnregisterListener(SomeVolumeChanged);
    }
    void Start()
    {
        requestInitialVolumeValuesStream.RaiseEvent();
    }

    private void SomeVolumeChanged(string which, float to)
    {
        if (which != mixerVolumeChannelParameter) return;
        else slider.value = to;
    }

    public void OnVolumeChanged(float vol)
    {
        volumeChangeStream.RaiseEvent(mixerVolumeChannelParameter, vol);
    }
}