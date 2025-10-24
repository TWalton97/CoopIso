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
    private InputAction lookAction;
    private InputAction basicAttackAction;
    private InputAction ability1Action;


    //Subscribe to these events
    public Action OnJumpPerformed;
    public Action<Vector2> OnMovePerformed;
    public Action<Vector2> OnMouseMoved;
    public Action OnBasicAttackPerformed;
    public Action OnAbility1Performed;

    private void Awake()
    {
        var gameplayMap = playerControls.FindActionMap("Player");

        moveAction = gameplayMap.FindAction("Move");

        jumpAction = gameplayMap.FindAction("Jump");
        jumpAction.performed += OnJump;

        lookAction = gameplayMap.FindAction("Look");

        basicAttackAction = gameplayMap.FindAction("Attack");
        basicAttackAction.performed += OnAttack;

        ability1Action = gameplayMap.FindAction("Ability1");
        ability1Action.performed += OnAbility1;
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
    }

    private void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        OnMovePerformed?.Invoke(moveValue);

        Vector2 mousePos = lookAction.ReadValue<Vector2>();
        OnMouseMoved?.Invoke(mousePos);

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


}
