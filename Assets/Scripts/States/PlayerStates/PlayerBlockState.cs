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
        player.HealthController.BlockTime = Time.time;
        player.MovementLocked = true;
        animator.SetBool("Blocking", true);
    }

    public override void OnExit()
    {
        player.MovementLocked = false;
        player.IsBlocking = false;
        animator.SetBool("Blocking", false);
        ToggleAimLine(false);
    }

    private void ToggleAimLine(bool value)
    {
        player.BowAimLineController.ToggleLineRenderer(value);
    }
}
