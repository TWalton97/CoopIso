using UnityEngine;
using Utilities;
public class SkeletonArcher : Enemy
{
    public Projectile projectile;
    public float projectileSpeed;
    public int damage;
    public Transform arrowSpawnPos;
    protected override void Start()
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

    public override void Attack()
    {
        if (attackTimer.IsRunning) return;

        Projectile proj = Instantiate(projectile, arrowSpawnPos.position, transform.rotation);
        proj.GetComponent<Collider>().includeLayers = targetLayer;
        proj.GetComponent<Collider>().excludeLayers = 1 >> gameObject.layer;

        proj.Init(projectileSpeed, damage, this, 3, false);
        attackTimer.Start();
    }

    public override void Die()
    {
        DistributeExperience();
        Destroy(gameObject, 1);
    }
}
