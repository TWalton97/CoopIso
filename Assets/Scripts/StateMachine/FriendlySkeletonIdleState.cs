using UnityEngine;
using UnityEngine.AI;
public class FriendlySkeletonIdleState : EnemyBaseState
{
    //When we're in range of our owner, but there's no enemies nearby, we just wander
    protected NavMeshAgent agent;
    protected EnemyStatsSO enemyStats;
    public FriendlySkeletonIdleState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
    {
        this.agent = agent;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        agent.isStopped = false;
    }

    public override void OnExit()
    {
        agent.isStopped = true;
    }
}
