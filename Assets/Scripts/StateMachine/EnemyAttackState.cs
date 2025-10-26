using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public bool AttackCompleted;

    AnimatorStateInfo animatorStateInfo;

    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        AttackCompleted = false;
        animator.CrossFade(AttackHash, crossFadeDuration);
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (animatorStateInfo.IsName("Attack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            AttackCompleted = true;
        }

        agent.SetDestination(enemy.transform.position);
        RotateTowardsTarget(GetRotationTowardsTarget());
    }

    private Quaternion GetRotationTowardsTarget()
    {
        Vector3 DirToTarget = player.transform.position - agent.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(DirToTarget, Vector3.up);
        targetRotation.x = 0;
        targetRotation.z = 0;
        return targetRotation;
    }

    private void RotateTowardsTarget(Quaternion targetRotation)
    {
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
    }
}
