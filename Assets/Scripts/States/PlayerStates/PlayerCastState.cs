using UnityEngine;

public class PlayerCastState : PlayerBaseState
{
    public PlayerCastState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.AbilityController.UseAbility1();
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
        player.abilityButtonPressed = false;
    }
}