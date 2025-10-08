using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BasicRangedProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private int bouncesUntilDeath = 2;
    private float speed = 10f;
    private float lifetime = 3f;
    private int damage = 1;
    private float _spawnTime;
    private Rigidbody2D _rb;

    private int numHitsTaken = 0;

    void Start()
    {
        _spawnTime = Time.time;
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = transform.up * speed;
    }

    void Update()
    {
        if (Time.time > _spawnTime + lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void SetSpeed(float s)
    {
        speed = s;
    }
    public void SetDamage(int d)
    {
        damage = d;
    }

    public void SetDirection(Vector2 d)
    {
        float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null || collision.collider == null)
        {
            Debug.LogWarning("Collided with an object that is already null?");
            return;
        }
        if (collision.collider.TryGetComponent<IHealth>(out var h))
            {
                h.TakeDamage(damage);
                Destroy(gameObject);
            }

        if (numHitsTaken == bouncesUntilDeath) Destroy(gameObject);
        else numHitsTaken++;
    }
}