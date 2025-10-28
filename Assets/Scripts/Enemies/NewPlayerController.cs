using UnityEngine;
using Utilities;

public class NewPlayerController : Entity
{
    public Rigidbody Rigidbody;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputController PlayerInputController;
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
    private Vector2 moveInput;
    public override void Awake()
    {
        base.Awake();
        attackCooldownTimer = new CountdownTimer(attackCooldown);
        _maximumMovementSpeed = _movementSpeed;
    }
    void Start()
    {
        stateMachine = new StateMachine();

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

        weapon1.SetPlayer(this);
        weapon2.SetPlayer(this);
    }

    void OnEnable()
    {
        PlayerInputController.OnMovePerformed += Move;
        PlayerInputController.OnJumpPerformed += Jump;
        PlayerInputController.OnBasicAttackPerformed += BasicAttack;
        PlayerInputController.OnMouseMoved += RotateTowardsMouse;
        PlayerInputController.OnStickMoved += RotateTowardsStick;
        PlayerInputController.OnAbility1Performed += Ability1;
        PlayerInputController.OnBlockPerformed += Block;
    }

    void OnDisable()
    {
        PlayerInputController.OnMovePerformed -= Move;
        PlayerInputController.OnJumpPerformed -= Jump;
        PlayerInputController.OnBasicAttackPerformed -= BasicAttack;
        PlayerInputController.OnMouseMoved -= RotateTowardsMouse;
        PlayerInputController.OnStickMoved -= RotateTowardsStick;
        PlayerInputController.OnAbility1Performed -= Ability1;
        PlayerInputController.OnBlockPerformed -= Block;
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        attackCooldownTimer.Tick(Time.deltaTime);
        Vector3 localVelocity = transform.InverseTransformDirection(Rigidbody.velocity);
        animator.SetFloat("VelocityX", Mathf.Clamp(localVelocity.x, -1, 1));
        animator.SetFloat("VelocityZ", Mathf.Clamp(localVelocity.z, -1, 1));
        animator.SetFloat("SpeedMultiplier", Mathf.Clamp(_movementSpeed / _maximumMovementSpeed, 0.5f, 1));
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();

        //Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * _movementSpeed;
        //Rigidbody.velocity = new Vector3(movement.x, Rigidbody.velocity.y, movement.z);
    }

    public void Move(Vector2 moveDir)
    {
        moveInput = moveDir;
        Vector3 inputDirection = new Vector3(moveDir.x, 0, moveDir.y);

        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        Vector3 newVel = rotatedInputDirection * _movementSpeed;
        newVel.y = Rigidbody.velocity.y;

        Rigidbody.velocity = newVel;
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

    public void Jump()
    {
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    private void BasicAttack()
    {
        currentWeapon.Enter();
        // if (attackCooldownTimer.IsRunning)
        // {
        //     return;
        // }

        // stateMachine.ChangeState(attackState);
        // attackCooldownTimer.Start();
    }

    private void Block(bool performed)
    {
        blocking = performed;
        animator.SetBool("Blocking", performed);
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
        print("Stick moving");
    }

    private void Ability1()
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
