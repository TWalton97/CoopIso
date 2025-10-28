using UnityEngine;
using Utilities;
using UnityEngine.InputSystem;

public class NewPlayerController : Entity
{
    public Rigidbody Rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private NewPlayerInputController PlayerInputController;
    [SerializeField] private PlayerInput playerInput;
    public NewWeaponController weaponController;
    public ExperienceController _experienceController;

    public float _movementSpeed;
    [ReadOnly] public float _maximumMovementSpeed;
    public float _jumpForce;
    public float attackCooldown;

    private PlayerIdleState idleState;
    private PlayerAttackState attackState;
    private PlayerBlockState blockState;
    public StateMachine stateMachine;
    CountdownTimer attackCooldownTimer;

    public Weapon currentWeapon;
    public Weapon weapon1;
    public Weapon weapon2;
    public GameObject shield;


    private Vector3 _mousePosOnGround;
    [SerializeField] private LayerMask GroundLayer = NavMeshUtils.GROUND_LAYER;
    public bool AttackCompleted = false;
    private bool blocking = false;
    private Vector2 _moveInput;
    [SerializeField] private Vector2 _lookInput;
    private const string KEYBOARD_SCHEME = "Keyboard&Mouse";
    private const string GAMEPAD_SCHEME = "Gamepad";
    public override void Awake()
    {
        base.Awake();
        attackCooldownTimer = new CountdownTimer(attackCooldown);
        _maximumMovementSpeed = _movementSpeed;
        weapon1.SetPlayer(this);
        weapon2.SetPlayer(this);
    }

    void Start()
    {
        stateMachine = new StateMachine();
        playerInput = GetComponent<PlayerInput>();

        idleState = new PlayerIdleState(this, animator);
        var moveState = new PlayerMoveState(this, animator);
        attackState = new PlayerAttackState(this, animator, weapon1);
        var attackComboState = new PlayerComboAttackState(this, animator);
        blockState = new PlayerBlockState(this, animator, weapon1);

        At(idleState, moveState, new FuncPredicate(() => Rigidbody.velocity.magnitude > 2));
        At(moveState, idleState, new FuncPredicate(() => Rigidbody.velocity.magnitude <= 0.5f));

        At(idleState, blockState, new FuncPredicate(() => blocking));
        At(blockState, idleState, new FuncPredicate(() => !blocking));

        At(attackState, idleState, new FuncPredicate(() => attackCooldownTimer.IsFinished));

        At(attackComboState, idleState, new FuncPredicate(() => attackCooldownTimer.IsFinished));

        stateMachine.SetState(idleState);
    }

    void OnEnable()
    {
        // PlayerInputController.OnMovePerformed += Move;
        // PlayerInputController.OnJumpPerformed += Jump;
        // PlayerInputController.OnBasicAttackPerformed += BasicAttack;
        // PlayerInputController.OnMouseMoved += RotateTowardsMouse;
        // PlayerInputController.OnStickMoved += RotateTowardsStick;
        // PlayerInputController.OnAbility1Performed += Ability1;
        // PlayerInputController.OnBlockPerformed += Block;
    }

    void OnDisable()
    {
        // PlayerInputController.OnMovePerformed -= Move;
        // PlayerInputController.OnJumpPerformed -= Jump;
        // PlayerInputController.OnBasicAttackPerformed -= BasicAttack;
        // PlayerInputController.OnMouseMoved -= RotateTowardsMouse;
        // PlayerInputController.OnStickMoved -= RotateTowardsStick;
        // PlayerInputController.OnAbility1Performed -= Ability1;
        // PlayerInputController.OnBlockPerformed -= Block;
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        attackCooldownTimer.Tick(Time.deltaTime);
        Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity).normalized;
        animator.SetFloat("VelocityX", Mathf.Clamp(localVelocity.x, -1, 1));
        animator.SetFloat("VelocityZ", Mathf.Clamp(localVelocity.z, -1, 1));
        animator.SetFloat("SpeedMultiplier", Mathf.Clamp(_movementSpeed / _maximumMovementSpeed, 0.5f, 1));
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        Move();
        //Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * _movementSpeed;
        //Rigidbody.velocity = new Vector3(movement.x, Rigidbody.velocity.y, movement.z);
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

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

    public void SetVelocity(Vector3 direction, float velocity)
    {
        Rigidbody.velocity = ConvertGlobalDirectionToLocal(direction) * velocity;
    }

    public Vector3 ConvertGlobalDirectionToLocal(Vector3 direction)
    {
        var dir = transform.TransformDirection(direction);
        return dir;
    }

    public void OnJump()
    {
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    private void OnAttack()
    {
        currentWeapon.Enter();
        // if (attackCooldownTimer.IsRunning)
        // {
        //     return;
        // }

        // stateMachine.ChangeState(attackState);
        // attackCooldownTimer.Start();
    }

    private void OnBlock(bool performed)
    {
        blocking = performed;
        animator.SetBool("Blocking", performed);
    }

    public void OnLookMouse(InputValue value)
    {
        var dir = value.Get<Vector2>();
        RotateTowardsMouse(dir);
    }

    public void OnLookStick(InputValue value)
    {
        var dir = value.Get<Vector2>();
        _lookInput = dir;
        RotateTowardsStick(dir);
    }

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

    private void OnAbility1()
    {

    }

    [ContextMenu("Equip Weapon One")]
    public void EquipWeaponOne()
    {
        currentWeapon = weapon1;
        animator.runtimeAnimatorController = weapon1.animatorOverrideController;
        weapon1.gameObject.SetActive(true);

        weapon2.gameObject.SetActive(false);
        shield.SetActive(false);
    }

    [ContextMenu("Equip Weapon Two")]
    public void EquipWeaponTwo()
    {
        currentWeapon = weapon2;
        animator.runtimeAnimatorController = weapon2.animatorOverrideController;
        weapon1.gameObject.SetActive(false);

        weapon2.gameObject.SetActive(true);
        shield.SetActive(true);
    }
}
