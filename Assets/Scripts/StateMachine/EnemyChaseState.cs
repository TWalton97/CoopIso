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
        agent.speed = enemy.chaseSpeed;
        animator.CrossFade(RunHash, crossFadeDuration);
    }

    public override void Update()
    {
        agent.SetDestination(enemy.playerDetector.Player.position);
    }
}
