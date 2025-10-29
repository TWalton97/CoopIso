using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public WeaponDataSO Data { get; private set; }
    public event Action OnEnter;
    public event Action OnExit;
    private int currentAttackCounter;
    private Animator animator;
    public RuntimeAnimatorController animatorOverrideController;
    public AnimationEventHandler EventHandler { get; private set; }
    public NewPlayerController newPlayerController { get; private set; }
    private Hitbox hitbox;
    public float hitboxActivationDelay;
    public int damage;
    public bool active = false;

    public int attackHash;

    private readonly float startingMovementSpeed;

    public AttackAnimationHash attackAnimationHash;
    public enum AttackAnimationHash
    {
        Attack1H,
        Block,
        MainHandAttack2H
    }

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
        SetAttackAnimationHash();
    }
    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        hitbox = GetComponent<Hitbox>();
        EventHandler = GetComponent<AnimationEventHandler>();
    }

    public void Enter(Action endAction)
    {
        newPlayerController.animator.CrossFade(attackHash, 0.2f, (int)PlayerAnimatorLayers.UpperBody);
        StartCoroutine(ActivateHitbox(endAction));

        OnEnter?.Invoke();
    }

    private void Exit()
    {
        newPlayerController._movementSpeed = newPlayerController._maximumMovementSpeed;

        OnExit?.Invoke();
    }

    public void SetPlayer(NewPlayerController controller)
    {
        newPlayerController = controller;
    }

    private IEnumerator ActivateHitbox(Action endAction)
    {
        float elapsedTime = 0f;
        elapsedTime += Time.deltaTime;
        while (elapsedTime < hitboxActivationDelay)
        {
            elapsedTime += Time.deltaTime;
            newPlayerController._movementSpeed = Mathf.Lerp(newPlayerController._maximumMovementSpeed, Data.MovementSpeedDuringAttack, elapsedTime / hitboxActivationDelay);
            yield return null;
        }
        hitbox.ActivateHitbox(damage);
        yield return new WaitForSeconds(0.9f - hitboxActivationDelay);
        endAction?.Invoke();
        Exit();
        yield return null;
    }

    private void SetAttackAnimationHash()
    {
        switch (attackAnimationHash)
        {
            case AttackAnimationHash.Attack1H:
                if (weaponHand == WeaponHand.MainHand)
                {
                    attackHash = PlayerBaseState.MainHandAttack1H_Hash;
                }
                else if (weaponHand == WeaponHand.OffHand)
                {
                    attackHash = PlayerBaseState.OffHandAttack1H_Hash;
                }
                break;

            case AttackAnimationHash.Block:
                attackHash = PlayerBaseState.Block_Hash;
                break;

            case AttackAnimationHash.MainHandAttack2H:
                attackHash = PlayerBaseState.MainHandAttack2H_Hash;
                break;
        }
    }
}
