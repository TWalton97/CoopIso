using UnityEngine;

public class PlayerAirborneState : PlayerBaseState
{
    private const float EXTRA_FALL_FORCE = 10f;
    public PlayerAirborneState(NewPlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.SetBool("Grounded", false);
    }

    public override void FixedUpdate()
    {
        player.Rigidbody.AddForce(-Vector3.up * EXTRA_FALL_FORCE);
    }
}
