using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Entity target;
    public bool AttackCompleted;
    private EnemyStatsSO enemyStats;

    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Entity target) : base(enemy, animator)
    {
        this.agent = agent;
        this.target = target;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        animator.CrossFade(AttackHash, crossFadeDuration);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        enemy.animationStatusTracker.OnAttackCompleted += CompleteAttack;
    }

    public override void OnExit()
    {
        agent.isStopped = false;
        float distance = enemy.target != null ? Vector3.Distance(enemy.transform.position, enemy.target.transform.position) : Mathf.Infinity;

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
        enemy.NextAttackTime = Time.time + (enemyStats.AttackCooldown / enemy.AttackSpeedMultiplier);
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
