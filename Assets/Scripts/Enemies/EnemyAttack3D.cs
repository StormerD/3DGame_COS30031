using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack3D : MonoBehaviour
{
    public EnemyData unitData;

    // logic to keep track of attack instances
    private bool _attackRequested = false;
    private float _nextAttackTime = 0f; // time next attack is allowed

    [Header("Lunge")]
    [SerializeField] private float lungeDuration = 0.1f;
    [SerializeField] private AnimationCurve lungeCurve = null;
    [SerializeField] private LayerMask playerMask = 0;
    private Collider _hurtBox = null;

    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Transform _player;
    private bool _isLunging = false;
    private bool _damageWindow = false;
    private bool _hitThisLunge = false;

    void Awake()
    {
        InitializeEnemyAttack();
    }

    public void InitializeEnemyAttack()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_hurtBox == null) _hurtBox = GetComponent<Collider>();
        if (lungeCurve == null) lungeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        if (playerMask == 0) playerMask = LayerMask.GetMask("Player");
        if (_player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) _player = p.transform;
        }
        if (_agent != null)
        {
            _agent.isStopped = false;
            _agent.updatePosition = true;
        }
        _hurtBox.isTrigger = true;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        _attackRequested = false;
        _nextAttackTime = 0f;
        _isLunging = false;
        _damageWindow = false;
        _hitThisLunge = false;
    }

    void FixedUpdate()
    {
        if (_attackRequested && !_isLunging) AttackPlayer();
    }

    public void TryAttackPlayer()
    {
        // Debug.Log(gameObject.name + " is trying to attack");
        // check if attack is on cooldown
        if (_isLunging || Time.time < _nextAttackTime) return; // don't attack if on cooldown
        _nextAttackTime = Time.time + unitData.timeBetweenAttacks;
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

        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.updatePosition = false;
        }

        Vector3 start = _rb.position;
        Vector3 dir = _player ? (_player.position - transform.position).normalized : transform.forward;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        Vector3 end = start + dir * unitData.attackRange;

        float t = 0f;
        while (t < lungeDuration)
        {
            t += Time.fixedDeltaTime;
            float p = Mathf.Clamp01(t / lungeDuration);
            float eased = lungeCurve.Evaluate(p);
            _rb.MovePosition(Vector3.Lerp(start, end, eased));
            yield return new WaitForFixedUpdate();
        }

        _damageWindow = false;
        _nextAttackTime = Time.time + unitData.timeBetweenAttacks; // set time of next allowed attack
        _isLunging = false;

        if (_agent != null)
        {
            _agent.isStopped = false;
            _agent.updatePosition = true;
            _agent.SetDestination(_player.position);
        }

        Debug.Log("Enemy Finished Lunge");
    }

    private void OnTriggerEnter(Collider collider)
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
