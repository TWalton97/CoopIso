using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        player.WeaponController.Attack(AttackCompleted);
    }

    private void AttackCompleted()
    {
        player.WeaponController.canAttack = true;
        player.attackStateMachine.ChangeState(player.idleState);
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        Debug.Log("Exiting attack state");
        player.attackButtonPressed = false;
    }
}
