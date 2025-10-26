using System;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(LootContainer), typeof(Animator))]
public class Enemy : MonoBehaviour, IHealth
{
    public EnemyData unitData;
    public event Action OnDeath;
    private int _currentHealth;
    private bool _hitThisFrame = false;
    private bool _isDead = false;

    private IObjectPool<Enemy> _enemyPool;
    private Animator _animator;

    public void SetPool(IObjectPool<Enemy> pool)
    {
        _enemyPool = pool;
    }

    void OnEnable()
    {
        _isDead = false;
        _currentHealth = unitData.maxHealth;
        var attack = GetComponent<EnemyAttack>();
        if (attack != null) attack.InitializeEnemyAttack();
    }

    void OnDisable() => _isDead = true;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); // in case we forget to set the layer, should set it auto
    }

    void Start()
    {
        _currentHealth = unitData.maxHealth;
        _animator = GetComponent<Animator>();
        
        OnDeath += GetComponent<LootContainer>().DropLoot;
    }

    void FixedUpdate()
    {
        _hitThisFrame = false;
    }

    public void TakeDamage(int amount)
    {
        if (_hitThisFrame || _isDead) return; // only take damage once per frame
        Debug.Log("Ouch! " + gameObject.name + " hit for " + amount + ", current: " + _currentHealth);
        //_animator.SetTrigger("Hit");
        _hitThisFrame = true;
        _currentHealth -= amount;
        if (_currentHealth <= 0 && !_isDead)
        {
            Debug.Log(gameObject.name + " is dead!");
            OnDeath?.Invoke();
            _isDead = true;
            _enemyPool.Release(this);
        }
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        if (_currentHealth > unitData.maxHealth)
        {
            _currentHealth = unitData.maxHealth;
        }
    }

    public int GetCurrentHealth()
    {
        return _currentHealth;
    }

    public int GetMaxHealth()
    {
        return unitData.maxHealth;
    }
}