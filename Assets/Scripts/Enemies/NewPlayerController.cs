using UnityEngine;
using Utilities;
using UnityEngine.InputSystem;
using System;

public class NewPlayerController : Entity
{
    public Rigidbody Rigidbody;
    public Animator animator;
    [SerializeField] private PlayerInput playerInput;
    public NewWeaponController weaponController;
    public ExperienceController _experienceController;
    private GroundCheck groundCheck;

    public float _movementSpeed;
    [ReadOnly] public float _maximumMovementSpeed;
    public float _jumpForce;
    public float attackCooldown;

    public StateMachine attackStateMachine;
    public PlayerIdleState idleState;
    PlayerAttackState attackState;
    public PlayerBlockState blockState;

    public StateMachine movementStateMachine;
    CountdownTimer attackCooldownTimer;

    [SerializeField] private LayerMask GroundLayer = NavMeshUtils.GROUND_LAYER;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool blockButtonPressed;
    [HideInInspector] public bool attackButtonPressed;
    private const string KEYBOARD_SCHEME = "Keyboard&Mouse";
    private const string GAMEPAD_SCHEME = "Gamepad";

    #region MonoBehaviour
    public override void Awake()
    {
        base.Awake();
        attackCooldownTimer = new CountdownTimer(attackCooldown);
        _maximumMovementSpeed = _movementSpeed;
        groundCheck = GetComponent<GroundCheck>();
    }
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        SetupMovementStateMachine();
        SetupAttackStateMachine();

        InventoryManager.OnMenuOpened += () => playerInput.SwitchCurrentActionMap("UI");
        InventoryManager.OnMenuClosed += () => playerInput.SwitchCurrentActionMap("Player");
        InventoryManager.OnMenuClosed += () => attackStateMachine.ChangeState(idleState);
    }

    void OnDisable()
    {
        InventoryManager.OnMenuOpened -= () => playerInput.SwitchCurrentActionMap("UI");
        InventoryManager.OnMenuClosed -= () => playerInput.SwitchCurrentActionMap("Player");
        InventoryManager.OnMenuClosed -= () => attackStateMachine.ChangeState(idleState);
    }

    private void SetupAttackStateMachine()
    {
        attackStateMachine = new StateMachine();

        idleState = new PlayerIdleState(this, animator);
        attackState = new PlayerAttackState(this, animator);
        blockState = new PlayerBlockState(this, animator);

        At(idleState, blockState, attackStateMachine, new FuncPredicate(() => blockButtonPressed && weaponController.HasShieldEquipped));
        At(blockState, idleState, attackStateMachine, new FuncPredicate(() => !blockButtonPressed && weaponController.HasShieldEquipped));

        Any(attackState, attackStateMachine, new FuncPredicate(() => attackButtonPressed));

        attackStateMachine.SetState(idleState);
    }

    private void SetupMovementStateMachine()
    {
        movementStateMachine = new StateMachine();

        var groundedState = new PlayerGroundedState(this, animator);
        var airborneState = new PlayerAirborneState(this, animator);

        At(groundedState, airborneState, movementStateMachine, new FuncPredicate(() => !groundCheck.Grounded));
        At(airborneState, groundedState, movementStateMachine, new FuncPredicate(() => groundCheck.Grounded));

        movementStateMachine.SetState(groundedState);
    }

    void At(IState from, IState to, StateMachine machine, IPredicate condition) => machine.AddTransition(from, to, condition);
    void Any(IState to, StateMachine machine, IPredicate condition) => machine.AddAnyTransition(to, condition);

    void Update()
    {
        attackStateMachine.Update();
        movementStateMachine.Update();
        attackCooldownTimer.Tick(Time.deltaTime);
        Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity).normalized;
        animator.SetFloat("VelocityX", Mathf.Clamp(localVelocity.x, -1, 1));
        animator.SetFloat("VelocityZ", Mathf.Clamp(localVelocity.z, -1, 1));
        animator.SetFloat("SpeedMultiplier", Mathf.Clamp(_movementSpeed / _maximumMovementSpeed, 0.5f, 1));

    }
    void FixedUpdate()
    {
        movementStateMachine.FixedUpdate();
        attackStateMachine.FixedUpdate();
        Move();
    }
    #endregion

    #region Movement
    private void Move()
    {
        Vector3 inputDirection = new Vector3(_moveInput.x, 0, _moveInput.y);

        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        Vector3 newVel = rotatedInputDirection * _movementSpeed;
        newVel.y = Rigidbody.velocity.y;

        Rigidbody.velocity = newVel;

        if (playerInput.currentControlScheme == GAMEPAD_SCHEME && _lookInput == Vector2.zero)
        {
            RotateTowardsStick(_moveInput);
        }
    }

    #endregion

    #region Looking
    private void RotateTowardsMouse(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, GroundLayer))
        {
            Vector3 hitPoint = hit.point;
            Vector3 updatedHitPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            Vector3 dirToPoint = (updatedHitPoint - transform.position).normalized;
            transform.LookAt(transform.position + dirToPoint);
        }
    }
    private void RotateTowardsStick(Vector2 dir)
    {
        Vector3 inputDirection = new Vector3(dir.x, 0, dir.y);
        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        transform.LookAt(transform.position + rotatedInputDirection);
    }
    #endregion

    #region PlayerInputMessages
    private void OnAbility1()
    {

    }

    public void OnLookStick(InputValue value)
    {
        var dir = value.Get<Vector2>();
        _lookInput = dir;
        RotateTowardsStick(dir);
    }

    public void OnLookMouse(InputValue value)
    {
        var dir = value.Get<Vector2>();
        RotateTowardsMouse(dir);
    }

    public void OnBlock(InputValue value)
    {
        if (!weaponController.HasShieldEquipped) return;

        if (value.isPressed)
        {
            blockButtonPressed = true;
        }
        else
        {
            blockButtonPressed = false;
        }
    }

    private void OnAttack()
    {
        if (weaponController.canAttack && weaponController.instantiatedPrimaryWeapon != null)
        {
            attackButtonPressed = true;
        }
    }

    public void OnJump()
    {
        if (!groundCheck.Grounded) return;
        animator.CrossFade(Animator.StringToHash("Jump"), 0.2f, (int)PlayerAnimatorLayers.FullBody);
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }
    #endregion

}
