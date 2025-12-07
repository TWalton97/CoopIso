using UnityEngine;

public class PlayerDieState : PlayerBaseState
{
    protected static readonly int DieHash = Animator.StringToHash("Die");
    public PlayerDieState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.Animator.SetBool("Dead", true);
        player.Animator.CrossFade(DieHash, 0f);
    }

    public override void OnExit()
    {

    }
}
