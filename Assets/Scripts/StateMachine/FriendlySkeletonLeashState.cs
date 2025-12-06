using UnityEngine;
using UnityEngine.AI;
public class FriendlySkeletonLeashState : EnemyBaseState
{
    //If the player exceeds our leash range, we run towards our owner
    //If the player exceeds double our leash range, we teleport to our owner
    protected NavMeshAgent agent;
    protected EnemyStatsSO enemyStats;
    private Transform _ownerTransform;
    public FriendlySkeletonLeashState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform ownerTransform) : base(enemy, animator)
    {
        this.agent = agent;
        enemyStats = enemy.EntityData as EnemyStatsSO;
        _ownerTransform = ownerTransform;
    }

    public override void OnEnter()
    {
        agent.speed = enemyStats.ChaseSpeed;
        agent.isStopped = false;
    }

    public override void Update()
    {
        agent.destination = _ownerTransform.position;
        if (enemy is FriendlySkeletonArcher archer)
        {
            if (!archer.OwnerInTeleportRange())
            {
                TeleportToPlayer();
            }
        }
    }

    public override void OnExit()
    {
        agent.speed = enemyStats.WanderSpeed;
        agent.isStopped = true;
    }

    private void TeleportToPlayer()
    {
        Vector3 offset = -_ownerTransform.forward * 1.5f;
        Vector3 targetPosition = _ownerTransform.position + offset;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            agent.Warp(targetPosition);
        }
    }
}
