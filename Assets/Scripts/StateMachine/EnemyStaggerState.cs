using UnityEngine;
using UnityEngine.AI;
public class EnemyStaggerState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;

    private float elapsedTime = 0f;
    private const float staggerDuration = 0.7f;

    public EnemyStaggerState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        animator.CrossFade(StaggerHash, crossFadeDuration);
        elapsedTime = staggerDuration;
    }

    public override void Update()
    {
        if (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            if (elapsedTime <= 0)
            {
                enemy.IsStaggered = false;
            }
        }
    }
}
