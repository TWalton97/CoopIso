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
    public WeaponRangeType weaponRangeType;
    public ItemData ItemData;

    public WeaponHand weaponHand;
    public enum WeaponHand
    {
        MainHand,
        OffHand
    }

    public NewWeaponController.WeaponAttackTypes weaponAttackType;

    public string itemID;
    protected SpawnedItemDataBase.SpawnedItemData spawnedWeaponData;
    protected int minDamage;
    protected int maxDamage;
    public int averageWeaponDamage;

    void Awake()
    {
        hitbox = GetComponent<Hitbox>();
    }

    public void Init(WeaponHand weaponHand, ItemData itemData)
    {
        ItemData = itemData;
        this.weaponHand = weaponHand;
        this.itemID = itemData.itemID;
        weaponRangeType = itemData.weaponRangeType;

        spawnedWeaponData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemID);// as SpawnedItemDataBase.SpawnedWeaponsData;



        if (itemData.data is WeaponDataSO weaponData)
        {
            minDamage = LootCalculator.CalculateQualityModifiedStat(weaponData.WeaponMinDamage, itemData.itemQuality);
            maxDamage = LootCalculator.CalculateQualityModifiedStat(weaponData.WeaponMaxDamage, itemData.itemQuality);
            averageWeaponDamage = (minDamage + maxDamage) / 2;
        }
        else if (itemData.data is BowSO bowData)
        {
            minDamage = LootCalculator.CalculateQualityModifiedStat(bowData.WeaponMinDamage, itemData.itemQuality);
            maxDamage = LootCalculator.CalculateQualityModifiedStat(bowData.WeaponMaxDamage, itemData.itemQuality);
            averageWeaponDamage = (minDamage + maxDamage) / 2;
        }
    }

    void OnDestroy()
    {
        if (newPlayerController != null)
            newPlayerController.AnimationStatusTracker.OnAttackCompleted -= Exit;
    }

    public virtual void Enter(Action endAction, int attackNum)
    {
        if (weaponAttackType == NewWeaponController.WeaponAttackTypes.OneHanded)
        {
            newPlayerController.Animator.SetFloat("AttackSpeedMultiplier", newPlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.OneHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.TwoHanded && newPlayerController.WeaponController.instantiatedSecondaryWeapon != null)
        {
            newPlayerController.Animator.SetFloat("AttackSpeedMultiplier", newPlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.OneHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.TwoHanded)
        {
            newPlayerController.Animator.SetFloat("AttackSpeedMultiplier", newPlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.TwoHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.Bow)
        {
            newPlayerController.Animator.SetFloat("AttackSpeedMultiplier", newPlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.BowAttack);
        }

        newPlayerController.AnimationStatusTracker.OnAttackCompleted += Exit;
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
        newPlayerController.Animator.SetBool("ShootFromAiming", false);
    }

    public virtual void Exit()
    {
        if (!active) return;
        hitbox.DeactivateHitbox();
        newPlayerController._movementSpeed = newPlayerController._maximumMovementSpeed;
        active = false;
        InvokeOnExit();
    }

    public void SetPlayer(NewPlayerController controller)
    {
        newPlayerController = controller;
    }

    public virtual void ActivateHitbox()
    {
        if (ItemData.data is WeaponDataSO weaponData)
        {
            int rolledDamage = UnityEngine.Random.Range(minDamage, maxDamage);
            hitbox.ActivateHitbox(rolledDamage);
        }
    }
}


