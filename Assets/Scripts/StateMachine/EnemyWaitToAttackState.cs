using UnityEngine;
using UnityEngine.AI;

public class EnemyWaitToAttackState : EnemyBaseState
{
    private readonly NavMeshAgent agent;
    private EnemyStatsSO enemyStats;
    private Transform player;

    public EnemyWaitToAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        agent.isStopped = true;
        agent.updateRotation = false;
    }

    public override void Update()
    {
        if (enemy.target == null) return;

        // --- Check distance to player ---
        float distance = Vector3.Distance(enemy.transform.position, enemy.target.position);
        if (distance > enemyStats.AttackRange + 0.05f)
        {
            // Outside attack range → go back to chase
            enemy.InAttackRange = false;

            return;
        }

        enemy.InAttackRange = true;

        // --- Rotate toward target ---
        Vector3 direction = (enemy.target.position - enemy.transform.position).normalized;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemyStats.RotationSpeed * Time.deltaTime);
        }

        // --- Check if enemy can attack ---
        float angleToTarget = Vector3.Angle(enemy.transform.forward, direction);
        if (Time.time >= enemy.NextAttackTime && angleToTarget < 1f)
        {
            enemy.CanAttack = true;
        }
    }

    public override void OnExit()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
    }
}