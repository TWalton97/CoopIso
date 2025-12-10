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
        agent.enabled = false;

        animator.SetLayerWeight(0, 0f);
        animator.SetLayerWeight(1, 0f);
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
