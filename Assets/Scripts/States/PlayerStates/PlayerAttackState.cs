using System.Collections;
using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    private Coroutine ReduceSpeedCoroutine;
    public PlayerAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.RotateToFaceLookPoint();
        player.MovementLocked = true;

        player.WeaponController.Attack(AttackCompleted);
    }

    private void AttackCompleted()
    {
        player.WeaponController.canAttack = true;
        player.attackStateMachine.ChangeState(player.idleState);
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        player.attackButtonPressed = false;
        if (ReduceSpeedCoroutine != null)
        {
            player.StopCoroutine(ReduceSpeedCoroutine);
        }
        player.MovementLocked = false;
    }
}