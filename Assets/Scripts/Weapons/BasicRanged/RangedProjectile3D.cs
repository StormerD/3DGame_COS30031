using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class RangedProjectile3D : MonoBehaviour, IProjectile
{
    public MeshRenderer visuals;
    [SerializeField] private int bouncesUntilDeath = 2;
    private float speed = 10f;
    private float lifetime = 3f;
    private int damage = 1;
    private float _spawnTime;
    private Rigidbody _rb;

    private int numHitsTaken = 0;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (visuals == null) Debug.LogError("Projectile prefab missing visuals ref");
    }

    void Start()
    {
        _spawnTime = Time.time;
        UpdateRigidbodySpeed();
    }

    void Update()
    {
        if (Time.time > _spawnTime + lifetime)
        {
            Destroy(gameObject, 3f);
            visuals.enabled = false;
        }
    }

    void UpdateRigidbodySpeed()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.forward * speed;
    }

    public void SetSpeed(float s)
    {
        speed = s;
        UpdateRigidbodySpeed();
    }
    public void SetDamage(int d)
    {
        damage = d;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<IHealth>(out var h))
        {
            h.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (numHitsTaken == bouncesUntilDeath) Destroy(gameObject);
        else numHitsTaken++;
    }
}