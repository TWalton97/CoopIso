using UnityEngine;
using Utilities;
using UnityEngine.InputSystem;
using System;

public class NewPlayerController : Entity
{
    public Rigidbody Rigidbody;
    public Animator animator;
    [SerializeField] private NewPlayerInputController PlayerInputController;
    [SerializeField] private PlayerInput playerInput;
    public NewWeaponController weaponController;
    public ExperienceController _experienceController;
    private GroundCheck groundCheck;

    public float _movementSpeed;
    [ReadOnly] public float _maximumMovementSpeed;
    public float _jumpForce;
    public float attackCooldown;

    private PlayerIdleState idleState;
    private PlayerAttackState attackState;
    private PlayerBlockState blockState;
    public StateMachine stateMachine;
    public StateMachine movementStateMachine;
    CountdownTimer attackCooldownTimer;

    private Vector3 _mousePosOnGround;
    [SerializeField] private LayerMask GroundLayer = NavMeshUtils.GROUND_LAYER;
    public bool AttackCompleted = false;
    private bool blocking = false;
    private Vector2 _moveInput;
    [SerializeField] private Vector2 _lookInput;
    int equippedWeapon = 0;
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
        stateMachine = new StateMachine();

        playerInput = GetComponent<PlayerInput>();

        idleState = new PlayerIdleState(this, animator);
        var moveState = new PlayerMoveState(this, animator);
        attackState = new PlayerAttackState(this, animator, weaponController.instantiatedPrimaryWeapon);
        var attackComboState = new PlayerComboAttackState(this, animator);
        blockState = new PlayerBlockState(this, animator, weaponController.instantiatedSecondaryWeapon);

        At(idleState, blockState, stateMachine, new FuncPredicate(() => blocking));
        At(blockState, idleState, stateMachine, new FuncPredicate(() => !blocking));

        At(attackState, idleState, stateMachine, new FuncPredicate(() => attackCooldownTimer.IsFinished));

        At(attackComboState, idleState, stateMachine, new FuncPredicate(() => attackCooldownTimer.IsFinished));

        stateMachine.SetState(idleState);

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
        stateMachine.Update();
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
        stateMachine.FixedUpdate();
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
            _mousePosOnGround = updatedHitPoint;
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

    public void OnSwapWeapon()
    {
        if (equippedWeapon == 0 || equippedWeapon == 2)
        {
            EquipWeaponOne();
            equippedWeapon = 1;
        }
        else
        {
            EquipWeaponTwo();
            equippedWeapon = 2;
        }
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

    private void OnBlock(bool performed)
    {
        blocking = performed;
        animator.SetBool("Blocking", performed);
    }

    private void OnAttack()
    {
        if (weaponController.instantiatedPrimaryWeapon == null) return;
        weaponController.Attack();
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

    #region Debug
    [ContextMenu("Equip Weapon One")]
    public void EquipWeaponOne()
    {
        weaponController.EquipWeapon(NewWeaponController.WeaponAttackTypes.DualWield, WeaponManager.Instance.OneHandedAxe, WeaponManager.Instance.OneHandedMace);
    }

    [ContextMenu("Equip Weapon Two")]
    public void EquipWeaponTwo()
    {
        weaponController.EquipWeapon(NewWeaponController.WeaponAttackTypes.TwoHanded, WeaponManager.Instance.TwoHandedSword);
    }
    #endregion
}
