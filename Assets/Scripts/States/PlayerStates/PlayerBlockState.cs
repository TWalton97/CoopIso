using UnityEngine;

public class PlayerBlockState : PlayerBaseState
{
    Weapon weapon;
    public PlayerBlockState(NewPlayerController player, Animator animator, Weapon weapon) : base(player, animator)
    {
        this.weapon = weapon;
    }

    public override void OnEnter()
    {
        animator.CrossFade(BlockHash, crossFadeDuration, (int)PlayerAnimatorLayers.UpperBody);
    }

    public override void OnExit()
    {

    }
}
