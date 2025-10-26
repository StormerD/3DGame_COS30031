using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private IntEventObject _playerCurrentHealthStream;
    [SerializeField] private IntEventObject _playerMaxHealthStream;
    [SerializeField] private Slider _slider;

    private int _lastRecievedCurrentHealth = 0;
    private int _lastRecievedMaxHealth = 0;

    void Awake()
    {
        if (_slider == null) Debug.LogWarning("Health UI does not have slider reference; assign in inspector!");
    }

    void OnEnable()
    {
        _playerCurrentHealthStream.RegisterListener(CurrentHealthChanged);
        _playerMaxHealthStream.RegisterListener(MaxHealthChanged);
    }

    void OnDisable()
    {
        _playerCurrentHealthStream.UnregisterListener(CurrentHealthChanged);
        _playerMaxHealthStream.UnregisterListener(MaxHealthChanged);
    }

    private void MaxHealthChanged(int to)
    {
        _lastRecievedMaxHealth = to;
        SetSliderValues();
    }

    private void CurrentHealthChanged(int to)
    {
        _lastRecievedCurrentHealth = to;
        SetSliderValues();
    }

    public void SetSliderValues()
    {        
        _slider.maxValue = _lastRecievedMaxHealth;
        _slider.value = _lastRecievedCurrentHealth;
    }
}
