using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyAttack : MonoBehaviour
{
    public EnemyData unitData;

    // logic to keep track of attack instances
    private bool _attackReady = true;
    private bool _attackRequested = false;
    private float _nextAttackTime = 0f; // time next attack is allowed

    [Header("Lunge")]
    [SerializeField] private AnimationCurve lungeCurve = null;
    [SerializeField] private LayerMask playerMask = 0;
    private Collider2D _hurtBox = null;

    private Rigidbody2D _rb;
    private bool _isLunging = false;
    private bool _damageWindow = false;
    private bool _hitThisLunge = false;

    void Awake()
    {
        InitializeEnemyAttack();
    }

    public void InitializeEnemyAttack()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_hurtBox == null) _hurtBox = GetComponent<Collider2D>();
        if (lungeCurve == null) lungeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        if (playerMask == 0) playerMask = LayerMask.GetMask("Player");
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        _attackReady = true;
        _attackRequested = false;
        _nextAttackTime = 0f;
        _isLunging = false;
        _damageWindow = false;
        _hitThisLunge = false;
    }

    void Update()
    {
        if (Time.time > _nextAttackTime && !_attackReady) _attackReady = true;
    }

    void FixedUpdate()
    {
        if (_attackRequested && !_isLunging) AttackPlayer();
    }

    public void TryAttackPlayer()
    {
        // Debug.Log(gameObject.name + " is trying to attack");
        // check if attack is on cooldown
        if (_isLunging || !_attackReady) return; // don't attack if on cooldown
        _attackReady = false;
        _attackRequested = true;

    }

    private void AttackPlayer()
    {
        _attackRequested = false;
        StartCoroutine(Lunge());
    }

    private IEnumerator Lunge()
    {
        Debug.Log("Enemy Lunging");
        _isLunging = true;
        _hitThisLunge = false;
        _damageWindow = true;

        Vector2 start = _rb.position;
        Vector2 dir = (Vector2)transform.up;
        Vector2 end = start + dir * unitData.lungeDistance;

        float t = 0f;
        while (t < unitData.lungeDuration)
        {
            t += Time.fixedDeltaTime;
            float p = Mathf.Clamp01(t / unitData.lungeDuration);
            float eased = lungeCurve.Evaluate(p);
            _rb.MovePosition(Vector2.Lerp(start, end, eased));
            yield return new WaitForFixedUpdate();
        }

        _damageWindow = false;
        _nextAttackTime = Time.time + unitData.timeBetweenAttacks; // set time of next allowed attack
        _isLunging = false;
        Debug.Log("Enemy Finished Lunge");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!_damageWindow || _hitThisLunge) return;
        if ((playerMask.value & (1 << collider.gameObject.layer)) == 0) return;
        if (collider.TryGetComponent<IHealth>(out var hp))
        {
            Debug.Log("Enemy Hit Player!");
            hp.TakeDamage(unitData.damage);
            _hitThisLunge = true;
        }
    }
}
