using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject player;

    void Awake()
    {
        if (player == null) Debug.LogWarning("Healthbar does not have a reference to the player.");
        if (!player.TryGetComponent(out PlayerHealth health)) Debug.LogWarning("Player does not have playerhealth.");
        health.ChangedHealth += SetHealth;
    }
    
    public void SetHealth(int current, int max)
    {
        if (_slider == null) return;
        
        _slider.maxValue = max;
        _slider.value = current;
    }
}
