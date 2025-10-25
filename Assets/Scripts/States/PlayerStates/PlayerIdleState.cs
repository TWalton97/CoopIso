using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(NewPlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(IdleHash, crossFadeDuration);
    }
}
