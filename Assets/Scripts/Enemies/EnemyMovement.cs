using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public event Action OnAttack;

    public EnemyData unitData;
    
    private Rigidbody2D _rb;
    private Vector2 _targetDirection;
    private float _targetDistance;
    private Transform _player;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = FindFirstObjectByType<PlayerMovement2D>().transform;
        OnAttack += () => GetComponent<EnemyAttack>().TryAttackPlayer();
    }

    void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateToTarget();
        TryAttack();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        Vector2 enemyToPlayerVector = _player.position - transform.position;
        _targetDirection = enemyToPlayerVector.normalized;
        _targetDistance = enemyToPlayerVector.magnitude;
    }

    private void RotateToTarget()
    {
        if (_targetDirection == Vector2.zero)
        {
            return;
        }

        // ## attempts at making the enemy have a rotation speed and lag behind the player. resulted in spinning out of control
        float angle = Mathf.Atan2(_targetDirection.y, _targetDirection.x) * Mathf.Rad2Deg - 90f;
        float currentAngle = _rb.rotation;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, angle, unitData.rotationSpeed * Time.deltaTime);
        _rb.SetRotation(newAngle);

        // Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        // Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, unitData.rotationSpeed * Time.deltaTime);
        // _rb.SetRotation(rotation);
    }

    private void TryAttack()
    {
        if (_targetDistance < 2f)
        {
            OnAttack?.Invoke();
        }
    }

    private void SetVelocity()
    {
        if (_targetDirection == Vector2.zero)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _rb.linearVelocity = transform.up * unitData.moveSpeed;
    }
}
