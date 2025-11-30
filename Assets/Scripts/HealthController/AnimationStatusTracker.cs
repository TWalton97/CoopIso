using System;
using UnityEngine;

public class AnimationStatusTracker : MonoBehaviour
{
    public Transform PlayerModel;
    public Action OnAttackCompleted;
    public Action OnAbilityCompleted;

    public void AttackAnimationCompleted()
    {
        OnAttackCompleted?.Invoke();
    }

    public void AbilityAnimationCompleted()
    {
        OnAbilityCompleted?.Invoke();
    }
}
