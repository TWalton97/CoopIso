using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;

    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        animator.CrossFade(AttackHash, crossFadeDuration);
    }

    public override void Update()
    {
        agent.SetDestination(enemy.transform.position);
        agent.transform.rotation = GetRotationTowardsTarget();
        enemy.Attack();
    }

    private Quaternion GetRotationTowardsTarget()
    {
        Vector3 DirToTarget = player.transform.position - agent.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(DirToTarget, Vector3.up);
        targetRotation.x = 0;
        targetRotation.z = 0;
        return targetRotation;
    }
}
