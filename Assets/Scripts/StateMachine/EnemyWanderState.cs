using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWanderState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    private Vector3 wanderTarget;
    private EnemyStatsSO enemyStats;

    private bool waiting = false;
    private float waitTime = 0f;
    private float waitDuration = 4f;

    public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
    {
        this.agent = agent;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        if (!agent.isActiveAndEnabled) return;

        waitDuration = Random.Range(4, 8);
        waiting = true;
        wanderTarget = enemy.transform.position;
        agent.isStopped = false;
    }

    public override void Update()
    {
        enemy.FindTargetsInAggroRange();
        if (enemy.damageTable.Count != 0)
        {
            enemy.SetTarget(enemy.damageTable.First().Key);
        }

        if (!agent.isActiveAndEnabled) return;

        if (wanderTarget == Vector3.zero)
        {
            SetNewWanderTarget(enemy);
        }

        agent.speed = enemyStats.WanderSpeed;

        if (waiting)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= waitDuration)
            {
                waiting = false;
                SetNewWanderTarget(enemy);
            }
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            waiting = true;
            waitTime = 0f;
            waitDuration = Random.Range(4, 8);
        }
    }

    private void SetNewWanderTarget(Enemy enemy)
    {
        Vector3 randomPoint = enemy.transform.position + Random.insideUnitSphere * enemyStats.WanderRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, enemyStats.WanderRadius, NavMesh.AllAreas))
        {
            wanderTarget = hit.position;
            agent.SetDestination(wanderTarget);
        }
    }

    public override void OnExit()
    {
        if (!agent.isActiveAndEnabled) return;

        agent.isStopped = true;
        waiting = false;
    }
}
