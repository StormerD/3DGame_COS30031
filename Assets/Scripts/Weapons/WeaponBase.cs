using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
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

    private float _nextBasicAttackTime = 0;
    private bool _basicReady = true;
    protected bool _doBasicAttack = false; // flag to run attack physics from FixedUpdate()
    private float _nextSecondaryAttackTime = 0;
    private bool _secondaryReady = true;
    protected bool _doSecondaryAttack = false;

    protected Vector3 _attackingDirection = Vector2.one;

    void Update()
    {
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
        if (_doSecondaryAttack) SecondaryPhysics();
    }

    public void Attack(Vector2 clickScreenPosition)
    {
        // if weapon is not ready to be used, emit an event for UI to possibly listen to; can do some sound / visual to show it's not ready
        if (!_basicReady)
        {
            AudioManager.Instance.PlayAudioClip(basicNotReady);
            OnBasicUsedNotReady?.Invoke();
            return;
        }
        _attackingDirection = GetAttackDirection(clickScreenPosition);
        _nextBasicAttackTime = Time.time + 1.0f / weaponData.basicAttacksPerSecond; // update next basic attack
        _basicReady = false;
        _doBasicAttack = true;
        OnBasicUsedReady?.Invoke(_attackingDirection);
        AudioManager.Instance.PlayAudioClip(basicAttack);
    }

    public void Secondary(Vector2 clickScreenPosition)
    {
        if (!_secondaryReady)
        {
            AudioManager.Instance.PlayAudioClip(secondaryNotReady);
            OnSecondaryUsedNotReady?.Invoke();
            return;
        }
        _attackingDirection = GetAttackDirection(clickScreenPosition);
        _nextSecondaryAttackTime = Time.time + weaponData.secondaryAttackCooldownSeconds;
        _secondaryReady = false;
        _doSecondaryAttack = true;
        OnSecondaryUsedReady?.Invoke(_attackingDirection);
        AudioManager.Instance.PlayAudioClip(secondaryAttack);
    }

    public WeaponData GetWeaponData() => weaponData;

    protected abstract void AttackPhysics();
    protected abstract void SecondaryPhysics();
    protected abstract Vector3 GetAttackDirection(Vector2 clickScreenPosition);
}