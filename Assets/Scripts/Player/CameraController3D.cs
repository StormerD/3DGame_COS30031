using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

// Credit to Faktory Studios for the camera controller tutorial: https://www.youtube.com/watch?v=o7O28SFGWS4
[RequireComponent(typeof(CinemachineCamera), typeof(CinemachineOrbitalFollow))]
public class CameraController3D : MonoBehaviour
{
    [SerializeField] private IntEventObject _freezeEvent;
    [SerializeField] private float _zoomSpeed = 1.5f;
    [SerializeField] private float _zoomLerpSpeed = 10;
    [SerializeField] private float _minDist = 2f;
    [SerializeField] private float _maxDist = 15f;
    [SerializeField] private PlayerInput playerInp;

    private CinemachineCamera _cam;
    private CinemachineOrbitalFollow _orbital;
    private Vector2 _scrollDelta;

    private float _targetZoom;
    private float _curZoom;
    private bool _canMoveCam = true;

    void Awake()
    {
        _cam = GetComponent<CinemachineCamera>();
        _orbital = GetComponent<CinemachineOrbitalFollow>();
    }
    void OnEnable()
    {
        _freezeEvent.RegisterListener(FreezeEvent);
    }
    void OnDisable()
    {
        _freezeEvent.UnregisterListener(FreezeEvent);
    }
    private void FreezeEvent(int state)
    {
        if (state != 0) FreezeCameraMovementAndEnableCursor();
        else UnfreezeCameraAndDisableCursor();
    }

    private void FreezeCameraMovementAndEnableCursor()
    {
        _canMoveCam = false;
        Cursor.lockState = CursorLockMode.None;
        _orbital.enabled = false;
    }
    private void UnfreezeCameraAndDisableCursor()
    {
        _canMoveCam = true;
        Cursor.lockState = CursorLockMode.Locked;
        _orbital.enabled = true;
    }

    void Start()
    {
        if (playerInp == null) Debug.LogWarning("Camera controller missing ref to player input; cannot scroll.");
        else
        {
            playerInp.mouseScroll.performed += HandleMouseScroll;
        }

        Cursor.lockState = CursorLockMode.Locked;

        _targetZoom = _curZoom = _orbital.Radius;
    }

    void HandleMouseScroll(CallbackContext ctx) => _scrollDelta = ctx.ReadValue<Vector2>();

    void Update()
    {
        if (!_canMoveCam) return;
        if (_scrollDelta.y != 0)
        {
            _targetZoom = Mathf.Clamp(_orbital.Radius - _scrollDelta.y * _zoomSpeed, _minDist, _maxDist);
            _scrollDelta = Vector2.zero;
        }

        _curZoom = Mathf.Lerp(_curZoom, _targetZoom, Time.deltaTime * _zoomLerpSpeed);
        _orbital.Radius = _curZoom;
    }
}
