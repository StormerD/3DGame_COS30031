using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public EnemyData unitData;

    // logic to keep track of attack instances
    private bool _attackReady = true;
    private bool _attackRequested = false;
    private float _nextAttackTime = 0f; // time next attack is allowed

    void Update()
    {
        // 
        if (Time.time > _nextAttackTime && !_attackReady)
        {
            _attackReady = true;
        }
    }

    void FixedUpdate()
    {
        if (_attackRequested) AttackPlayer();
    }

    public void TryAttackPlayer()
    {
        Debug.Log(gameObject.name + " is trying to attack");
        // check if attack is on cooldown
        if (!_attackReady) return; // don't attack if on cooldown
        _attackReady = false;
        _nextAttackTime = Time.time + 1f / unitData.attacksPerSec; // set time of next allowed attack
        _attackRequested = true;

    }

    private void AttackPlayer()
    {
        if (!_attackRequested) return;
        _attackRequested = false;
        RaycastHit2D rayHit = new RaycastHit2D();
        rayHit = Physics2D.Raycast(transform.position, transform.up, unitData.attackRange, LayerMask.GetMask("Player")); // send raycast in the direction the enemy is facing and detect for player
        Debug.DrawRay(transform.position, transform.up * unitData.attackRange, Color.red, 2f);
        if (rayHit.collider != null && rayHit.collider.TryGetComponent<IHealth>(out var playerHealth))
        {
            playerHealth.TakeDamage(unitData.damage);
        }

    }
}
