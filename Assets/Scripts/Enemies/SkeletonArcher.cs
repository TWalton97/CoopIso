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
        var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
        var attackState = new EnemyArcherAttackState(this, animator, agent, playerDetector.Player);
        var deathState = new EnemyDieState(this, animator, agent, transform);
        var staggerStage = new EnemyStaggerState(this, animator, agent, transform);

        At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
        At(attackState, chaseState, new FuncPredicate(() => attackState.AttackCompleted));

        //Any(staggerStage, new FuncPredicate(() => IsStaggered && !IsDead));
        //At(staggerStage, chaseState, new FuncPredicate(() => !IsStaggered && !IsDead));

        Any(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(wanderState);

        stateMachine.OnStateChanged += UpdateStateName;
    }

    public override void Attack()
    {
        if (attackTimer.IsRunning) return;

        Projectile proj = Instantiate(projectile, arrowSpawnPos.position, arrowSpawnPos.transform.rotation);
        proj.Init(projectileSpeed, damage, this, 3, false);
        //Instantiate projectile here
        attackTimer.Start();
    }

    // private Quaternion GetRotationTowardsTarget()
    // {
    //     Vector3 DirToTarget = enemy.playerDetector.Player.transform.position - agent.transform.position;
    //     Quaternion targetRotation = Quaternion.LookRotation(DirToTarget, Vector3.up);
    //     targetRotation.x = 0;
    //     targetRotation.z = 0;
    //     return targetRotation;
    // }
}
