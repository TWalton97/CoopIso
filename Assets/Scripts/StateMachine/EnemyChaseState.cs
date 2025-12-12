using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyBaseState
{
    private readonly NavMeshAgent agent;
    private EnemyStatsSO enemyStats;

    private AttackSlotManager slotManager;
    private int assignedSlot = -1;

    private float repulsionStrength = 0.1f;

    public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
        enemyStats = enemy.EntityData as EnemyStatsSO;
    }

    public override void OnEnter()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
        slotManager = enemy.transform.GetComponent<AttackSlotManager>();
    }

    public override void Update()
    {
        agent.speed = enemy.chaseSpeed;
        enemy.FindTargetsInAggroRange();
        enemy.UpdateTarget();

        if (enemy.target == null) return;

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.target.GetComponent<Collider>().ClosestPoint(enemy.transform.position));

        // If outside personal attack range, approach player
        if (distanceToPlayer > enemyStats.AttackRange)
        {
            agent.SetDestination(enemy.target.transform.position);
            enemy.InAttackRange = false;
            assignedSlot = -1; // reset slot when out of range
            return;
        }

        enemy.InAttackRange = true;

        if (assignedSlot == -1 && slotManager != null)
        {
            assignedSlot = slotManager.RequestSlotDynamic(enemy.transform.position);
        }

        // --- Move toward slot with small repulsion from neighbors ---
        if (assignedSlot != -1)
        {
            Vector3 slotPos = slotManager.GetSlotPosition(assignedSlot);

            Vector3 repulsion = Vector3.zero;
            Collider[] neighbors = Physics.OverlapSphere(enemy.transform.position, 0.5f);
            foreach (var col in neighbors)
            {
                if (col.transform == enemy.transform) continue;
                Enemy other = col.GetComponent<Enemy>();
                if (other != null)
                    repulsion += (enemy.transform.position - other.transform.position).normalized * repulsionStrength;
            }

            agent.SetDestination(slotPos + repulsion);
        }
    }

    public override void OnExit()
    {
        if (assignedSlot != -1 && slotManager != null)
        {
            slotManager.ReleaseSlot(assignedSlot);
            assignedSlot = -1;
        }
        agent.isStopped = true;
    }
}