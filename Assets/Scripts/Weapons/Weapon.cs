using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public WeaponDataSO Data { get; private set; }
    public event Action OnEnter;
    public event Action OnExit;
    public NewPlayerController newPlayerController { get; private set; }
    public int damage;
    public bool active = false;

    public WeaponHand weaponHand;
    public enum WeaponHand
    {
        MainHand,
        OffHand
    }

    public NewWeaponController.WeaponAttackTypes weaponAttackType;

    public void Init(WeaponHand weaponHand)
    {
        this.weaponHand = weaponHand;
    }

    void OnDestroy()
    {
        if (newPlayerController != null)
            newPlayerController.AnimationStatusTracker.OnAnimationCompleted -= Exit;
    }

    public virtual void Enter(Action endAction, int attackNum)
    {
        newPlayerController.AnimationStatusTracker.OnAnimationCompleted += Exit;
        active = true;
        newPlayerController.Animator.SetInteger("counter", attackNum);
        newPlayerController.Animator.SetTrigger("Attack");
        InvokeOnEnter();
    }

    public void InvokeOnEnter()
    {
        OnEnter?.Invoke();
    }

    public void InvokeOnExit()
    {
        OnExit?.Invoke();
    }

    public virtual void Exit()
    {
        if (!active) return;
        newPlayerController._movementSpeed = newPlayerController._maximumMovementSpeed;
        active = false;
        InvokeOnExit();
    }

    public void SetPlayer(NewPlayerController controller)
    {
        newPlayerController = controller;
    }
}


