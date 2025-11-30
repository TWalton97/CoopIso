using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        if (player.WeaponController.instantiatedPrimaryWeapon != null && player.WeaponController.instantiatedPrimaryWeapon.weaponAttackType == NewWeaponController.WeaponAttackTypes.Bow)
        {
            ToggleAimLine(true);
        }
        player.IsBlocking = true;
        player._movementSpeed = 0f;
        animator.SetBool("Blocking", true);
    }

    public override void OnExit()
    {
        player.IsBlocking = false;
        player._movementSpeed = player._maximumMovementSpeed;
        animator.SetBool("Blocking", false);
        ToggleAimLine(false);
    }

    private void ToggleAimLine(bool value)
    {
        player.BowAimLineController.ToggleLineRenderer(value);
    }
}
