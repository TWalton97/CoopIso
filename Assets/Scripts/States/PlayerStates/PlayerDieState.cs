using UnityEngine;

public class PlayerDieState : PlayerBaseState
{
    protected static readonly int DieHash = Animator.StringToHash("Die");
    public PlayerDieState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.PlayerContext.PlayerInput.actions.Disable();
        player.Rigidbody.velocity = Vector3.zero;
        player.Animator.SetBool("Dead", true);
        player.Animator.CrossFade(DieHash, 0f);
    }

    public override void OnExit()
    {
        player.PlayerContext.PlayerInput.actions.Enable();
    }
}
