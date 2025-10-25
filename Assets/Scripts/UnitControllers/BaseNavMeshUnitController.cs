using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Targeter))]
public class BaseNavMeshUnitController : BaseUnitController
{
    public NavMeshAgent Agent;
    protected StateMachine StateMachine;
    protected Targeter Targeter;
    public AttackController AttackController;
    [SerializeField] private Animator Animator;

    [ReadOnly] public BaseUnitController Target;
    [ReadOnly] public string StateName;

    protected bool attackCompleted = false;

    public override void Awake()
    {
        base.Awake();

        Agent = GetComponent<NavMeshAgent>();
        StateMachine = new StateMachine();
        Targeter = GetComponent<Targeter>();
        AttackController = GetComponent<AttackController>();
        Animator = GetComponentInChildren<Animator>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        StateMachine.OnStateChanged += UpdateStateName;
        HealthController.OnTakeDamage += SetTargetToDamager;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        StateMachine.OnStateChanged -= UpdateStateName;
    }

    public void At(IState from, IState to, IPredicate condition) => StateMachine.AddTransition(from, to, condition);
    public void Any(IState to, IPredicate condition) => StateMachine.AddAnyTransition(to, condition);

    public virtual void Update()
    {
        StateMachine.Update();

        Animator.SetFloat("Velocity", Agent.velocity.magnitude / Agent.speed);
    }

    public virtual void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }

    public void UpdateStateName() => StateName = StateMachine.current.State.ToString();

    public void SetAgentDestination(Vector3 position)
    {
        Agent.destination = position;
    }

    public void ResetAgentDestination()
    {
        Agent.destination = transform.position;
    }

    public bool HasTarget() => Target != null;

    public void FindTarget()
    {
        Target = Targeter.ReturnValidBaseUnitController();
    }

    public BaseUnitController ReturnTarget()
    {
        return Target;
    }

    public bool IsTargetStillInRange()
    {
        return Targeter.IsTargetStillInRange(Target);
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    private void SetTargetToDamager(int damage, BaseUnitController controller)
    {
        Target = controller;
    }

    public void CompleteAttack()
    {
        attackCompleted = true;
        Invoke(nameof(ResetBool), 2f);
    }

    public void Attack()
    {
        Animator.SetTrigger("Attack");
    }

    private void ResetBool() => attackCompleted = false;

}



