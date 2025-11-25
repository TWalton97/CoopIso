using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Vector3 startPoint;
    readonly float wanderRadius;
    float elapsedTime = 0f;
    float waitTime = 2;
    private bool IsWaiting = false;

    public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
    {
        this.agent = agent;
        this.startPoint = enemy.transform.position;
        this.wanderRadius = wanderRadius;
    }

    public override void OnEnter()
    {
        
    }

    public override void Update()
    {
        agent.speed = enemy.wanderSpeed;
        animator.SetFloat("MovementAnimationMultiplier", agent.speed / enemy.StartWanderSpeed);

        if (HasReachedDestination())
        {
            if (!IsWaiting)
            {
                IsWaiting = true;
                //animator.CrossFade(IdleHash, crossFadeDuration);
            }
            elapsedTime += Time.deltaTime;
            if (elapsedTime < waitTime) return;
            elapsedTime = 0;
            waitTime = Random.Range(2, 4);
            //animator.CrossFade(WalkHash, crossFadeDuration);
            IsWaiting = false;
            var randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += startPoint;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
            var finalPosition = hit.position;

            agent.SetDestination(finalPosition);
        }
    }

    bool HasReachedDestination()
    {
        return !agent.pathPending
               && agent.remainingDistance <= agent.stoppingDistance
               && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
    }
}
