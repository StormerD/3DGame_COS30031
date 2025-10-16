using UnityEngine;

public class BasicRanged : Weapon2D
{
    [Header("Attack configuration")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float basicAttackProjectileSpeed;
    public float secondaryAttackDistance; // secondary attack is a beam
    public float secondaryAttackWidth;
    public float secondaryAttackKnockbackForce = 5.0f;
    public int maxSecondaryHits = 20;

    private Collider2D[] _secondaryHits;
    private ContactFilter2D _secondaryContactFilter;

    void Awake()
    {
        if (projectileSpawnPoint == null) Debug.LogWarning("set projectile spawn!");
        if (projectilePrefab == null) Debug.LogWarning("Set projectile prefab!");
        if (!projectilePrefab.TryGetComponent<IProjectile>(out _)) Debug.LogWarning("Projectile prefab needs an iprojectile script");
        _secondaryHits = new Collider2D[maxSecondaryHits];
        _secondaryContactFilter = new();
        _secondaryContactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
    }

    protected override void AttackPhysics()
    {
        if (!_doBasicAttack) return;
        _doBasicAttack = false;

        projectileSpawnPoint.localPosition = _attackingDirection.normalized;
        BasicRangedProjectile proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(Vector3.forward, _attackingDirection)).GetComponent<BasicRangedProjectile>();
        proj.SetDamage(weaponData.basicAttackDamage);
        proj.SetSpeed(basicAttackProjectileSpeed);
    }

    protected override void SecondaryPhysics()
    {
        _doSecondaryAttack = false;
        Vector3 normalizedDir = _attackingDirection.normalized;
        int hits = Physics2D.OverlapBox(transform.position + normalizedDir * (secondaryAttackDistance / 2), new Vector2(secondaryAttackDistance, secondaryAttackWidth), Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg, _secondaryContactFilter, _secondaryHits);

        for(int i = 0; i < hits; i++)
        {
            if (_secondaryHits[i].TryGetComponent<IHealth>(out var h))
            {
                h.TakeDamage(weaponData.secondaryAttackDamage);
            }
        }

        // debug draw a box to show bounds
        Vector3 endOfBox = normalizedDir * secondaryAttackDistance;
        Vector3 perpDist = Vector2.Perpendicular(normalizedDir) * (secondaryAttackWidth / 2);
        Debug.DrawLine(transform.position, transform.position + endOfBox, Color.green, 2f);
        Debug.DrawLine(transform.position + perpDist, transform.position - perpDist, Color.green, 2f);
        Debug.DrawLine(transform.position + endOfBox + perpDist, transform.position + endOfBox - perpDist, Color.green, 2f);
    }
}
