using System.Collections;
using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.WeaponController.Attack(AttackCompleted);
        if (player.WeaponController.instantiatedPrimaryWeapon != null)
        {
            WeaponDataSO weaponData = player.WeaponController.instantiatedPrimaryWeapon.Data as WeaponDataSO;
            player.StartCoroutine(ReduceMovementSpeed(weaponData.MovementSpeedDuringAttack));
            //player._movementSpeed = weaponData.MovementSpeedDuringAttack;
        }
    }

    private void AttackCompleted()
    {
        player._movementSpeed = player._maximumMovementSpeed;
        player.WeaponController.canAttack = true;
        player.attackStateMachine.ChangeState(player.idleState);
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        player.attackButtonPressed = false;
    }

    private IEnumerator ReduceMovementSpeed(float targetSpeed)
    {
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            player._movementSpeed = Mathf.Lerp(player._maximumMovementSpeed, targetSpeed, elapsedTime / 0.5f);
            yield return null;
        }
    }

}
