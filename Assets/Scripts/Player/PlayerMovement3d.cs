using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMovement3d : MonoBehaviour, IMover3D
{
    public float maxSpeed = 30f;
    public float playerSpeed = 10f;
    public float acceleration = 10f;
    public float deceleration = 5f;
    public float dashSpeed = 15f;
    public float dashLength = 0.3f;
    public float dashCooldownSeconds = 1.5f;

    private UnityEngine.Rigidbody _rb;
    private PlayerInput _inp;
    private Vector3 currentVelocity = Vector3.zero;
    private bool _canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inp = GetComponent<PlayerInput>();
        Debug.Log(_rb.GetType());
        Debug.Log(typeof(Rigidbody).GetProperty("velocity"));
        Debug.Log(typeof(Rigidbody).GetProperty("velocity"));

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_canMove) return;

        Vector2 inp = _inp.move.ReadValue<Vector2>();
        Vector3 inpSwizzle = new(inp.x, _rb.velocity.y, inp.y);
        Vector3 targetVelocity = inpSwizzle * playerSpeed;
        Vector3 velocityChange = targetVelocity - currentVelocity;
        Vector3 velocityForce = velocityChange * acceleration;

        _rb.AddForce(velocityForce, ForceMode.Acceleration);

        if (inp == Vector2.zero)
        {
            Vector3 decelerationForce = -currentVelocity.normalized * deceleration;
            _rb.AddForce(decelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
        }

        currentVelocity = _rb.velocity;
    }

    public void FreezeActions() => _canMove = false;
    public void UnfreezeActions() => _canMove = true;
    public Vector3 GetCurrentDirection() => currentVelocity.normalized;
}
