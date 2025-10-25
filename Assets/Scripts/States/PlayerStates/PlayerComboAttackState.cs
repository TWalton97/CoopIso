using UnityEngine;
public class PlayerComboAttackState : PlayerBaseState
{
    public PlayerComboAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFade(AttackHash2, crossFadeDuration);
    }
}
