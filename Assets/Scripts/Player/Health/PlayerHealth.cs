using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHealth : MonoBehaviour, IHealth
{
    [Tooltip("The stream that this player will emit current health data on.")]
    [SerializeField] private IntEventObject _currentHealthStream;
    [Tooltip("The stream that this object will emit max health data on.")]
    [SerializeField] private IntEventObject _maxHealthStream;
    [SerializeField] private int _maxHealth;
    [Tooltip("The stream used to signify a player's death.")]
    [SerializeField] private BasicEventObject _playerDeathStream;

    private int _currentHealth;
    private bool _hitThisFrame = false;

    void Start()
    {
        _currentHealth = _maxHealth;
        EmitHealthStreams();
    }

    void FixedUpdate()
    {
        _hitThisFrame = false;
    }

    private void EmitHealthStreams()
    {
        _currentHealthStream.RaiseEvent(_currentHealth);
        _maxHealthStream.RaiseEvent(_maxHealth);
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

        EmitHealthStreams();
        Debug.Log("Player took : " + damageAmount + " damage! Current health: " + _currentHealth);

        if (_currentHealth <= 0)
        {
            _playerDeathStream.RaiseEvent();
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
        EmitHealthStreams();
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