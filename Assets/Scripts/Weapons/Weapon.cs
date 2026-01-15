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

    public virtual void ActivateHitbox(int comboNumber)
    {
        if (ItemData.ItemSO is WeaponSO weaponData)
        {
            int rolledDamage = UnityEngine.Random.Range(minDamage, maxDamage) + PlayerContext.PlayerController.PlayerStatsBlackboard.GetBonusDamage(weaponAttackType);
            hitbox.ActivateHitbox((int)(rolledDamage * GetComboDamageMultiplier(comboNumber - 1)));
        }
    }

    private float GetComboDamageMultiplier(int comboIndex)
    {
        switch (weaponAttackType)
        {
            case NewWeaponController.WeaponAttackTypes.OneHanded:
                return GetOneHandedMultiplier(comboIndex);

            case NewWeaponController.WeaponAttackTypes.TwoHanded:
                return GetTwoHandedMultiplier(comboIndex);

            case NewWeaponController.WeaponAttackTypes.DualWield:
                return GetDualWieldMultiplier(comboIndex);

            case NewWeaponController.WeaponAttackTypes.Bow:
                return 1f;

            default:
                return 1f;
        }
    }

    private float GetOneHandedMultiplier(int comboIndex)
    {
        switch (comboIndex)
        {
            case 0: return 1.0f;
            case 1: return 1.0f;
            case 2: return 1.1f;
            default: return 1.1f;
        }
    }

    private float GetTwoHandedMultiplier(int comboIndex)
    {
        switch (comboIndex)
        {
            case 0: return 0.9f;
            case 1: return 1.4f;
            case 2: return 1.8f;
            default: return 1.35f;
        }
    }

    private float GetDualWieldMultiplier(int comboIndex)
    {
        switch (comboIndex)
        {
            case 0: return 0.75f;
            case 1: return 0.75f;
            case 2: return 0.9f;
            default: return 0.9f;
        }
    }
}


