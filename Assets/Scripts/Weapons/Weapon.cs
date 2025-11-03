using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public ItemSO Data { get; private set; }
    public event Action OnEnter;
    public event Action OnExit;
    public NewPlayerController newPlayerController { get; private set; }
    private bool active = false;
    private Hitbox hitbox;

    public WeaponHand weaponHand;
    public enum WeaponHand
    {
        MainHand,
        OffHand
    }

    public NewWeaponController.WeaponAttackTypes weaponAttackType;

    public List<Affix> affixes;
    public string itemID;
    private SpawnedItemDataBase.SpawnedWeaponsData spawnedWeaponData;

    void Awake()
    {
        hitbox = GetComponent<Hitbox>();
    }

    public void Init(WeaponHand weaponHand, string itemID)
    {
        this.weaponHand = weaponHand;
        this.itemID = itemID;
        spawnedWeaponData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(itemID) as SpawnedItemDataBase.SpawnedWeaponsData;
    }

    void OnDestroy()
    {
        if (newPlayerController != null)
            newPlayerController.AnimationStatusTracker.OnAnimationCompleted -= Exit;
    }

    public virtual void Enter(Action endAction, int attackNum)
    {
        if (weaponAttackType == NewWeaponController.WeaponAttackTypes.OneHanded)
        {
            newPlayerController.Animator.SetFloat("AttackSpeedMultiplier", spawnedWeaponData.attacksPerSecond * AnimatorClipLengths.OneHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.TwoHanded)
        {
            newPlayerController.Animator.SetFloat("AttackSpeedMultiplier", spawnedWeaponData.attacksPerSecond * AnimatorClipLengths.TwoHandedAttack);
        }

        newPlayerController.AnimationStatusTracker.OnAnimationCompleted += Exit;
        active = true;
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

    public void ActivateHitbox()
    {
        int rolledDamage = UnityEngine.Random.Range(spawnedWeaponData.weaponMinDamage, spawnedWeaponData.weaponMaxDamage);
        hitbox.ActivateHitbox(rolledDamage);
    }
}


