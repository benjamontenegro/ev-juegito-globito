using UnityEngine;
using UnityEngine.InputSystem;
// using System.Collections;
// using System.Collections.Generic;
// using System;

public class InputHandler : MonoBehaviour
{
    // CLASE DEL PLAYERINPUT
    public static PlayerInput PlInp; 

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        PlInp = new PlayerInput();

        _moveAction = PlInp.Player.Move;
        _jumpAction = PlInp.Player.Jump;
    }

    private void OnEnable() { PlInp.Player.Enable(); }
    private void OnDisable() { PlInp.Player.Disable(); }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        // ARRIA ABAJO IZQUIERDA Y DERECHA

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        // se entiende? xd
    }
}
