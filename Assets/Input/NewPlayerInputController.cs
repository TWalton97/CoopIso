using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class NewPlayerInputController : MonoBehaviour
{
    public bool IsHoldingBlock { get; private set; }

    public Action OnJumpPerformed;
    public Action<Vector2> OnMovePerformed;
    public Action<Vector2> OnMouseMoved;
    public Action<Vector2> OnStickMoved;
    public Action OnBasicAttackPerformed;
    public Action OnAbility1Performed;
    public Action<bool> OnBlockPerformed;

    private PlayerInputActions _playerInputActions;
    private PlayerInput _playerInput;
    [SerializeField] private NewPlayerController _playerController;
    private const string KEYBOARD_SCHEME = "Keyboard&Mouse";
    private const string GAMEPAD_SCHEME = "Gamepad";

    public int PlayerIndex;


    void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInput = GetComponent<PlayerInput>();
        _playerController = GetComponent<NewPlayerController>();
    }

    void Start()
    {
        EnablePlayerInput();
    }

    void OnEnable()
    {
        _playerInput.onControlsChanged += SwitchBindings;

        //Single input actions
        _playerInputActions.Player.Jump.performed += OnJump;
        _playerInputActions.Player.Attack.performed += OnAttack;
        // _playerInputActions.Player.Ability1.performed += OnAbility1;
        _playerInputActions.Player.Block.performed += OnBlock;
    }

    void OnDisable()
    {
        _playerInput.onControlsChanged -= SwitchBindings;

        _playerInputActions.Player.Jump.performed -= OnJump;
        _playerInputActions.Player.Attack.performed -= OnAttack;
        // _playerInputActions.Player.Ability1.performed -= OnAbility1;
        _playerInputActions.Player.Block.performed -= OnBlock;
    }

    void Update()
    {
        OnMovePerformed?.Invoke(_playerInputActions.Player.Move.ReadValue<Vector2>());

        if (_playerInput.currentControlScheme == KEYBOARD_SCHEME)
        {
            OnMouseMoved?.Invoke(_playerInputActions.Player.LookMouse.ReadValue<Vector2>());
        }
        else if (_playerInput.currentControlScheme == GAMEPAD_SCHEME)
        {
            OnStickMoved?.Invoke(_playerInputActions.Player.LookStick.ReadValue<Vector2>());
        }

    }

    public void EnablePlayerInput()
    {
        _playerInputActions.Player.Enable();
        _playerInputActions.UI.Disable();

        _playerInputActions.Player.Attack.Enable();
    }

    public void EnableUIInput()
    {
        _playerInputActions.UI.Enable();
        _playerInputActions.Player.Disable();
    }

    public void DisableAllInput()
    {
        _playerInputActions.Disable();
    }

    private void SwitchBindings(PlayerInput playerInput)
    {
        string bindingGroup = _playerInputActions.controlSchemes.First(x => x.name == _playerInput.currentControlScheme).bindingGroup;
        _playerInputActions.bindingMask = InputBinding.MaskByGroup(bindingGroup);
    }

    public void OnAbility1(InputAction.CallbackContext context)
    {
        OnAbility1Performed?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnBasicAttackPerformed?.Invoke();
        }
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnBlockPerformed?.Invoke(true);

        if (context.canceled)
            OnBlockPerformed?.Invoke(false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        OnJumpPerformed?.Invoke();
    }
}
