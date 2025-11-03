using UnityEngine;
using UnityEngine.AI;
public class EnemyDieState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public EnemyDieState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        agent.enabled = false;
        foreach (GameObject obj in enemy.body)
        {
            obj.SetActive(false);
        }
        enemy.ragdoll.SetActive(true);
        enemy.coll.enabled = false;
        //animator.CrossFade(DieHash, crossFadeDuration);
    }

    public override void Update()
    {

    }
}
