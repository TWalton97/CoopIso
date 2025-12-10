using UnityEngine;
using UnityEngine.AI;
public class FriendlySkeletonDieState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public FriendlySkeletonDieState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        agent.isStopped = true;

        animator.Play(DieHash);

        enemy.coll.enabled = false;

        if (enemy.statusController != null)
        {
            enemy.statusController.RemoveAllStatuses();
        }
    }

    public override void Update()
    {

    }
}
