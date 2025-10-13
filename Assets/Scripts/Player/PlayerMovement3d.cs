using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerMovement3D : MonoBehaviour, IMover3D
{
    public float playerSpeed;
    private Rigidbody _rb;
    private PlayerInput _inp;
    private bool _canMove = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inp = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canMove) return;

        Vector2 inp = _inp.move.ReadValue<Vector2>() * playerSpeed;
        Vector3 inpSwizzle = new(inp.x, _rb.linearVelocity.y, inp.y);
        _rb.linearVelocity = inpSwizzle;
    }

    public void FreezeActions() => _canMove = false;
    public void UnfreezeActions() => _canMove = true;
    public Vector3 GetCurrentDirection() => Vector3.zero;
}
