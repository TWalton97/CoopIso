using UnityEngine;
using UnityEngine.AI;
public class EnemyDieState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    private float elapsedTime = 0f;
    public EnemyDieState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        agent.enabled = false;
        animator.CrossFade(DieHash, crossFadeDuration);
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 1.5f)
        {
            enemy.Die();
        }
    }



}
