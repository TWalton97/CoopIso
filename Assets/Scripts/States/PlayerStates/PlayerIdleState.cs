using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    //State does nothing and waits to transition to either attack or block state
    public PlayerIdleState(NewPlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        
    }
}
