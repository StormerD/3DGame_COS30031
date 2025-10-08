using System;
using UnityEngine;

public class BasicMelee : MonoBehaviour, IWeapon
{
    public event Action OnBasicReady;
    public event Action<Vector2> OnBasicUsedReady;
    public event Action OnBasicUsedNotReady;
    public event Action OnSecondaryReady;
    public event Action<Vector2> OnSecondaryUsedReady;
    public event Action OnSecondaryUsedNotReady;

    public WeaponData weaponData;
    public float basicAttackRange = 2.0f;
    public float attackWidthDegrees = 45f;
    public int basicAttackRaycastAmount = 5; // attack is done in a cone shape over +- attackWidthDegrees, will be split into this many raycasts
    public float secondaryAttackRadius = 3.0f; // secondary attack hits all enemies in a circle of this radius

    // in order to make this script fully separate from player data, this should be an "IMover" interface or something
    // but since I don't think we will have time to create crazy enemy AIs that use weapons, I omitted that interface.
    private PlayerMovement _playerMovement;
    private float _nextBasicAttackTime = 0;
    private bool _basicReady = true;
    private bool _doBasicAttack = false; // physics should be done in FixedUpdate not in attack(), set a flag to do it
    private float _nextSecondaryAttackTime = 0;
    private bool _secondaryReady = true;
    private bool _doSecondaryAttack = false;

    private Vector2 _attackingDirection = Vector2.one;

    void Update()
    {
        if (_playerMovement == null && !transform.parent.TryGetComponent<PlayerMovement>(out _playerMovement))
        {
            Debug.LogWarning("Parent of weapon BasicMelee is either not a Player or is a player missing a movement script");
            return;
        }

        if (Time.time > _nextBasicAttackTime && !_basicReady)
        {
            _basicReady = true;
            OnBasicReady?.Invoke();
        }
        if (Time.time > _nextSecondaryAttackTime && !_secondaryReady)
        {
            _secondaryReady = true;
            OnSecondaryReady?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (_doBasicAttack) AttackPhysics();
        if (_doSecondaryAttack) SecondaryAttackPhysics();
    }

    public void Attack()
    {
        // if weapon is not ready to be used, emit an event for UI to possibly listen to; can do some sound / visual to show it's not ready
        if (!_basicReady)
        {
            OnBasicUsedNotReady?.Invoke();
            return;
        }
        _attackingDirection = _playerMovement?.GetCurrentDirection() ?? Vector2.zero;
        if (_attackingDirection == Vector2.zero) _attackingDirection = Vector2.up;
        _nextBasicAttackTime = Time.time + 1.0f / weaponData.basicAttacksPerSecond; // update next basic attack
        _basicReady = false;
        _doBasicAttack = true;
        OnBasicUsedReady?.Invoke(_attackingDirection);
        AudioManager.Instance.PlayAttackSound();
    }

    private void AttackPhysics()
    {
        if (!_doBasicAttack) return;
        _doBasicAttack = false;

        float angleStep = 0;
        if (basicAttackRaycastAmount > 1) angleStep = attackWidthDegrees * 2 / (basicAttackRaycastAmount - 1);
        float startAngle = -attackWidthDegrees;

        RaycastHit2D[] hits = new RaycastHit2D[basicAttackRaycastAmount];
        for (int i = 0; i < basicAttackRaycastAmount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 rayDir = Quaternion.Euler(0, 0, angle) * _attackingDirection;
            hits[i] = Physics2D.Raycast(transform.position, rayDir, basicAttackRange, LayerMask.GetMask("Enemy"));
            Debug.DrawRay(transform.position, rayDir * basicAttackRange, Color.red, 2f);
            if (hits[i].collider != null)
            {
                if (hits[i].collider.TryGetComponent<IHealth>(out var h))
                {
                    // apply damage to enemy; it's okay if we hit the same enemy multiple times
                    // as they should track if they've been hit in a fixedUpdate frame and will not apply damage multiple times
                    // (however this does mean that if two player attacks like a basic and a secondary attack 
                    // hit the same one only one of those damages will proc -- not good!)
                    h.TakeDamage(weaponData.basicAttackDamage);
                }
            }
        }
    }

    public void Secondary()
    {
        if (!_secondaryReady)
        {
            OnSecondaryUsedNotReady?.Invoke();
            return;
        }
        _attackingDirection = _playerMovement?.GetCurrentDirection() ?? Vector2.zero;
        if (_attackingDirection == Vector2.zero) _attackingDirection = Vector2.up;
        _nextSecondaryAttackTime = Time.time + weaponData.secondaryAttackCooldownSeconds;
        _secondaryReady = false;
        _doSecondaryAttack = true;
        OnSecondaryUsedReady?.Invoke(_attackingDirection);
    }

    private void SecondaryAttackPhysics()
    {
        _doSecondaryAttack = false;
        foreach (var c in Physics2D.OverlapCircleAll(transform.position, secondaryAttackRadius, LayerMask.GetMask("Enemy")))
        {
            if (c.TryGetComponent<IHealth>(out var h))
            {
                h.TakeDamage(weaponData.secondaryAttackDamage);
            }
        }
        // draw debug circle range
        Debug.DrawRay(transform.position, Vector2.up * secondaryAttackRadius, Color.blue, 2f);
        Debug.DrawRay(transform.position, Vector2.right * secondaryAttackRadius, Color.blue, 2f);
        Debug.DrawRay(transform.position, Vector2.down * secondaryAttackRadius, Color.blue, 2f);
        Debug.DrawRay(transform.position, Vector2.left * secondaryAttackRadius, Color.blue, 2f);
    }

    public WeaponData GetWeaponData() => weaponData;
}