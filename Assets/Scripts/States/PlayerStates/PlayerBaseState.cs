using UnityEngine;
public class PlayerBaseState : IState
{
    protected readonly NewPlayerController player;
    protected readonly Animator animator;

    public static readonly int Block_Hash = Animator.StringToHash("Block");

    protected const float crossFadeDuration = 0.1f;

    protected PlayerBaseState(NewPlayerController player, Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnExit()
    {

    }
}
