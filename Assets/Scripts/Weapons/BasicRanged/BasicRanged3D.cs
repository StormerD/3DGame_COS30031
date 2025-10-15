using UnityEngine;

public class BasicRanged3D : Weapon3D
{
    [Header("weapon visuals")]
    public MeshRenderer visuals;

    [Header("Attack configuration")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float basicAttackProjectileSpeed;

    [Header("Secondary config")]
    public GameObject secondaryPrefab; 
    public float secondaryInitialSpeed = 23;
    public float secondaryTimeBeforeReturn = 0.9f;
    public float secondaryDecelerationTime = 0.3f;
    public float secondaryFlybackSpeed = 5f;
    [Tooltip("When the secondary's rigidbody speed reaches this threshold, it will stop using AddForce and will directly move towards the player")]
    public float secondaryMovebackSpeedThresh = 30f;
    private bool _secondaryOngoing;
    private bool _playedBasicNotReady = false;

    void Start()
    {
        if (projectileSpawnPoint == null) Debug.LogWarning("set projectile spawn!");
        if (projectilePrefab == null) Debug.LogWarning("Set projectile prefab!");
        if (!projectilePrefab.TryGetComponent<IProjectile>(out _)) Debug.LogWarning("Projectile prefab needs an iprojectile script");
        _subclassHandlesBasicSounds = true;
    }

    protected override void AttackPhysics()
    {
        if (!_doBasicAttack || _secondaryOngoing) 
        {
            if (!_playedBasicNotReady) // need this check otherwise when secondary is ongoing if you spam click basic it will blow up your headphones (trust me not fun :( )
            {
                AudioManager.Instance.PlayAudioClip(basicNotReady);
                _playedBasicNotReady = true;
            }
            return; 
        }
        _doBasicAttack = false;
        _playedBasicNotReady = false;

        AudioManager.Instance.PlayAudioClip(basicAttack);

        projectileSpawnPoint.localPosition = _attackingDirection;
        RangedProjectile3D proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(_attackingDirection, Vector3.up)).GetComponent<RangedProjectile3D>();
        proj.SetDamage(weaponData.basicAttackDamage);
        proj.SetSpeed(basicAttackProjectileSpeed);
    }

    protected override void SecondaryPhysics()
    {
        if (!_doSecondaryAttack) return;
        _doSecondaryAttack = false;
        _secondaryOngoing = true;
        visuals.enabled = false;

        projectileSpawnPoint.localPosition = _attackingDirection;
        Ranged3DSecondary proj = Instantiate(secondaryPrefab, projectileSpawnPoint.position, Quaternion.LookRotation(_attackingDirection, Vector3.up)).GetComponent<Ranged3DSecondary>();
        proj.Initialize(weaponData.secondaryAttackDamage, secondaryInitialSpeed, secondaryFlybackSpeed, secondaryMovebackSpeedThresh, secondaryTimeBeforeReturn, secondaryDecelerationTime, transform);
        proj.OnCompleteFlight += FinishSecondary;
    }

    private void FinishSecondary()
    {
        _secondaryOngoing = false;
        visuals.enabled = true;
    }
}
