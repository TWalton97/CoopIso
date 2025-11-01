using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("entering attack state");
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
        Debug.Log("exiting attack state");
        player.attackButtonPressed = false;
    }
}
