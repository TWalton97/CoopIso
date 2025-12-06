using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    [field: SerializeField] public ItemSO Data;
    public event Action OnEnter;
    public event Action OnExit;
    public PlayerContext PlayerContext;
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

    public void Init(PlayerContext playerContext, WeaponHand weaponHand, ItemData itemData)
    {
        PlayerContext = playerContext;
        ItemData = itemData;
        this.weaponHand = weaponHand;
        this.itemID = itemData.ItemID;
        weaponRangeType = itemData.WeaponRangeType;

        spawnedWeaponData = playerContext.PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemID);// as SpawnedItemDataBase.SpawnedWeaponsData;



        if (itemData.ItemSO is WeaponSO weaponData)
        {
            minDamage = itemData.MinDamage;
            maxDamage = itemData.MaxDamage;
            averageWeaponDamage = (minDamage + maxDamage) / 2;
        }
    }

    void OnDestroy()
    {
        if (PlayerContext.PlayerController != null)
            PlayerContext.PlayerController.AnimationStatusTracker.OnAttackCompleted -= Exit;
    }

    public virtual void Enter(Action endAction, int attackNum)
    {
        if (weaponAttackType == NewWeaponController.WeaponAttackTypes.OneHanded)
        {
            PlayerContext.PlayerController.Animator.SetFloat("AttackSpeedMultiplier", PlayerContext.PlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.OneHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.TwoHanded && PlayerContext.PlayerController.WeaponController.instantiatedSecondaryWeapon != null)
        {
            PlayerContext.PlayerController.Animator.SetFloat("AttackSpeedMultiplier", PlayerContext.PlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.OneHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.TwoHanded)
        {
            PlayerContext.PlayerController.Animator.SetFloat("AttackSpeedMultiplier", PlayerContext.PlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.TwoHandedAttack);
        }
        else if (weaponAttackType == NewWeaponController.WeaponAttackTypes.Bow)
        {
            PlayerContext.PlayerController.Animator.SetFloat("AttackSpeedMultiplier", PlayerContext.PlayerController.PlayerStatsBlackboard.AttacksPerSecond * AnimatorClipLengths.BowAttack);
        }

        PlayerContext.PlayerController.AnimationStatusTracker.OnAttackCompleted += Exit;
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
        PlayerContext.PlayerController.Animator.SetBool("ShootFromAiming", false);
    }

    public virtual void Exit()
    {
        if (!active) return;
        PlayerContext.PlayerController._movementSpeed = PlayerContext.PlayerController._maximumMovementSpeed;
        active = false;
        InvokeOnExit();
    }

    public virtual void ActivateHitbox()
    {
        if (ItemData.ItemSO is WeaponSO weaponData)
        {
            int rolledDamage = UnityEngine.Random.Range(minDamage, maxDamage);
            hitbox.ActivateHitbox(rolledDamage);
        }
    }
}


