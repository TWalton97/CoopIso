using UnityEngine;
using System;

[Serializable]
public abstract class WeaponComponent : MonoBehaviour
{
    protected Weapon weapon;

    //protected AnimationEventHandler EventHandler => weapon.EventHandler;
    protected AnimationEventHandler EventHandler;
    protected NewPlayerController playerController => weapon.newPlayerController;

    protected bool isAttackActive;

    protected virtual void Awake()
    {
        weapon = GetComponent<Weapon>();
        EventHandler = GetComponent<AnimationEventHandler>();
    }

    protected virtual void HandleEnter()
    {
        isAttackActive = true;
    }

    protected virtual void HandleExit()
    {
        isAttackActive = false;
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }
}
