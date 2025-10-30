using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput), typeof(Collider))]
public class PlayerMovement3D : MonoBehaviour, IMover3D
{
    [SerializeField] private MovementStats3D movementStats;
    public float footstepInterval = 0.4f; // Time between footsteps
    private float footstepTimer = 0f;


    [SerializeField] private GameObject dustPoolPrefab;
    private ParticleSystemPool _dustPool;
    [SerializeField] private GameObject dashPoolPrefab;
    private ParticleSystemPool _dashPool;

    private Rigidbody _rb;
    private PlayerInput _inp;
    private Vector3 _currentVelocity = Vector3.zero;
    private bool _canDash = true;
    private bool _canMove = true;
    private float _dashTimeStamp;
    private float _distanceToGround;
    private Quaternion lastRotation;
    private Transform cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inp = GetComponent<PlayerInput>();
        _inp.dash.performed += Dash;
        _inp.jump.performed += Jump;
        _distanceToGround = GetComponent<Collider>().bounds.extents.y;
        cam = Camera.main.transform;

        _dustPool = Instantiate(dustPoolPrefab).GetComponent<ParticleSystemPool>();
        _dashPool = Instantiate(dashPoolPrefab).GetComponent<ParticleSystemPool>();
    }

    void Update()
    {
        // Debug.Log("Update is running");
        if (!_canMove) return;

        Vector2 inp = _inp.move.ReadValue<Vector2>();
        bool isMoving = inp != Vector2.zero;
        bool isGrounded = IsGrounded();

        if (isGrounded && isMoving)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                AudioManager.Instance.PlayFootstep();
                Vector3 spawnPos = transform.position + new Vector3(0, -0.5f, 0); // adjust Y for foot level
                _dustPool.PlayParticle(spawnPos);

                footstepTimer = 0f;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_canMove) return;

        Vector2 inp = _inp.move.ReadValue<Vector2>(); 
        Vector3 targetVelocity = Vector3.zero;

        if (inp != Vector2.zero)
        {
            //The next two lines ensure that checking the direction of the camera is not affected by looking up or down
            Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            
            Vector3 movementDirection = camForward * inp.y + camRight * inp.x;
            movementDirection.Normalize();

            lastRotation = transform.rotation;
            targetVelocity = movementDirection * movementStats.speed;
            transform.rotation = Quaternion.Lerp(lastRotation, Quaternion.LookRotation(movementDirection), Time.deltaTime*movementStats.directionLerpSpeed);
        } else {
            Vector3 decelerationForce = -_currentVelocity.normalized * movementStats.deceleration;
            Vector3 decelerationForceSwizzle = new(decelerationForce.x, 0, decelerationForce.z);
            _rb.AddForce(decelerationForceSwizzle, ForceMode.Acceleration);
        }

        Vector3 velocityChange = targetVelocity - _currentVelocity; // _currentVelocity is originally set to zero and is updating every loop
        Vector3 velocityForce = velocityChange * movementStats.acceleration;
        velocityForce.y = 0;

        _rb.AddForce(velocityForce, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > movementStats.maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * movementStats.maxSpeed;
            _rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);
        }

        _currentVelocity = _rb.linearVelocity;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
    }

    void Jump(CallbackContext ctx) => Jump();

    public void Jump()
    {
        if (IsGrounded())
        {
            _rb.AddForce(Vector3.up * movementStats.jumpForce, ForceMode.Impulse);
        }
    }

    void Dash(CallbackContext ctx) => Dash();

    public void Dash()
    {
        if (_canDash)
        {
            _rb.AddForce(GetCurrentDirection() * movementStats.dashForce, ForceMode.Impulse);

            _dashTimeStamp = Time.time;
            _canDash = false;
            _dashPool.PlayParticle(transform.position);
        } else if ((Time.time - _dashTimeStamp) > movementStats.dashCooldownSeconds) 
        {
            _canDash = true;
        }
    }

    public void FreezeActions() => _canMove = false;
    public void UnfreezeActions() => _canMove = true;
    public Vector3 GetCurrentDirection() => _currentVelocity.normalized;
}
