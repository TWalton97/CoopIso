using UnityEngine;
using System;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using System.Linq;

public class PlayerInputController : MonoBehaviour  //WE CAN USE PlayerInputActions.IPlayerActions <- the name of our input asset
{
    public InputActionAsset playerControls;
    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction basicAttackAction;
    private InputAction ability1Action;
    private InputAction blockAction;


    //Subscribe to these events
    public Action OnJumpPerformed;
    public Action<Vector2> OnMovePerformed;
    public Action<Vector2> OnMouseMoved;
    public Action<Vector2> OnStickMoved;
    public Action OnBasicAttackPerformed;
    public Action OnAbility1Performed;
    public Action<bool> OnBlockPerformed;

    private void Awake()
    {
        var gameplayMap = playerControls.FindActionMap("Player");

        moveAction = gameplayMap.FindAction("Move");

        jumpAction = gameplayMap.FindAction("Jump");
        jumpAction.performed += OnJump;

        lookAction = gameplayMap.FindAction("Look");
        lookAction.performed += OnLook;

        basicAttackAction = gameplayMap.FindAction("Attack");
        basicAttackAction.performed += OnAttack;

        ability1Action = gameplayMap.FindAction("Ability1");
        ability1Action.performed += OnAbility1;

        blockAction = gameplayMap.FindAction("Block");
        blockAction.performed += OnBlock;
        blockAction.canceled += OnBlockEnd;
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
        basicAttackAction.performed -= OnAttack;
        ability1Action.performed -= OnAbility1;
        blockAction.started -= OnBlock;
    }

    private void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        OnMovePerformed?.Invoke(moveValue);

        Vector2 mousePos = lookAction.ReadValue<Vector2>();
        OnMouseMoved?.Invoke(mousePos);
    }

    public void OnLook(CallbackContext context)
    {
        InputDevice device = context.control.device;

        if (device is Keyboard)
        {
            OnMouseMoved?.Invoke(context.ReadValue<Vector2>());
        }
        else if (device is Gamepad)
        {
            OnStickMoved?.Invoke(context.ReadValue<Vector2>());
        }
    }

    public void OnJump(CallbackContext context)
    {
        if (context.performed) OnJumpPerformed?.Invoke();
    }

    public void OnAttack(CallbackContext context)
    {
        if (context.performed) OnBasicAttackPerformed?.Invoke();
    }

    public void OnAbility1(CallbackContext context)
    {
        if (context.performed) OnAbility1Performed?.Invoke();
    }

    public void OnBlock(CallbackContext context)
    {
        OnBlockPerformed?.Invoke(true);
    }

    public void OnBlockEnd(CallbackContext context)
    {
        OnBlockPerformed?.Invoke(false);
    }
}
