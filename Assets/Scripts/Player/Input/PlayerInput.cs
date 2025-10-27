using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    public InputAction dash, interact, move, attack, secondary, pause, jump;
    [SerializeField] private BasicEventObject _playerDeathStream;
    [SerializeField] private IntEventObject _freezeStream;
    private FrameInput _inputActions;

    void Awake()
    {
        _inputActions = new FrameInput();
        dash = _inputActions.Player.Dash;
        interact = _inputActions.Player.Interact;
        move = _inputActions.Player.Move;
        attack = _inputActions.Player.Attack;
        secondary = _inputActions.Player.Ability2;
        pause = _inputActions.Player.Pause;
        jump = _inputActions.Player.Jump;
    }

    void OnEnable()
    {
        _inputActions.Enable();
        _playerDeathStream.RegisterListener(DisableSelectInput);
        _freezeStream.RegisterListener(FreezeEvent);
    }
    void OnDisable() 
    {
        _inputActions.Disable();
        _playerDeathStream.UnregisterListener(DisableSelectInput);
        _freezeStream.UnregisterListener(FreezeEvent);
    }

    void Start()
    {
        pause.performed += PauseWrapper;
    }
    
    private void FreezeEvent(int state)
    {
        if (state == 0) EnableSelectInput();
        else DisableSelectInput();
    }

    public void DisableSelectInput()
    {
        DisableMovement();
        DisableFighting();
        interact.Disable();
    }
    void DisableMovement() {
        dash.Disable();
        move.Disable();
        jump.Disable();
    }
    void DisableFighting() {
        attack.Disable();
        secondary.Disable();
    }

    void EnableSelectInput()
    {
        EnableMovement();
        EnableFighting();
        interact.Enable();
    }
    void EnableMovement()
    {
        dash.Enable();
        move.Enable();
        jump.Enable();
    }
    void EnableFighting()
    {
        attack.Enable();
        secondary.Enable();
    }

    void PauseWrapper(CallbackContext callbackContext)
    {
        if (PauseManager.instance != null) PauseManager.instance.Pause();
        else Debug.LogWarning("PauseManager null; cannot pause!");
    }
}
