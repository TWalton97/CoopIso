using System;
using UnityEngine;

public class FriendlySkeletonWarrior : Enemy
{
    public NewPlayerController Owner;
    private FriendlyUnitSO friendlyUnitSO;

    public Action<FriendlySkeletonWarrior> OnFriendlyWarriorDied;

    protected override void Start()
    {

    }

    public virtual void Init(NewPlayerController controller)
    {
        Owner = controller;
        friendlyUnitSO = EntityData as FriendlyUnitSO;
        stateMachine = new StateMachine();

        var idleState = new EnemyWanderState(this, animator, agent, wanderRadius);
        var leashState = new FriendlySkeletonLeashState(this, animator, agent, Owner.transform);
        var chaseState = new EnemyChaseState(this, animator, agent);
        var waitToAttackState = new EnemyWaitToAttackState(this, animator, agent, target);
        var attackState = new EnemyAttackState(this, animator, agent, target);
        var deathState = new FriendlySkeletonDieState(this, animator, agent, transform);

        At(idleState, chaseState, new FuncPredicate(() => target != null && OwnerInLeashRange()));
        At(chaseState, idleState, new FuncPredicate(() => target == null));

        At(chaseState, waitToAttackState, new FuncPredicate(() => InAttackRange && OwnerInLeashRange()));
        At(waitToAttackState, chaseState, new FuncPredicate(() => !InAttackRange));
        At(waitToAttackState, idleState, new FuncPredicate(() => target == null));

        At(waitToAttackState, attackState, new FuncPredicate(() => CanAttack && OwnerInLeashRange()));
        At(attackState, waitToAttackState, new FuncPredicate(() => !CanAttack));
        At(attackState, idleState, new FuncPredicate(() => target == null));

        At(idleState, leashState, new FuncPredicate(() => !OwnerInLeashRange()));
        At(chaseState, leashState, new FuncPredicate(() => !OwnerInLeashRange()));
        At(waitToAttackState, leashState, new FuncPredicate(() => !OwnerInLeashRange()));

        At(leashState, idleState, new FuncPredicate(() => OwnerInLeashRange()));

        Any(deathState, new FuncPredicate(() => IsDead));

        stateMachine.SetState(idleState);

        stateMachine.OnStateChanged += UpdateStateName;
    }

    public override void Die()
    {
        base.Die();
        OnFriendlyWarriorDied?.Invoke(this);
    }

    public override void Update()
    {
        if (Owner == null) return;
        base.Update();
    }

    public override void FixedUpdate()
    {
        if (IsDead) return;
        if (Owner == null) return;
        base.FixedUpdate();
    }

    public bool OwnerInLeashRange()
    {
        return Vector3.Distance(transform.position, Owner.transform.position) <= friendlyUnitSO.LeashRange;
    }

    public bool OwnerInTeleportRange()
    {
        return Vector3.Distance(transform.position, Owner.transform.position) <= friendlyUnitSO.TeleportRange;
    }
}
