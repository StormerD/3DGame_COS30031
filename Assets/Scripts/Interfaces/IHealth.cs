using System;
using UnityEngine;

public interface IHealth
{
    void TakeDamage(int amount);
    void Heal(int amount);
    int GetCurrentHealth();
    int GetMaxHealth();
    public event Action OnDeath;
}
