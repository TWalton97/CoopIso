using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerController : BaseNavMeshUnitController
{
    public float MovementSpeed;

    [SerializeField] private NavMeshIdleState idleState;
    [SerializeField] private NavMeshChaseState chaseState;
    [SerializeField] private NavMeshAttackState attackState;

    public override void Awake()
    {
        base.Awake();

        idleState = new NavMeshIdleState(this);
        chaseState = new NavMeshChaseState(this);
        attackState = new NavMeshAttackState(this);

        At(idleState, chaseState, new FuncPredicate(() => HasTarget()));
        At(chaseState, idleState, new FuncPredicate(() => !HasTarget()));
        At(chaseState, attackState, new FuncPredicate(() => Agent.remainingDistance < 2f && attackCompleted == false));
        At(attackState, idleState, new FuncPredicate(() => attackCompleted = true));

        StateMachine.SetState(idleState);
        UpdateStateName();

        SetNavMeshAgentStats();
    }

    private void SetNavMeshAgentStats()
    {
        Agent.speed = MovementSpeed;
    }
}
