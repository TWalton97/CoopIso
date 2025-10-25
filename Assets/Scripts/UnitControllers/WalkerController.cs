using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerController : BaseNavMeshUnitController
{
    public float MovementSpeed;

    [SerializeField] private NavMeshIdleState idleState;
    [SerializeField] private NavMeshChaseState chaseState;

    public override void Awake()
    {
        base.Awake();

        idleState = new NavMeshIdleState(this);
        chaseState = new NavMeshChaseState(this);

        At(idleState, chaseState, new FuncPredicate(() => HasTarget()));
        At(chaseState, idleState, new FuncPredicate(() => !HasTarget()));

        StateMachine.SetState(idleState);
        UpdateStateName();

        SetNavMeshAgentStats();
    }

    private void SetNavMeshAgentStats()
    {
        Agent.speed = MovementSpeed;
    }
}
