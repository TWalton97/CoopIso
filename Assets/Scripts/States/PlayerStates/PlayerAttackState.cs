using UnityEngine;
using Utilities;

public class PlayerAttackState : PlayerBaseState
{
    Weapon weapon;
    private float startingSpeed;
    public PlayerAttackState(NewPlayerController player, Animator animator, Weapon weapon) : base(player, animator)
    {
        this.weapon = weapon;
    }

    public override void OnEnter()
    {
        weapon.Enter();
        startingSpeed = player._movementSpeed;
        //animator.CrossFade(AttackHash, crossFadeDuration, (int)PlayerAnimatorLayers.UpperBody);
    }

    public override void Update()
    {
    }

    public override void OnExit()
    {
        player._movementSpeed = player._maximumMovementSpeed;
    }
}
