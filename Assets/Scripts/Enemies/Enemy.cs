using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

public class Enemy : Entity
{
    public SpawnedItemDataBase spawnedItemDataBase;
    public int ExpValue;
    public float ExpRange;
    [SerializeField] protected NavMeshAgent agent;
    public PlayerDetector playerDetector;
    [SerializeField] protected Animator animator;
    public Collider coll;
    public GameObject ragdoll;
    public List<GameObject> body;

    public float wanderSpeed;
    public float chaseSpeed;

    public float rotationSpeed;

    [SerializeField] protected float wanderRadius = 10f;
    [SerializeField] protected float timeBetweenAttacks = 1f;

    protected StateMachine stateMachine;
    protected Hitbox hitbox;

    protected CountdownTimer attackTimer;

    protected bool IsDead = false;
    [HideInInspector] public bool IsStaggered = false;

    public string StateName;
    protected EnemyWanderState wanderState;

    protected bool spawned = false;

    public AnimationStatusTracker animationStatusTracker;

    public override void Awake()
    {
        base.Awake();
        hitbox = GetComponentInChildren<Hitbox>();
        StartCoroutine(WaitForNavMeshAndSpawn());
        animationStatusTracker = GetComponent<AnimationStatusTracker>();
        if (animationStatusTracker == null)
            animationStatusTracker = GetComponentInChildren<AnimationStatusTracker>();
    }

    protected virtual void Start()
    {
        attackTimer = new CountdownTimer(timeBetweenAttacks);

        stateMachine = new StateMachine();

        wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
        var deathState = new EnemyDieState(this, animator, agent, transform);
        var staggerStage = new EnemyStaggerState(this, animator, agent, transform);

        At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => attackState.AttackCompleted));

        Any(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(wanderState);

        stateMachine.OnStateChanged += UpdateStateName;
    }

    private void OnEnable()
    {
        if (wanderState != null) stateMachine.SetState(wanderState);
        healthController.OnTakeDamage += Stagger;
        healthController.OnDie += () => IsDead = true;
        healthController.OnDie += Die;
    }

    private void OnDisable()
    {
        healthController.OnTakeDamage -= Stagger;
        healthController.OnDie -= () => IsDead = true;
        healthController.OnDie -= Die;

        stateMachine.OnStateChanged -= UpdateStateName;
    }

    protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        if (!spawned) return;
        stateMachine.Update();
        attackTimer.Tick(Time.deltaTime);
        UpdateAnimatorParameters();
    }

    void FixedUpdate()
    {
        if (!spawned) return;
        stateMachine.FixedUpdate();
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetFloat("Velocity", agent.velocity.magnitude / chaseSpeed);
    }

    public virtual void Attack()
    {
        if (attackTimer.IsRunning) return;

        hitbox.ActivateHitbox(2);
        attackTimer.Start();
    }

    public override void Die()
    {
        DistributeExperience();
        Destroy(gameObject, 5);
    }

    private void Stagger(int damage, Entity controller)
    {
        IsStaggered = true;
    }

    protected virtual void UpdateStateName()
    {
        StateName = stateMachine.current.State.ToString();
    }

    private void DistributeExperience()
    {
        NewPlayerController[] playerControllers = FindObjectsOfType<NewPlayerController>();
        foreach (NewPlayerController playerController in playerControllers)
        {
            playerController.ExperienceController.AddExperience(ExpValue);
        }
    }

    private IEnumerator WaitForNavMeshAndSpawn()
    {
        while (!NavMesh.SamplePosition(transform.position, out _, 1f, NavMesh.AllAreas))
        {
            yield return new WaitForSeconds(0.5f);
        }
        agent.enabled = true;
        spawned = true;
    }
}
