using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    public InputAction dash, interact, move, attack, secondary, pause, jump;

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

    void Start()
    {
        if(PauseManager.instance != null)
        {
            pause.performed += PauseWrapper;
            PauseManager.instance.OnPause += DisableSelectInput; // disables everything except the "pause" action
            PauseManager.instance.OnUnpause += EnableSelectInput; // vice-versa
        }
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

    void PauseWrapper(CallbackContext callbackContext) => PauseManager.instance.Pause();
    void OnEnable() => _inputActions.Enable();
    void OnDisable() => _inputActions.Disable();
}
