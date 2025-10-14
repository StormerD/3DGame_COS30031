using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMovement3d : MonoBehaviour, IMover3D
{
    public float maxSpeed = 100f;
    public float playerSpeed = 10f;
    public float acceleration = 8f;
    public float deceleration = 5f;
    public float jumpForce = 50f;
    public float dashForce = 15f;
    public float dashCooldownSeconds = 1.5f;

    private UnityEngine.Rigidbody _rb;
    private PlayerInput _inp;
    private Vector3 _currentVelocity = Vector3.zero;
    private bool _canDash = true;
    private bool _canMove = true;
    private float _dashTimeStamp;
    private float _distanceToGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inp = GetComponent<PlayerInput>();
        _inp.dash.performed += Dash;
        _inp.jump.performed += Jump;
        _distanceToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_canMove) return;

        Vector2 inp = _inp.move.ReadValue<Vector2>(); 
        //Vector3 inpSwizzle = new(inp.x, _rb.velocity.y, inp.y);
        Vector3 targetVelocity = Vector3.zero;

        if (inp != Vector2.zero)
        {
            Transform cam = Camera.main.transform;
            //The next two lines ensure that checking the direction of the camera is not affected by looking up or down
            Vector3 camForward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            Vector3 camRight = Vector3.ProjectOnPlane(cam.right, Vector3.up).normalized;
            
            Vector3 movementDirection = camForward * inp.y + camRight * inp.x;
            movementDirection.Normalize();

            targetVelocity = movementDirection * playerSpeed;
        } else {
            Vector3 decelerationForce = -_currentVelocity.normalized * deceleration;
            Vector3 decelerationForceSwizzle = new(decelerationForce.x, 0, decelerationForce.z);
            _rb.AddForce(decelerationForceSwizzle, ForceMode.Acceleration);
        }

        Vector3 velocityChange = targetVelocity - _currentVelocity; // _currentVelocity is originally set to zero and is updating every loop
        Vector3 velocityForce = velocityChange * acceleration;

        _rb.AddForce(velocityForce, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
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
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Dash(CallbackContext ctx) => Dash();

    public void Dash()
    {
        if (_canDash)
        {
            _rb.AddForce(GetCurrentDirection() * dashForce, ForceMode.Impulse);

            _dashTimeStamp = Time.time;
            _canDash = false;
        } else if ((Time.time - _dashTimeStamp) > dashCooldownSeconds) 
        {
            _canDash = true;
        }
    }

    public void FreezeActions() => _canMove = false;
    public void UnfreezeActions() => _canMove = true;
    public Vector3 GetCurrentDirection() => _currentVelocity.normalized;
}
