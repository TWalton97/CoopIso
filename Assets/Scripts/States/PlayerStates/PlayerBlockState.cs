using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    public PlayerBlockState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player._movementSpeed = 1.5f;
        animator.SetBool("Blocking", true);
        animator.CrossFade(Block_Hash, crossFadeDuration, (int)PlayerAnimatorLayers.UpperBody);
    }

    public override void OnExit()
    {
        player._movementSpeed = player._maximumMovementSpeed;
        animator.SetBool("Blocking", false);
    }
}
