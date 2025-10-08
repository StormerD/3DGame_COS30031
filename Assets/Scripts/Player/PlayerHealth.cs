using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHealth : MonoBehaviour, IHealth
{
    [SerializeField]
    private int _maxHealth;
    public event Action OnDeath; // can be used to trigger game over screen
    public event Action<int, int> ChangedHealth; // used for telling healthbar UI to update

    private int _currentHealth;
    private bool _hitThisFrame = false;

    void Start()
    {
        _currentHealth = _maxHealth;
        if (GameOverScreen.instance != null) OnDeath += GameOverScreen.instance.OpenMenu;
        OnDeath += GetComponent<PlayerInput>().DisableSelectInput; // Player cannot take movement actions when dead

        ChangedHealth?.Invoke(_currentHealth, _maxHealth); // update values for healthbar UI 
    }

    void FixedUpdate()
    {
        _hitThisFrame = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (_hitThisFrame) return; // only take damage once per fixed frame
        _currentHealth -= damageAmount; // apply damage
        _hitThisFrame = true; // flag player as already hit
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        ChangedHealth?.Invoke(_currentHealth, _maxHealth);
        Debug.Log("Player took : " + damageAmount + " danage! Current health: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke(); // invoke player death systems (gameover screen, pause enemies, disable player inputs, etc.)
            Debug.Log("## Player has died ##");
        }
    }

    // in case we add health potions to enemy drops or around the map
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount; // add health

        if (_currentHealth > _maxHealth) // don't want player gaining more health than the maximum
        {
            _currentHealth = _maxHealth;
        }
        ChangedHealth?.Invoke(_currentHealth, _maxHealth);
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }
}
