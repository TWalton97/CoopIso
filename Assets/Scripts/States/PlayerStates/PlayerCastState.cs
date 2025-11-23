using UnityEngine;

public class PlayerCastState : PlayerBaseState
{
    public PlayerCastState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        PlayerUserInterfaceController controller = InventoryManager.Instance.GetPlayerUserInterfaceControllerByIndex(player.PlayerInputController.playerIndex);
        AbilityScrollController.AbilityData selectedAbility = controller.AbilityScrollController.ActiveAbility;
        player.AbilityController.UseAbility(selectedAbility);
        player._movementSpeed = player._maximumMovementSpeed * selectedAbility.AbilitySO.MovementSpeedMultiplierWhileUsing;
        player.AnimationStatusTracker.OnAbilityCompleted += AbilityCompleted;
    }

    private void AbilityCompleted()
    {
        player.attackStateMachine.ChangeState(player.idleState);
        player.AnimationStatusTracker.OnAbilityCompleted -= AbilityCompleted;
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        player._movementSpeed = player._maximumMovementSpeed;
        player.abilityButtonPressed = false;
    }
}