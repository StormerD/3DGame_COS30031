using UnityEngine;

public class BasicMelee3D : Weapon3D
{
    [Header("Attack configuration")]
    public float basicAttackRange = 2.0f;
    public float attackWidthDegrees = 45f;
    public int basicAttackRaycastAmount = 5; // attack is done in a cone shape over +- attackWidthDegrees, will be split into this many raycasts
    public float secondaryAttackRadius = 3.0f; // secondary attack hits all enemies in a circle of this radius

    protected override void AttackPhysics()
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

    protected override void SecondaryPhysics()
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
}
