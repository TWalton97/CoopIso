using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public bool AttackCompleted;

    AnimatorStateInfo animatorStateInfo;
    float actualNormalizedTime;

    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        enemy.StartCoroutine(WaitForEndOfAttack());
    }

    public override void OnExit()
    {
        AttackCompleted = false;
    }

    public override void Update()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        actualNormalizedTime = Mathf.Clamp01(animatorStateInfo.normalizedTime);

        agent.SetDestination(enemy.transform.position);
        RotateTowardsTarget(GetRotationTowardsTarget());
    }

    private Quaternion GetRotationTowardsTarget()
    {
        Vector3 DirToTarget = enemy.playerDetector.Player.transform.position - agent.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(DirToTarget, Vector3.up);
        targetRotation.x = 0;
        targetRotation.z = 0;
        return targetRotation;
    }

    private void RotateTowardsTarget(Quaternion targetRotation)
    {
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, enemy.rotationSpeed * Time.deltaTime);
    }

    private IEnumerator WaitForEndOfAttack()
    {
        yield return null;
        animator.CrossFade(AttackHash, crossFadeDuration);
        AttackCompleted = false;

        yield return new WaitForSeconds(0.3f);

        while (actualNormalizedTime < 0.99f)
        {
            yield return null;
        }
        AttackCompleted = true;
    }
}
