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
        if (player.attackStateMachine.previous.State == player.blockState && player.WeaponController.instantiatedPrimaryWeapon.weaponRangeType == WeaponRangeType.Ranged)
        {
            player.WeaponController.Attack(AttackCompleted, true);
            return;
        }

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
        player.Animator.SetBool("ShootFromAiming", false);

        player.attackButtonPressed = false;
        if (ReduceSpeedCoroutine != null)
        {
            player.StopCoroutine(ReduceSpeedCoroutine);
        }
        player.MovementLocked = false;
    }
}