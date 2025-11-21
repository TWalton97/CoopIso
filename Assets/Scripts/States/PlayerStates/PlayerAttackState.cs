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
        player.WeaponController.Attack(AttackCompleted);
        if (player.WeaponController.instantiatedPrimaryWeapon != null)
        {
            if (player.WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(WeaponDataSO))
            {
                WeaponDataSO weaponData = player.WeaponController.instantiatedPrimaryWeapon.Data as WeaponDataSO;
                //ReduceSpeedCoroutine = player.StartCoroutine(ReduceMovementSpeed(weaponData.MovementSpeedMultiplierDuringAttack));
            }
            else if (player.WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(BowSO))
            {
                BowSO weaponData = player.WeaponController.instantiatedPrimaryWeapon.Data as BowSO;
                //ReduceSpeedCoroutine = player.StartCoroutine(ReduceMovementSpeed(weaponData.MovementSpeedMultiplierDuringAttack));
            }
        }
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

    private IEnumerator ReduceMovementSpeed(float targetSpeed)
    {
        float elapsedTime = 0f;

        while (elapsedTime < (1 / player.PlayerStatsBlackboard.AttacksPerSecond))
        {
            elapsedTime += Time.deltaTime;
            player._movementSpeed = Mathf.Lerp(player._maximumMovementSpeed, player._maximumMovementSpeed * targetSpeed, elapsedTime / 0.5f);
            yield return null;
        }
    }

}
