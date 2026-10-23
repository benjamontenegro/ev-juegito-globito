using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour
{
    // CLASE DEL PLAYERINPUT
    public static PlayerInput PlayerInput; 

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    // public static bool RunIsHeld;
    
    private InputAction _moveAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        // PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.Player.Move;
        _jumpAction = PlayerInput.Player.Jump;
    }

    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        // ARRIA ABAJO IZQUIERDA Y DERECHA

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        // se entiende? xd
    }
}
