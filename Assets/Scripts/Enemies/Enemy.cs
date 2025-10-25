using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class Enemy : Entity
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] PlayerDetector playerDetector;
    [SerializeField] Animator animator;

    public float wanderSpeed;
    public float chaseSpeed;

    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float timeBetweenAttacks = 1f;

    StateMachine stateMachine;
    Hitbox hitbox;

    CountdownTimer attackTimer;

    private bool IsDead = false;

    public override void Awake()
    {
        base.Awake();
        hitbox = GetComponentInChildren<Hitbox>();
    }

    void Start()
    {
        attackTimer = new CountdownTimer(timeBetweenAttacks);

        stateMachine = new StateMachine();

        var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
        var deathState = new EnemyDieState(this, animator, agent, transform);

        At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

        Any(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(wanderState);
    }

    private void OnEnable()
    {
        healthController.OnDie += () => IsDead = true;
    }

    private void OnDisable()
    {
        healthController.OnDie -= () => IsDead = true;
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        attackTimer.Tick(Time.deltaTime);
    }

    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void Attack()
    {
        if (attackTimer.IsRunning) return;

        hitbox.ActivateHitbox(2);
        attackTimer.Start();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
