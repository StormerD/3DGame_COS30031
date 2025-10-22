using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour, IMover
{
    public float playerSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashLength = 0.3f;
    public float dashCooldownSeconds = 1.5f;
    public bool canMove = true;

    public GameObject dashParticlesPrefab;

    private Rigidbody2D _rb;
    private Vector2 _currentDirection = Vector2.zero;
    private float _canDashNext = 0;
    private float _dashStartedAt = 0;
    private Vector2 _dashDirection = Vector2.zero;
    private PlayerInput _inp;

    void Awake()
    {
        _inp = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _inp.dash.performed += Dash;
        PauseManager.instance.OnPause += FreezeActions;
        PauseManager.instance.OnUnpause += UnfreezeActions;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

        Vector2 inp = _inp.move.ReadValue<Vector2>();
        _rb.linearVelocity = playerSpeed * inp.normalized;

        // true while dash should be ongoing
        if (Time.time <= _dashStartedAt + dashLength)
        {
            _rb.linearVelocity = _dashDirection * dashSpeed;
        }

        // keep track of current direction for other scripts (like weapons) that depend on player direction
        // but only update when actually moving. this way if the player stops moving the last direction is saved
        _currentDirection = _rb.linearVelocity == Vector2.zero ? _currentDirection : _rb.linearVelocity.normalized;
    }

    void Dash(CallbackContext ctx) => Dash(false);

     void SpawnDashParticles()
    {
        if (dashParticlesPrefab != null)
        {
            // Instantiate at character's position, no rotation
            GameObject particles = Instantiate(dashParticlesPrefab, transform.position, Quaternion.identity);
            
            //rotate particles to face dash direction
            particles.transform.right = _dashDirection;
            
            Destroy(particles, 0.5f); // auto-destroy after effect duration
        }
    }

    public void Dash(bool ignoreCooldown)
    {
        float t = Time.time;
        if (t > _canDashNext || ignoreCooldown)
        {
            _canDashNext = t + dashCooldownSeconds;
            _dashStartedAt = t;
            _dashDirection = _rb.linearVelocity.normalized;

            SpawnDashParticles();
            AudioManager.Instance.PlayDashSound();
        }
    }

    public Vector2 GetCurrentDirection() => _currentDirection;

    public void FreezeActions()
    {
        canMove = false;
    }

    public void UnfreezeActions()
    {
        canMove = true;
    }
}
