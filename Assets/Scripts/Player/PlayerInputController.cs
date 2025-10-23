using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;

public class PlayerInputController : MonoBehaviour
{
    public InputActionAsset playerControls;
    private InputAction jumpAction;
    private InputAction moveAction;


    //Subscribe to these events
    public Action OnJumpPerformed;
    public Action<Vector2> OnMovePerformed;

    private void Awake()
    {
        var gameplayMap = playerControls.FindActionMap("Player");

        moveAction = gameplayMap.FindAction("Move");

        jumpAction = gameplayMap.FindAction("Jump");
        jumpAction.performed += OnJump;
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void OnDestroy()
    {
        jumpAction.performed -= OnJump;
    }

    private void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        OnMovePerformed?.Invoke(moveValue);
    }

    public void OnJump(CallbackContext context)
    {
        if (context.performed) OnJumpPerformed?.Invoke();
    }


}
