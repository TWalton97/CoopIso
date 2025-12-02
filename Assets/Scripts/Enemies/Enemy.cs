using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

[RequireComponent(typeof(EntityIdentity))]
public class Enemy : Entity
{
    private EntityIdentity entityIdentity;
    public StatusController statusController;
    public SpawnedItemDataBase spawnedItemDataBase;
    public int ExpValue;
    public float ExpRange;
    public NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    public Collider coll;
    public GameObject ragdoll;
    public List<GameObject> body;

    public float wanderSpeed;
    public float chaseSpeed;

    public float rotationSpeed;

    [SerializeField] protected float wanderRadius = 10f;
    public float timeBetweenAttacks = 1f;

    protected StateMachine stateMachine;
    protected Hitbox hitbox;

    protected CountdownTimer attackTimer;

    public bool IsDead = false;
    [HideInInspector] public bool IsStaggered = false;

    public string StateName;
    protected EnemyWanderState wanderState;

    public bool spawned = false;

    public AnimationStatusTracker animationStatusTracker;

    protected bool attackOnCooldown = false;

    public bool HasSpawnedItems = false;

    public float StartWanderSpeed { get; private set; }
    public float StartChaseSpeed { get; private set; }

    public Transform target;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;
    public float attackRange;
    public bool InAttackRange = false;
    public bool CanAttack = false;
    public float NextAttackTime;

    private EnemyStatsSO enemyStats;

    private Dictionary<Transform, float> damageTable = new Dictionary<Transform, float>();

    public override void Awake()
    {
        base.Awake();

        enemyStats = EntityData as EnemyStatsSO;


        entityIdentity = GetComponent<EntityIdentity>();

        EntityStatus = new EntityStatus(entityIdentity.GUID, transform.position, IsDead);

        hitbox = GetComponentInChildren<Hitbox>();
        animationStatusTracker = GetComponent<AnimationStatusTracker>();
        if (animationStatusTracker == null)
            animationStatusTracker = GetComponentInChildren<AnimationStatusTracker>();

        statusController = GetComponent<StatusController>();
        StartWanderSpeed = wanderSpeed;
        StartChaseSpeed = chaseSpeed;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(0, 99);
        agent.stoppingDistance = enemyStats.AttackRange * 0.9f;
        ApplyStats();
    }

    protected virtual void Start()
    {
        attackTimer = new CountdownTimer(timeBetweenAttacks);

        stateMachine = new StateMachine();

        wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, agent, target);
        var waitToAttackState = new EnemyWaitToAttackState(this, animator, agent, target);
        var attackState = new EnemyAttackState(this, animator, agent, target);
        var deathState = new EnemyDieState(this, animator, agent, transform);

        At(wanderState, chaseState, new FuncPredicate(() => target != null));
        At(chaseState, wanderState, new FuncPredicate(() => target == null));

        At(chaseState, waitToAttackState, new FuncPredicate(() => InAttackRange));
        At(waitToAttackState, chaseState, new FuncPredicate(() => !InAttackRange));

        At(waitToAttackState, attackState, new FuncPredicate(() => CanAttack));
        At(attackState, waitToAttackState, new FuncPredicate(() => !CanAttack));

        Any(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(wanderState);

        stateMachine.OnStateChanged += UpdateStateName;
    }

    public override void ApplyStats()
    {
        base.ApplyStats();
        chaseSpeed = EntityData.MovementSpeed;
        if (EntityData is EnemyStatsSO enemyData)
        {
            wanderSpeed = enemyData.WanderSpeed;
        }
    }

    private void OnEnable()
    {
        spawned = true;
        if (wanderState != null) stateMachine.SetState(wanderState);
        HealthController.OnTakeDamage += Stagger;
        HealthController.OnDie += () => IsDead = true;
        HealthController.OnDie += Die;
        HealthController.OnTakeDamage += AlertNearbyEnemies;
        HealthController.OnTakeDamage += UpdateDamageTable;
    }

    private void OnDisable()
    {
        HealthController.OnTakeDamage -= Stagger;
        HealthController.OnDie -= () => IsDead = true;
        HealthController.OnDie -= Die;

        stateMachine.OnStateChanged -= UpdateStateName;
        HealthController.OnTakeDamage -= AlertNearbyEnemies;
        HealthController.OnTakeDamage -= UpdateDamageTable;
    }

    protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

    void Update()
    {
        if (!spawned) return;
        stateMachine.Update();
        attackTimer.Tick(Time.deltaTime);
        UpdateAnimatorParameters();
        PushAwayFromNearbyEnemies();
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
        //Destroy(gameObject, 5);
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

    public IEnumerator AttackCooldown()
    {
        attackOnCooldown = true;
        yield return new WaitForSeconds(timeBetweenAttacks);
        attackOnCooldown = false;
        yield return null;
    }

    public EntityStatus ReturnEntityStatus()
    {
        EntityStatus.GUID = entityIdentity.GUID;
        EntityStatus.WorldPosition = transform.position;
        EntityStatus.IsDead = IsDead;
        return EntityStatus;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public Transform FindTargetInAggroRange()
    {
        EnemyStatsSO enemyStats = EntityData as EnemyStatsSO;
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyStats.AggroRange, targetLayer);
        if (colliders.Length == 0)
        {
            return null;
        }

        foreach (Collider coll in colliders)
        {
            Vector3 dirToColl = (coll.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(coll.transform.position, transform.position);
            if (!Physics.Raycast(transform.position, dirToColl, distance, obstructionLayer))
            {
                return coll.transform;
            }
        }

        return colliders[0].transform;
    }

    public void PushAwayFromNearbyEnemies()
    {
        if (IsDead) return;
        Vector3 repulsion = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(transform.position, 1);

        foreach (var col in neighbors)
        {
            if (col.gameObject == gameObject) continue;
            Enemy other = col.GetComponent<Enemy>();
            if (other != null)
            {
                Vector3 away = (transform.position - other.transform.position).normalized;
                float dist = Vector3.Distance(transform.position, other.transform.position);
                float strength = Mathf.Clamp01(1 - dist / 1); // stronger when closer
                repulsion += away * strength * 0.1f; // tweak 0.1f for intensity
            }
        }

        agent.Move(repulsion);
    }

    private void UpdateDamageTable(int damage, Entity entity)
    {
        if (!damageTable.ContainsKey(entity.transform))
        {
            damageTable.Add(entity.transform, 0f);
        }

        damageTable[entity.transform] += damage;

        UpdateTarget();
    }

    public void UpdateTarget()
    {
        if (damageTable.Count == 0)
            return;

        Transform topAttacker = null;
        float maxDamage = 0f;

        foreach (var kvp in damageTable)
        {
            if (kvp.Value >= maxDamage)
            {
                maxDamage = kvp.Value;
                topAttacker = kvp.Key;
            }
        }

        if (topAttacker != null)
            target = topAttacker;
    }

    private void AlertNearbyEnemies(int damage, Entity attacker)
    {
        LayerMask enemyLayer = 1 << gameObject.layer;
        Collider[] nearby = Physics.OverlapSphere(transform.position, enemyStats.AggroRange, enemyLayer);
        foreach (var col in nearby)
        {
            if (col.transform == transform) continue;
            Enemy other = col.GetComponent<Enemy>();
            if (other != null)
            {
                if (other.target == null)
                {
                    other.UpdateDamageTable(0, attacker);
                }
            }
        }
    }
}
