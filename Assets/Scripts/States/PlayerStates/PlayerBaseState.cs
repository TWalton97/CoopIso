using UnityEngine;
public class PlayerBaseState : IState
{
    protected readonly NewPlayerController player;
    protected readonly Animator animator;

    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int WalkHash = Animator.StringToHash("Walk");
    protected static readonly int RunHash = Animator.StringToHash("Run");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int AttackHash2 = Animator.StringToHash("Attack2");
    protected static readonly int AttackHash3 = Animator.StringToHash("Attack3");

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
