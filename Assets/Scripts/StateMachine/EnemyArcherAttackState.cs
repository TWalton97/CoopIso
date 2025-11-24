using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArcherAttackState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public bool AttackCompleted;

    AnimatorStateInfo animatorStateInfo;
    float actualNormalizedTime;

    private Coroutine attackCoroutine;

    public EnemyArcherAttackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        agent.speed = 0;
        attackCoroutine = enemy.StartCoroutine(WaitForEndOfAttack());
    }

    public override void OnExit()
    {
        enemy.StartCoroutine(enemy.AttackCooldown());
        if (attackCoroutine != null)
        {
            enemy.StopCoroutine(attackCoroutine);
        }
        AttackCompleted = false;
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
        while (!CheckAngleToAttacker(enemy.playerDetector.Player.gameObject, 10f))
        {
            RotateTowardsTarget(GetRotationTowardsTarget());
            yield return null;
        }
        agent.transform.rotation = GetRotationTowardsTarget();
        yield return null;
        animator.CrossFade(AttackHash, crossFadeDuration, 1);
        AttackCompleted = false;

        yield return new WaitForSeconds(0.2f);

        while (actualNormalizedTime < 0.99f)
        {
            yield return null;
        }
        attackCoroutine = null;
        AttackCompleted = true;
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
