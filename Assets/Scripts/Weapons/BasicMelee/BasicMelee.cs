using UnityEngine;

public class BasicMelee : Weapon2D
{

    [Header("Attack configuration")]
    public float basicAttackRange = 2.0f;
    public float attackWidthDegrees = 45f;
    public int basicAttackRaycastAmount = 5; // attack is done in a cone shape over +- attackWidthDegrees, will be split into this many raycasts
    public float secondaryAttackRadius = 3.0f; // secondary attack hits all enemies in a circle of this radius
    public int maxSecondaryHits = 30;

    private RaycastHit2D[] _basicHits;
    private Collider2D[] _secondaryHits;
    private ContactFilter2D _contactFilter;
    private int _enemyLayerMask;

    void Start()
    {
        _secondaryHits = new Collider2D[maxSecondaryHits];
        _enemyLayerMask = LayerMask.GetMask("Enemy");
        _contactFilter = new();
        _contactFilter.SetLayerMask(_enemyLayerMask);
        _basicHits = new RaycastHit2D[basicAttackRaycastAmount];
    }


    protected override void AttackPhysics()
    {
        if (!_doBasicAttack) return;
        _doBasicAttack = false;

        float angleStep = 0;
        if (basicAttackRaycastAmount > 1) angleStep = attackWidthDegrees * 2 / (basicAttackRaycastAmount - 1);
        float startAngle = -attackWidthDegrees;

        for (int i = 0; i < basicAttackRaycastAmount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 rayDir = Quaternion.Euler(0, 0, angle) * _attackingDirection;
            _basicHits[i] = Physics2D.Raycast(transform.position, rayDir, basicAttackRange, _enemyLayerMask);
            Debug.DrawRay(transform.position, rayDir * basicAttackRange, Color.red, 2f);
            if (_basicHits[i].collider != null)
            {
                if (_basicHits[i].collider.TryGetComponent<IHealth>(out var h))
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
        int hits = Physics2D.OverlapCircle(transform.position, secondaryAttackRadius, _contactFilter, _secondaryHits);
        for (int i = 0; i < hits; i++)
        {
            if (_secondaryHits[i].TryGetComponent<IHealth>(out var h))
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