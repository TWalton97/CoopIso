using UnityEngine;
using UnityEngine.AI;
public class EnemyChaseState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;

    public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
    }

    public override void Update()
    {
        agent.speed = enemy.chaseSpeed;
        animator.SetFloat("MovementAnimationMultiplier", agent.speed / enemy.StartChaseSpeed);

        if (enemy.playerDetector.CanAttackPlayer())
        {
            agent.SetDestination(enemy.transform.position);
        }
        else
        {
            agent.SetDestination(enemy.playerDetector.Player.position);
        }

    }
}
