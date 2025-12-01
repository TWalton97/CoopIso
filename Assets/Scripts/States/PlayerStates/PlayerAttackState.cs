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

        if (player.WeaponController.instantiatedPrimaryWeapon != null)
        {
            if (player.WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(WeaponDataSO))
            {
                WeaponDataSO weaponData = player.WeaponController.instantiatedPrimaryWeapon.Data as WeaponDataSO;
            }
            else if (player.WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(BowSO))
            {
                BowSO weaponData = player.WeaponController.instantiatedPrimaryWeapon.Data as BowSO;
                if (player.blockButtonPressed)
                {
                    player.WeaponController.Attack(AttackCompleted, true);
                    player._movementSpeed = 0;
                    return;
                }
            }
        }
        player.WeaponController.Attack(AttackCompleted);
        player._movementSpeed = 0;
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
        player._movementSpeed = player._maximumMovementSpeed;
    }
}