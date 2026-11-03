using UnityEngine;
using UnityEngine.InputSystem;
// using System.Collections;
// using System.Collections.Generic;
// using System;

public class InputHandler : MonoBehaviour
{
    // CLASE DEL PLAYERINPUT
    public static PlayerInput PlInp; 

    // DIRECCIONAL
    public static Vector2 Movement;
    // INPUTS SALTOS
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    // INPUTS INFLARSE/ABILIDAD
    public static bool AbilityWasPressed;
    public static bool AbilityIsHeld;
    public static bool AbilityWasReleased;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _abilityAction;

    private void Awake()
    {
        PlInp = new PlayerInput();

        _moveAction = PlInp.Player.Move;
        _jumpAction = PlInp.Player.Jump;
        _abilityAction = PlInp.Player.Attack1;
    }

    private void OnEnable() { PlInp.Player.Enable(); }
    // ESTO SIEMPRE TIENE QUE IR CON ENABLE?????
    private void OnDisable() { PlInp.Player.Disable(); }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        // ARRIA ABAJO IZQUIERDA Y DERECHA

        JumpWasPressed  = _jumpAction.WasPressedThisFrame();
        JumpIsHeld      = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        AbilityWasPressed   = _abilityAction.WasPressedThisFrame();
        AbilityIsHeld       = _abilityAction.IsPressed();
        AbilityWasReleased  = _abilityAction.WasReleasedThisFrame();
        // se entiende? xd
    }
}
