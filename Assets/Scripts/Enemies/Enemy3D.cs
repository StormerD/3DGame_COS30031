

using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Enemy3D : MonoBehaviour, IHealth
{
    public event Action OnDeath;

    private int health = 10;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); // in case we forget to set the layer, should set it auto
    }

    public int GetCurrentHealth()
    {
        throw new NotImplementedException();
    }

    public int GetMaxHealth()
    {
        throw new NotImplementedException();
    }

    public void Heal(int amount)
    {
        throw new NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
        Debug.Log("Ouch! " + gameObject.name + " hit for " + amount);
    }
}