using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(NewPlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        //animator.CrossFade(RunHash, crossFadeDuration);
    }
}
