using System;
using UnityEngine;

public class BasicRanged : MonoBehaviour, IWeapon
{
    public event Action OnBasicReady;
    public event Action<Vector2> OnBasicUsedReady;
    public event Action OnBasicUsedNotReady;
    public event Action OnSecondaryReady;
    public event Action<Vector2> OnSecondaryUsedReady;
    public event Action OnSecondaryUsedNotReady;

    public WeaponData weaponData;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float basicAttackProjectileSpeed;
    public float secondaryAttackDistance; // secondary attack is a beam
    public float secondaryAttackWidth;
    public float secondaryAttackKnockbackForce = 5.0f;

    private PlayerMovement _playerMovement;
    private float _nextBasicAttackTime = 0;
    private bool _basicReady = true;
    private bool _doBasicAttack = false;
    private float _nextSecondaryAttackTime = 0;
    private bool _secondaryReady = true;
    private bool _doSecondaryAttack = false;

    private Vector2 _attackingDirection = Vector2.one;

    void Start()
    {
        if (projectileSpawnPoint == null) Debug.LogWarning("set projectile spawn!");
        if (projectilePrefab == null) Debug.LogWarning("Set projectile prefab!");
        if (!projectilePrefab.TryGetComponent<IProjectile>(out _)) Debug.LogWarning("Projectile prefab needs an iprojectile script");
    }

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
        if (_attackingDirection == Vector2.zero) _attackingDirection = Vector2.one;
        _nextBasicAttackTime = Time.time + 1.0f / weaponData.basicAttacksPerSecond; // update next basic attack
        _basicReady = false;
        _doBasicAttack = true;
        OnBasicUsedReady?.Invoke(_attackingDirection);
    }

    private void AttackPhysics()
    {
        if (!_doBasicAttack) return;
        _doBasicAttack = false;

        Vector2 dir = _playerMovement.GetCurrentDirection();
        if (dir == Vector2.zero) dir = Vector2.up; // if player is not moving, just attack "up"
        projectileSpawnPoint.localPosition = dir.normalized;
        GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(Vector3.forward, dir));
    }

    public void Secondary()
    {
        if (!_secondaryReady)
        {
            OnSecondaryUsedNotReady?.Invoke();
            return;
        }
        _attackingDirection = _playerMovement?.GetCurrentDirection() ?? Vector2.zero;
        if (_attackingDirection == Vector2.zero) _attackingDirection = Vector2.one;
        _nextSecondaryAttackTime = Time.time + weaponData.secondaryAttackCooldownSeconds;
        _secondaryReady = false;
        _doSecondaryAttack = true;
        OnSecondaryUsedReady?.Invoke(_attackingDirection);
    }

    private void SecondaryAttackPhysics()
    {
        _doSecondaryAttack = false;
        Vector3 normalizedDir = _playerMovement.GetCurrentDirection().normalized;

        foreach (var c in Physics2D.OverlapBoxAll(transform.position + normalizedDir * (secondaryAttackDistance / 2), new Vector2(secondaryAttackDistance, secondaryAttackWidth), Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg, LayerMask.GetMask("Enemy")))
        {
            if (c.TryGetComponent<IHealth>(out var h))
            {
                h.TakeDamage(weaponData.secondaryAttackDamage);
            }
        }

        _playerMovement.AddImpulseForce(-1 * secondaryAttackKnockbackForce * normalizedDir);

        // debug draw a box to show bounds
        Vector3 endOfBox = (Vector3)_playerMovement.GetCurrentDirection().normalized * secondaryAttackDistance;
        Vector3 perpDist = Vector2.Perpendicular(_playerMovement.GetCurrentDirection().normalized) * (secondaryAttackWidth / 2);
        Debug.DrawLine(transform.position, transform.position + endOfBox, Color.green, 2f);
        Debug.DrawLine(transform.position + perpDist, transform.position - perpDist, Color.green, 2f);
        Debug.DrawLine(transform.position + endOfBox + perpDist, transform.position + endOfBox - perpDist, Color.green, 2f);
    }

    public WeaponData GetWeaponData() => weaponData;
}
