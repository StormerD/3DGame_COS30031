using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHealth : MonoBehaviour, IHealth
{
    [Tooltip("The stream that this player will emit current health data on.")]
    [SerializeField] private IntEventObject _currentHealthStream;
    [Tooltip("The stream that this object will emit max health data on.")]
    [SerializeField] private IntEventObject _maxHealthStream;

    [SerializeField] private HealthStat healthStats;
    [Tooltip("The stream used to signify a player's death.")]
    [SerializeField] private BasicEventObject _playerDeathStream;

    private int _currentHealth;
    private bool _hitThisFrame = false;

    void Awake()
    {
        _currentHealth = healthStats.startingHealth;
    }

    void Start()
    {
        EmitHealthStreams();
        SceneManager.activeSceneChanged += SceneChanged;
    }

    void FixedUpdate()
    {
        _hitThisFrame = false;
    }

    private void EmitHealthStreams()
    {
        _currentHealthStream.RaiseEvent(_currentHealth);
        _maxHealthStream.RaiseEvent(healthStats.maxHealth);
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

        if (_currentHealth <= 0)
        {
            _playerDeathStream.RaiseEvent();
            Debug.Log("## Player has died ##");
        }
    }

    private void SceneChanged(Scene _, Scene __) => FullHeal();

    public void FullHeal()
    {
        _currentHealth = healthStats.maxHealth;
    }

    // in case we add health potions to enemy drops or around the map
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount; // add health

        if (_currentHealth > healthStats.maxHealth) // don't want player gaining more health than the maximum
        {
            _currentHealth = healthStats.maxHealth;
        }
        EmitHealthStreams();
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return healthStats.maxHealth;
    }
}