using UnityEngine;

public class BasicMelee3D : Weapon3D
{
    [Header("basic attack configuration")]
    public float basicAttackRange = 2.0f;
    public float attackWidthDegrees = 45f;
    public int basicAttackRaycastAmount = 3; // attack is done in a cone shape over +- attackWidthDegrees, will be split into this many raycasts
    public float sphereCastRadius = 0.2f;
    [Header("Secondary attack config")]
    public float secondaryAttackRadius = 3.0f; // secondary attack hits all enemies in a circle of this radius
    public int maxSecondaryHits = 100;

    private Collider[] _hitResult;
    private int _basicMask;
    private int _secondaryMask;


    void Awake()
    {
        _hitResult = new Collider[maxSecondaryHits];
        // basic can only hit enemies; won't to anything to enemy attacks
        _basicMask = 1 << LayerMask.NameToLayer("Enemy");
        // secondary can destroy both enemies and enemy attacks
        _secondaryMask = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("EnemyAttack");
    }

    protected override void AttackPhysics()
    {
        if (!_doBasicAttack) return;
        _doBasicAttack = false;

        Debug.Log("basci atack");

        float angleStep = 0;
        if (basicAttackRaycastAmount > 1) angleStep = attackWidthDegrees * 2 / (basicAttackRaycastAmount - 1);
        float startAngle = -attackWidthDegrees;
        
        for (int i = 0; i < basicAttackRaycastAmount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 rayDir = Quaternion.Euler(0, angle, 0) * _attackingDirection;
            Debug.DrawRay(transform.root.position, rayDir * basicAttackRange, Color.red, 2f);
            
            // using the root transform here as that will (should?) be the player
            if (Physics.SphereCast(transform.root.position, sphereCastRadius, rayDir, out RaycastHit hit, basicAttackRange, _basicMask))
            {
                if (hit.collider.TryGetComponent<IHealth>(out var h)) h.TakeDamage(weaponData.basicAttackDamage);
            }
        }
    }

    protected override void SecondaryPhysics()
    {
        _doSecondaryAttack = false;
        int enemiesInRange = Physics.OverlapSphereNonAlloc(transform.position, secondaryAttackRadius, _hitResult, _secondaryMask);
        for (int i = 0; i < enemiesInRange; i++)
        {
            Debug.Log("Hit: " + _hitResult[i].name + ", layer" + LayerMask.LayerToName(_hitResult[i].gameObject.layer));
            if (_hitResult[i].TryGetComponent<IHealth>(out var health))
            {
                health.TakeDamage(weaponData.secondaryAttackDamage);
            }
            else Debug.LogWarning("Hit does not have IHealth");
        }
    }
}
