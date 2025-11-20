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
    protected SpawnedItemDataBase.SpawnedItemData spawnedWeaponData;

    void Awake()
    {
        hitbox = GetComponent<Hitbox>();
    }

    public void Init(WeaponHand weaponHand, string itemID)
    {
        this.weaponHand = weaponHand;
        this.itemID = itemID;

        spawnedWeaponData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(itemID);// as SpawnedItemDataBase.SpawnedWeaponsData;
        
        if (spawnedWeaponData.GetType() == typeof(SpawnedItemDataBase.SpawnedWeaponsData))
        {
            spawnedWeaponData = spawnedWeaponData as SpawnedItemDataBase.SpawnedWeaponsData;
        }
        else if (spawnedWeaponData.GetType() == typeof(SpawnedItemDataBase.SpawnedBowData))
        {
            spawnedWeaponData = spawnedWeaponData as SpawnedItemDataBase.SpawnedBowData;
        }
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

    public virtual void ActivateHitbox()
    {
        if (spawnedWeaponData.GetType() == typeof(SpawnedItemDataBase.SpawnedWeaponsData))
        {
            SpawnedItemDataBase.SpawnedWeaponsData weaponData = spawnedWeaponData as SpawnedItemDataBase.SpawnedWeaponsData;
            int rolledDamage = UnityEngine.Random.Range(weaponData.weaponMinDamage, weaponData.weaponMaxDamage);
            hitbox.ActivateHitbox(rolledDamage);
        }
    }
}


