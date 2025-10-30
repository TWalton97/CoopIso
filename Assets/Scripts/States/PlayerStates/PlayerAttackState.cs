using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(NewPlayerController player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        Debug.Log("Enter attack state");

        player.weaponController.Attack(AttackCompleted);
    }

    private void AttackCompleted()
    {
        player.weaponController.canAttack = true;
        player.attackStateMachine.ChangeState(player.idleState);
    }

    public override void Update()
    {

    }

    public override void OnExit()
    {
        player.attackButtonPressed = false;
        Debug.Log("Exit attack state");
    }
}
