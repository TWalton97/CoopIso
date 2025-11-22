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
    private Coroutine attackCoroutine;

    public EnemyAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        //AttackCompleted = false;
        attackCoroutine = enemy.StartCoroutine(WaitForEndOfAttack());
        agent.speed = 0;
        enemy.animationStatusTracker.OnAttackCompleted += CompleteAttack;
    }

    public override void OnExit()
    {
        if (attackCoroutine != null)
        {
            enemy.StopCoroutine(attackCoroutine);
        }
        AttackCompleted = false;
        enemy.animationStatusTracker.OnAttackCompleted -= CompleteAttack;
    }

    private void CompleteAttack()
    {
        AttackCompleted = true;
    }

    public override void Update()
    {
        animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        actualNormalizedTime = Mathf.Clamp01(animatorStateInfo.normalizedTime);

        agent.SetDestination(enemy.transform.position);
        //RotateTowardsTarget(GetRotationTowardsTarget());
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
        //animator.CrossFade(IdleHash, crossFadeDuration);
        while (!CheckAngleToAttacker(enemy.playerDetector.Player.gameObject, 30))
        {
            RotateTowardsTarget(GetRotationTowardsTarget());
            yield return null;
        }
        animator.CrossFade(AttackHash, crossFadeDuration);
        yield return null;
    }

    private bool CheckAngleToAttacker(GameObject attacker, float blockAngle)
    {
        var directionToPlayer = attacker.transform.position - enemy.transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, enemy.transform.forward);

        if (!(angleToPlayer < blockAngle / 2f))
        {
            return false;
        }
        return true;
    }
}
