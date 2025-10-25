using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshAttackState : BaseState
{
    BaseNavMeshUnitController navMeshController;
    public NavMeshAttackState(BaseUnitController _controller) : base(_controller)
    {
        navMeshController = _controller as BaseNavMeshUnitController;
    }

    public override void OnEnter()
    {
        navMeshController.AttackController.attackSO.Attack(navMeshController.transform, navMeshController.transform.forward);
        navMeshController.Attack();
        navMeshController.CompleteAttack();
    }

}
