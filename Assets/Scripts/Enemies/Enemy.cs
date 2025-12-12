using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    protected StateMachine stateMachine;
    protected Hitbox hitbox;

    [HideInInspector] public bool IsStaggered = false;

    public string StateName;
    protected EnemyWanderState wanderState;

    public bool spawned = false;

    public AnimationStatusTracker animationStatusTracker;

    protected bool attackOnCooldown = false;

    public bool HasSpawnedItems = false;

    public float StartWanderSpeed { get; private set; }
    public float StartChaseSpeed { get; private set; }

    public Entity target;
    public LayerMask targetLayer;
    public LayerMask obstructionLayer;
    public float attackRange;
    public bool InAttackRange = false;
    public bool CanAttack = false;
    public float NextAttackTime;

    private EnemyStatsSO enemyStats;

    public Dictionary<Entity, float> damageTable = new Dictionary<Entity, float>();
    public int damage;
    public int Level = 1;

    private bool hasTakenDamage = false;
    private float nextTargetSwapTime;

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
        agent.avoidancePriority = UnityEngine.Random.Range(0, 99);
        if (enemyStats.AttackRange < 3)
        {
            agent.stoppingDistance = enemyStats.AttackRange * 0.9f;
        }
    }

    protected virtual void Start()
    {
        stateMachine = new StateMachine();

        wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var chaseState = new EnemyChaseState(this, animator, agent);
        var waitToAttackState = new EnemyWaitToAttackState(this, animator, agent, target);
        var attackState = new EnemyAttackState(this, animator, agent, target);
        var deathState = new EnemyDieState(this, animator, agent, transform);

        At(wanderState, chaseState, new FuncPredicate(() => target != null));
        At(chaseState, wanderState, new FuncPredicate(() => target == null));

        At(chaseState, waitToAttackState, new FuncPredicate(() => InAttackRange));
        At(waitToAttackState, chaseState, new FuncPredicate(() => !InAttackRange));

        At(waitToAttackState, wanderState, new FuncPredicate(() => target == null));

        At(waitToAttackState, attackState, new FuncPredicate(() => CanAttack));
        At(attackState, waitToAttackState, new FuncPredicate(() => !CanAttack));

        Any(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(wanderState);

        stateMachine.OnStateChanged += UpdateStateName;
    }

    public override void ApplyStats()
    {
        HealthController.IncreaseMaximumHealth(EntityData.MaximumHealth * Level);
        chaseSpeed = EntityData.MovementSpeed;
        damage = enemyStats.AttackDamage * Level;
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

    public virtual void Update()
    {
        if (!spawned) return;
        stateMachine.Update();
        if (IsDead) return;
        UpdateAnimatorParameters();
        PushAwayFromNearbyEnemies();
    }

    public virtual void FixedUpdate()
    {
        if (IsDead) return;
        if (!spawned) return;
        stateMachine.FixedUpdate();
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetFloat("Velocity", agent.velocity.magnitude / chaseSpeed);
        animator.SetFloat("AttackSpeedMultiplier", AttackSpeedMultiplier);
    }

    public virtual void Attack()
    {
        if (target != null)
            hitbox.Init(enemyStats.AttackDamage, 1 << target.gameObject.layer, this, false);

        hitbox.ActivateHitbox(enemyStats.AttackDamage);
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

    protected void DistributeExperience()
    {
        NewPlayerController[] playerControllers = FindObjectsOfType<NewPlayerController>();
        foreach (NewPlayerController playerController in playerControllers)
        {
            playerController.ExperienceController.AddExperience(ExpValue);
        }
    }

    public EntityStatus ReturnEntityStatus()
    {
        EntityStatus.GUID = entityIdentity.GUID;
        EntityStatus.WorldPosition = transform.position;
        EntityStatus.IsDead = IsDead;
        return EntityStatus;
    }

    public void SetTarget(Entity target)
    {
        this.target = target;
    }

    public void FindTargetsInAggroRange()
    {
        EnemyStatsSO enemyStats = EntityData as EnemyStatsSO;
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyStats.AggroRange, targetLayer);
        if (colliders.Length == 0)
        {
            return;
        }

        foreach (Collider coll in colliders)
        {
            Vector3 dirToColl = (coll.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(coll.transform.position, transform.position);
            if (!Physics.Raycast(transform.position, dirToColl, distance, obstructionLayer))
            {
                if (coll.TryGetComponent(out Entity entity))
                {
                    if (entity != this)
                    {
                        if (!entity.IsDead)
                        {
                            if (!damageTable.ContainsKey(entity))
                            {
                                UpdateDamageTable(0, entity);
                            }
                        }
                    }
                }
            }
        }

        return;
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

        if (agent.isOnNavMesh)
            agent.Move(repulsion);
    }

    public void UpdateDamageTable(int damage, Entity entity)
    {
        if (damage > 0)
            hasTakenDamage = true;

        if (!damageTable.ContainsKey(entity))
        {
            damageTable.Add(entity, damage);
        }

        damageTable[entity] += damage;

        UpdateTarget();
    }

    private void PruneDeadEntities()
    {
        if (damageTable == null || damageTable.Count == 0)
            return;

        // Temp list so we don't modify the dictionary while iterating it
        List<Entity> toRemove = null;

        foreach (var kvp in damageTable)
        {
            Entity e = kvp.Key;

            if (e == null || e.IsDead)
            {
                if (toRemove == null)
                    toRemove = new List<Entity>(4); // small default size

                if (target == e)
                    target = null;

                toRemove.Add(e);
            }
        }

        // Remove after iteration
        if (toRemove != null)
        {
            foreach (var deadEntity in toRemove)
            {
                damageTable.Remove(deadEntity);
            }
        }
    }

    public void UpdateTarget()
    {
        PruneDeadEntities();

        if (damageTable.Count == 0)
            return;

        if (!hasTakenDamage)
        {
            if (Time.time < nextTargetSwapTime)
                return;

            float shortestDist = Mathf.Infinity;
            Entity nearestEntity = null;
            foreach (var kvp in damageTable.Keys)
            {
                float dist = Vector3.Distance(kvp.transform.position, transform.position);

                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    nearestEntity = kvp;
                }
            }

            if (target != nearestEntity)
            {
                target = nearestEntity;
                nextTargetSwapTime = Time.time + 1f;
            }
            return;
        }

        Entity topAttacker = null;
        float maxDamage = 0f;
        if (target != null)
        {
            topAttacker = target;
            maxDamage = damageTable[target] * 5f;
        }

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

    public void ClearDamageTable()
    {
        damageTable.Clear();
        target = null;
    }

    //If we haven't taken damage yet, then we search in a smaller radius around us for closer targets
    //Until we taken damage, we swap to the closest target
}
