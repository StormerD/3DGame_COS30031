using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Ranged3DSecondary : MonoBehaviour, IProjectile
{
    public event Action OnCompleteFlight;
    public MeshRenderer visuals;

    private int _damage = 1;
    private Rigidbody _rb;
    private float _returnTime;
    private float _decelerationStartTime;
    private Vector3 _startVelocity = Vector3.zero;
    private float _forceDestroyTime = Mathf.Infinity;
    private Transform _weaponOrigin;
    private float _moveDirectlyBackSpeedThreshold;
    private float _flybackForce;
    private float _decelerationTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (visuals == null) Debug.LogError("Secondary prefab is missing visuals ref");
    }

    void FixedUpdate()
    {
        float t = Time.time;
        if (t > _forceDestroyTime) DelayedDestroy();

        if (t > _returnTime) // move back to weapon origin
        {
            Vector3 moveBackDirection = _weaponOrigin.position - transform.position;
            float sqDist = moveBackDirection.sqrMagnitude;
            float currentSpeed = _rb.linearVelocity.magnitude;
            moveBackDirection.Normalize();
            if (sqDist < 1f) DelayedDestroy(); // consider it 'returned' to the player
            else if (sqDist < 10f || currentSpeed > _moveDirectlyBackSpeedThreshold) // directly move it towards player
            {
                currentSpeed = Mathf.Max(currentSpeed, _flybackForce);
                _rb.linearVelocity = moveBackDirection * currentSpeed;
            }
            else
            {
                _rb.AddForce(moveBackDirection * _flybackForce, ForceMode.VelocityChange);
            }
        }
        else if (t > _decelerationStartTime) // deceleration period
        {
            if (_startVelocity == Vector3.zero) _startVelocity = _rb.linearVelocity;
            float ratio = (t - _decelerationStartTime) / _decelerationTime;
            Vector3 speedNow = Vector3.Lerp(_startVelocity, Vector3.zero, ratio);
            _rb.linearVelocity = speedNow;
        }
    }
    
    private void DelayedDestroy()
    {
        OnCompleteFlight?.Invoke();
        Destroy(gameObject, 0.1f);
        visuals.enabled = false; // this hidse the mesh render but keeps the trail 
        _rb.linearVelocity = Vector3.zero;
    }

    public void SetSpeed(float s) => _rb.linearVelocity = transform.forward * s;
    public void SetDamage(int d) => _damage = d;
    public void SetWeaponOrigin(Transform g) => _weaponOrigin = g;
    public void Initialize(int damage, float speed, float flybackForce, float movebackThresh, float timeBeforeReturn, float decelTime, Transform weaponOrig)
    {
        SetSpeed(speed); SetDamage(damage);
        _flybackForce = flybackForce;
        _moveDirectlyBackSpeedThreshold = movebackThresh;
        float spawnTime = Time.time;
        _returnTime = spawnTime + timeBeforeReturn;
        _decelerationStartTime = _returnTime - decelTime;
        _forceDestroyTime = spawnTime + timeBeforeReturn * 3;
        _weaponOrigin = weaponOrig;
        _decelerationTime = decelTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IHealth>(out var h))
        {
            h.TakeDamage(_damage);
        }
    }
}