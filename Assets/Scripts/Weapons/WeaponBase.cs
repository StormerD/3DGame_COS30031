using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public event Action OnBasicReady;
    public event Action<Vector2> OnBasicUsedReady;
    public event Action OnBasicUsedNotReady;
    public event Action OnSecondaryReady;
    public event Action<Vector2> OnSecondaryUsedReady;
    public event Action OnSecondaryUsedNotReady;
    [Header("Weapon cooldown and damage data")]
    public WeaponData weaponData;
    [Header("Weapon sounds")]
    public AudioClip basicAttack;
    public AudioClip basicNotReady;
    public AudioClip secondaryAttack;
    public AudioClip secondaryNotReady;

    private IMover2D _entityMovement;
    private float _nextBasicAttackTime = 0;
    private bool _basicReady = true;
    protected bool _doBasicAttack = false; // flag to run attack physics from FixedUpdate()
    private float _nextSecondaryAttackTime = 0;
    private bool _secondaryReady = true;
    protected bool _doSecondaryAttack = false;

    protected Vector2 _attackingDirection = Vector2.one;

    void Start()
    {
        _entityMovement = null;
    }

    void Update()
    {
        if (_entityMovement == null && !transform.parent.TryGetComponent(out _entityMovement))
        {
            Debug.LogWarning("Parent of weapon missing IMover script");
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

    public void LinkNewMover(IMover2D mover) => _entityMovement = mover;

    void FixedUpdate()
    {
        if (_doBasicAttack) AttackPhysics();
        if (_doSecondaryAttack) SecondaryPhysics();
    }

    public void Attack()
    {
        // if weapon is not ready to be used, emit an event for UI to possibly listen to; can do some sound / visual to show it's not ready
        if (!_basicReady)
        {
            AudioManager.Instance.PlayAudioClip(basicNotReady);
            OnBasicUsedNotReady?.Invoke();
            return;
        }
        Debug.Log("Getting attacking direction, currently have " + _attackingDirection);
        _attackingDirection = _entityMovement?.GetCurrentDirection() ?? Vector2.zero;
        Debug.Log("Now is " + _attackingDirection);
        if (_attackingDirection == Vector2.zero) _attackingDirection = Vector2.up;
        _nextBasicAttackTime = Time.time + 1.0f / weaponData.basicAttacksPerSecond; // update next basic attack
        _basicReady = false;
        _doBasicAttack = true;
        OnBasicUsedReady?.Invoke(_attackingDirection);
        AudioManager.Instance.PlayAudioClip(basicAttack);
    }

    public void Secondary()
    {
        if (!_secondaryReady)
        {
            AudioManager.Instance.PlayAudioClip(secondaryNotReady);
            OnSecondaryUsedNotReady?.Invoke();
            return;
        }
        _attackingDirection = _entityMovement?.GetCurrentDirection() ?? Vector2.zero;
        if (_attackingDirection == Vector2.zero) _attackingDirection = Vector2.up;
        _nextSecondaryAttackTime = Time.time + weaponData.secondaryAttackCooldownSeconds;
        _secondaryReady = false;
        _doSecondaryAttack = true;
        OnSecondaryUsedReady?.Invoke(_attackingDirection);
        AudioManager.Instance.PlayAudioClip(secondaryAttack);
    }

    public WeaponData GetWeaponData() => weaponData;

    protected abstract void AttackPhysics();
    protected abstract void SecondaryPhysics();
}