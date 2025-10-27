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
    public float comboWindow;

    private PlayerAttackState attackState;

    public StateMachine stateMachine;
    CountdownTimer attackCooldownTimer;


    private Vector3 _mousePosOnGround;
    [SerializeField] private LayerMask GroundLayer = NavMeshUtils.GROUND_LAYER;
    public bool AttackCompleted = false;
    public override void Awake()
    {
        base.Awake();
        attackCooldownTimer = new CountdownTimer(attackCooldown);
        _maximumMovementSpeed = _movementSpeed;
    }
    void Start()
    {
        stateMachine = new StateMachine();

        var idleState = new PlayerIdleState(this, animator);
        var moveState = new PlayerMoveState(this, animator);
        attackState = new PlayerAttackState(this, animator);
        var attackComboState = new PlayerComboAttackState(this, animator);

        At(idleState, moveState, new FuncPredicate(() => Rigidbody.velocity.magnitude > 2));
        At(moveState, idleState, new FuncPredicate(() => Rigidbody.velocity.magnitude <= 0.5f));

        At(attackState, idleState, new FuncPredicate(() => attackCooldownTimer.IsFinished));

        At(attackComboState, idleState, new FuncPredicate(() => attackCooldownTimer.IsFinished));

        stateMachine.SetState(idleState);
    }

    void OnEnable()
    {
        PlayerInputController.OnMovePerformed += Move;
        PlayerInputController.OnJumpPerformed += Jump;
        PlayerInputController.OnBasicAttackPerformed += BasicAttack;
        PlayerInputController.OnMouseMoved += RotateTowardsMouse;
        PlayerInputController.OnAbility1Performed += Ability1;
    }

    void OnDisable()
    {
        PlayerInputController.OnMovePerformed -= Move;
        PlayerInputController.OnJumpPerformed -= Jump;
        PlayerInputController.OnBasicAttackPerformed -= BasicAttack;
        PlayerInputController.OnMouseMoved -= RotateTowardsMouse;
        PlayerInputController.OnAbility1Performed -= Ability1;
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        attackCooldownTimer.Tick(Time.deltaTime);
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void Move(Vector2 moveDir)
    {
        Vector3 inputDirection = new Vector3(moveDir.x, 0, moveDir.y);

        Quaternion rotation = Quaternion.AngleAxis(225f, Vector3.up);

        Vector3 rotatedInputDirection = rotation * inputDirection;

        Vector3 newVel = rotatedInputDirection * _movementSpeed;
        newVel.y = Rigidbody.velocity.y;

        Rigidbody.velocity = newVel;
    }

    public void Jump()
    {
        Rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
    }

    private void BasicAttack()
    {
        if (attackCooldownTimer.IsRunning)
        {
            if (stateMachine.current.State == attackState && attackState.CanAttack())
            {
                attackState.ProgressCombo();
                attackCooldownTimer.Reset();
            }
            return;
        }

        stateMachine.ChangeState(attackState);
        attackCooldownTimer.Start();
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

    private void Ability1()
    {

    }
}
