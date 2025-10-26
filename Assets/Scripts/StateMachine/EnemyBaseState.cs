using UnityEngine;
public abstract class EnemyBaseState : IState
{
    protected readonly Enemy enemy;
    protected readonly Animator animator;

    protected static readonly int IdleHash = Animator.StringToHash("Idle");
    protected static readonly int WalkHash = Animator.StringToHash("Walk");
    protected static readonly int RunHash = Animator.StringToHash("Run");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int DieHash = Animator.StringToHash("Die");
    protected static readonly int StaggerHash = Animator.StringToHash("Stagger");

    protected const float crossFadeDuration = 0.1f;

    protected EnemyBaseState(Enemy enemy, Animator animator)
    {
        this.enemy = enemy;
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
