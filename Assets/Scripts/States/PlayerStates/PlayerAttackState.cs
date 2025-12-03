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

        if (player.WeaponController.instantiatedPrimaryWeapon != null)
        {
            if (player.WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(WeaponSO))
            {
                WeaponSO weaponData = player.WeaponController.instantiatedPrimaryWeapon.Data as WeaponSO;
            }
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
        player.attackButtonPressed = false;
        if (ReduceSpeedCoroutine != null)
        {
            player.StopCoroutine(ReduceSpeedCoroutine);
        }
        player.MovementLocked = false;
    }
}