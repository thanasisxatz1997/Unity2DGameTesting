using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    private PlayerInput inputActions;
    public event EventHandler OnPlayerJump;
    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerInteract;
    public event EventHandler OnPlayerRangedAttack;
    public event EventHandler OnPlayerDash;

    private void Awake()
    {
        inputActions = new PlayerInput();
    }
    private void Start()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed += Jump_performed;
        inputActions.Player.Attack.performed += Attack_performed;
        inputActions.Player.Interact.performed += Interact_performed;
        inputActions.Player.RangedAttack.performed += RangedAttack_performed;
        inputActions.Player.Dash.performed += Dash_performed;
    }

    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }

    private void RangedAttack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerRangedAttack?.Invoke(this,EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerInteract?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerJump?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMoveInputNormalized()
    {
        Vector2 moveInput = inputActions.Player.Movement.ReadValue<Vector2>();
        Debug.Log("MoveInput= " + moveInput);
        return moveInput.normalized;
    }
}
