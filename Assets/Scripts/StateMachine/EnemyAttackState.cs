using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public bool AttackCompleted;
    private EnemyStatsSO enemyStats;

    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        animator.CrossFade(AttackHash, crossFadeDuration);
        agent.isStopped = true;
        enemy.animationStatusTracker.OnAttackCompleted += CompleteAttack;
    }

    public override void OnExit()
    {
        agent.isStopped = false;
        float distance = enemy.target != null ? Vector3.Distance(enemy.transform.position, enemy.target.position) : Mathf.Infinity;

        if (distance <= enemyStats.AttackRange)
        {
            enemy.InAttackRange = true;
        }
        else
        {
            enemy.InAttackRange = false;
        }

        AttackCompleted = false;
        enemy.animationStatusTracker.OnAttackCompleted -= CompleteAttack;
    }

    private void CompleteAttack()
    {
        enemy.NextAttackTime = Time.time + enemyStats.AttackCooldown;
        enemy.CanAttack = false;
        AttackCompleted = true;
        enemy.CanAttack = false;
    }

    public override void Update()
    {
        // animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // actualNormalizedTime = Mathf.Clamp01(animatorStateInfo.normalizedTime);

        // agent.SetDestination(enemy.transform.position);
        // //RotateTowardsTarget(GetRotationTowardsTarget());
    }
}
