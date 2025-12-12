using UnityEngine;
using UnityEngine.AI;

public class EnemyWaitToAttackState : EnemyBaseState
{
    private readonly NavMeshAgent agent;
    private EnemyStatsSO enemyStats;
    private Entity target;

    public EnemyWaitToAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Entity target) : base(enemy, animator)
    {
        this.agent = agent;
        this.target = target;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        agent.isStopped = true;
        agent.updateRotation = false;
    }

    public override void Update()
    {
        if (enemy.target != null && enemy.target.IsDead)
            enemy.target = null;

        enemy.UpdateTarget();

        if (enemy.target == null) return;

        // --- Check distance to player ---
        float distance = Vector3.Distance(enemy.transform.position, enemy.target.GetComponent<Collider>().ClosestPoint(enemy.transform.position));
        if (distance > enemyStats.AttackRange + 0.05f)
        {
            // Outside attack range → go back to chase
            enemy.InAttackRange = false;

            return;
        }

        enemy.InAttackRange = true;

        // --- Rotate toward target ---
        Vector3 direction = (enemy.target.transform.position - enemy.transform.position).normalized;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.5f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, enemyStats.RotationSpeed * Time.deltaTime);
        }

        // --- Check if enemy can attack ---
        float angleToTarget = Vector3.Angle(enemy.transform.forward, direction);
        if (Time.time >= enemy.NextAttackTime && angleToTarget < 3f)
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