using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

public class NewWeaponController : MonoBehaviour
{
    public NewPlayerController newPlayerController;
    public Animator animator;

    public Transform mainHandTransform;
    public Transform offHandTransform;

    public Action OnWeaponUpdated;

    public Weapon instantiatedPrimaryWeapon;
    public Weapon instantiatedSecondaryWeapon;
    public bool primaryWeaponAttackCompleted = false;

    private CountdownTimer comboCounter;    //We use this to determine when to reset our current attack combo
    private float comboPeriod = 3;
    int numAttacks;

    public bool canAttack = true;
    public bool HasShieldEquipped;
    private Action OnActionCompleted;

    private bool StarterWeaponsEquipped = false;

    public AttackProfile oneHandedAttackProfile;
    public AttackProfile twoHandedAttackProfile;
    public AttackProfile dualWieldAttackProfile;
    public AttackProfile bowAttackProfile;

    public float CombinedWeaponDamage;

    protected void Awake()
    {
        comboCounter = new CountdownTimer(comboPeriod);
    }

    void Start()
    {
        newPlayerController.AnimationStatusTracker.OnAttackCompleted += ResetAttack;
    }

    void OnEnable()
    {
        comboCounter.OnTimerStop += () => numAttacks = 0;
    }

    void OnDisable()
    {
        newPlayerController.AnimationStatusTracker.OnAttackCompleted -= ResetAttack;
        comboCounter.OnTimerStop -= () => numAttacks = 0;
    }

    void Update()
    {
        comboCounter.Tick(Time.deltaTime);
    }

    public void EquipStarterItems(Item mainHand, Item offHand)
    {
        if (StarterWeaponsEquipped) return;
        StarterWeaponsEquipped = true;

        if (mainHand != null)
        {
            Item starterSword = Instantiate(mainHand);
            starterSword.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterSword.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.MainHand)[0].EquipGear(starterSword.itemData, newPlayerController);
            Destroy(starterSword.gameObject);
        }

        if (offHand != null)
        {
            Item starterShield = Instantiate(offHand);
            starterShield.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterShield.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.OffHand)[0].EquipGear(starterShield.itemData, newPlayerController);
            Destroy(starterShield.gameObject);
        }
    }

    public void Attack(Action OnActionCompleted)
    {
        this.OnActionCompleted = OnActionCompleted;

        if (!canAttack)
        {
            OnActionCompleted?.Invoke();
            return;
        }

        if (instantiatedPrimaryWeapon == null)
        {
            newPlayerController.Animator.SetInteger("counter", numAttacks);
            newPlayerController.Animator.SetTrigger("Attack");
            numAttacks++;
            canAttack = false;
            comboCounter.Start();
            if (numAttacks >= 2)
            {
                numAttacks = 0;
            }
            return;
        }

        instantiatedPrimaryWeapon.Enter(OnActionCompleted, numAttacks);
        newPlayerController.Animator.SetInteger("counter", numAttacks);
        newPlayerController.Animator.SetTrigger("Attack");
        numAttacks++;
        canAttack = false;
        comboCounter.Start();

        if (instantiatedPrimaryWeapon.Data.GetType() == typeof(WeaponDataSO))
        {
            WeaponDataSO weaponData = instantiatedPrimaryWeapon.Data as WeaponDataSO;
            if (numAttacks >= weaponData.NumberOfAttacksInCombo)
            {
                numAttacks = 0;
            }
        }
        else if (instantiatedPrimaryWeapon.Data.GetType() == typeof(BowSO))
        {
            BowSO weaponData = instantiatedPrimaryWeapon.Data as BowSO;
            if (numAttacks >= weaponData.NumberOfAttacksInCombo)
            {
                numAttacks = 0;
            }
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        OnActionCompleted.Invoke();
    }

    public void EquipOneHandedWeapon(GameObject weaponPrefab, ItemData itemData)
    {
        if (instantiatedPrimaryWeapon == null)
        {
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponPrefab);
            UpdateAnimator();
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded && !newPlayerController.PlayerStatsBlackboard.TwoHandedMastery)
        {
            UnequipWeapon(Weapon.WeaponHand.MainHand);
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponPrefab);
            UpdateAnimator();
        }
        else if (instantiatedSecondaryWeapon == null)
        {
            instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
            instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
            instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand, itemData);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.OffHand, weaponPrefab);
            UpdateAnimator();
        }
        else
        {
            UnequipWeapon(Weapon.WeaponHand.MainHand);
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponPrefab);
            UpdateAnimator();
        }
        CalculateWeaponDamage();
        OnWeaponUpdated?.Invoke();
    }

    public void EquipTwoHandedWeapon(GameObject weaponPrefab, ItemData itemData)
    {
        //Unequip both existing weapons if they exist
        UnequipWeapon(Weapon.WeaponHand.MainHand);
        UnequipWeapon(Weapon.WeaponHand.OffHand);

        //Create the object in the player's main hand
        instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
        instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
        instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData);
        newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponPrefab);
        UpdateAnimator();
        CalculateWeaponDamage();
        OnWeaponUpdated?.Invoke();
    }

    public void EquipOffhand(GameObject weaponPrefab, ItemData itemData)
    {
        UnequipWeapon(Weapon.WeaponHand.OffHand);
        instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
        instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
        instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand, itemData);
        if (instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.Shield)
        {
            SpawnedItemDataBase.SpawnedShieldData spawnedShieldData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedSecondaryWeapon.itemID) as SpawnedItemDataBase.SpawnedShieldData;
            newPlayerController.PlayerHealthController.UpdateArmorAmount(spawnedShieldData.armorAmount);
            HasShieldEquipped = true;
        }
        newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.OffHand, weaponPrefab);
        UpdateAnimator();
        CalculateWeaponDamage();
        OnWeaponUpdated?.Invoke();
    }

    public void UpdateAnimator()
    {
        primaryWeaponAttackCompleted = false;
        canAttack = true;

        if (instantiatedPrimaryWeapon == null)
        {
            //animator.runtimeAnimatorController = UnarmedAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && HasShieldEquipped)
        {
            ApplyAttackProfile(oneHandedAttackProfile);
            //animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && instantiatedSecondaryWeapon == null)
        {
            ApplyAttackProfile(oneHandedAttackProfile);
            //animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
        {
            ApplyAttackProfile(dualWieldAttackProfile);
            //animator.runtimeAnimatorController = DualWieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded && instantiatedSecondaryWeapon == null)
        {
            ApplyAttackProfile(twoHandedAttackProfile);
            //animator.runtimeAnimatorController = TwoHandedAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded && instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            ApplyAttackProfile(dualWieldAttackProfile);
            //animator.runtimeAnimatorController = DualWieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.Bow)
        {
            ApplyAttackProfile(bowAttackProfile);
            //animator.runtimeAnimatorController = BowAnimator;
        }
        CheckForAppropriateMastery();
        OnWeaponUpdated?.Invoke();
    }

    public void CheckForAppropriateMastery()
    {
        if (instantiatedPrimaryWeapon == null && instantiatedSecondaryWeapon == null)
        {
            if (newPlayerController.PlayerStatsBlackboard.UnarmedMastery)
            {
                animator.SetBool("WeaponMastery", true);
                return;
            }
        }
        else if (instantiatedPrimaryWeapon == null && instantiatedSecondaryWeapon != null)
        {
            return;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && (instantiatedSecondaryWeapon == null || HasShieldEquipped))
        {
            if (newPlayerController.PlayerStatsBlackboard.OneHandedMastery)
            {
                animator.SetBool("WeaponMastery", true);
                return;
            }
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded && instantiatedSecondaryWeapon == null)
        {
            // if (newPlayerController.PlayerStatsBlackboard.TwoHandedMastery)
            // {
            //     animator.SetBool("WeaponMastery", true);
            //     return;
            // }
        }
        else if (instantiatedPrimaryWeapon != null)
        {
            if (newPlayerController.PlayerStatsBlackboard.DualWieldMastery && instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
            {
                animator.SetBool("WeaponMastery", true);
                return;
            }
            else if (newPlayerController.PlayerStatsBlackboard.TwoHandedMastery && instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
            {
                animator.SetBool("WeaponMastery", true);
                return;
            }
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.Bow)
        {
            if (newPlayerController.PlayerStatsBlackboard.BowMastery)
            {
                animator.SetBool("WeaponMastery", true);
                return;
            }
        }

        animator.SetBool("WeaponMastery", false);

    }

    public void UnequipWeapon(Weapon.WeaponHand weaponHand)
    {
        switch (weaponHand)
        {
            case Weapon.WeaponHand.MainHand:
                if (instantiatedPrimaryWeapon != null)
                {
                    newPlayerController.PlayerContext.PlayerPreviewManager.UnequipWeaponFromPlayer(newPlayerController.PlayerInputController.playerIndex, Weapon.WeaponHand.MainHand);
                    Destroy(instantiatedPrimaryWeapon.gameObject);
                    instantiatedPrimaryWeapon = null;
                }
                break;
            case Weapon.WeaponHand.OffHand:
                if (instantiatedSecondaryWeapon != null)
                {
                    newPlayerController.PlayerContext.PlayerPreviewManager.UnequipWeaponFromPlayer(newPlayerController.PlayerInputController.playerIndex, Weapon.WeaponHand.OffHand);
                    if (HasShieldEquipped)
                    {
                        SpawnedItemDataBase.SpawnedShieldData spawnedShieldData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedSecondaryWeapon.itemID) as SpawnedItemDataBase.SpawnedShieldData;
                        newPlayerController.PlayerHealthController.UpdateArmorAmount(-spawnedShieldData.armorAmount);
                    }
                    Destroy(instantiatedSecondaryWeapon.gameObject);
                    HasShieldEquipped = false;
                    instantiatedSecondaryWeapon = null;
                }
                break;
        }
        CalculateWeaponDamage();
    }

    public void ApplyAttackProfile(AttackProfile attackProfile)
    {
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("PunchLeft", attackProfile.attack1);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("PunchRight", attackProfile.attack2);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("PunchLeft", attackProfile.attack3);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("SpinAttack_TwoWeapons", attackProfile.comboAttack);
    }

    private void CalculateWeaponDamage()
    {
        //If the player only has one weapon, we just take the average damage of it
        //If the player has two, we take 70% of each one and then average it
        int primaryWeaponDamage;
        int secondaryWeaponDamage;

        if (instantiatedPrimaryWeapon == null)
        {
            primaryWeaponDamage = 0;
        }
        else
        {
            primaryWeaponDamage = instantiatedPrimaryWeapon.averageWeaponDamage;
        }

        if (instantiatedSecondaryWeapon == null || HasShieldEquipped)
        {
            secondaryWeaponDamage = 0;
        }
        else
        {
            secondaryWeaponDamage = instantiatedSecondaryWeapon.averageWeaponDamage;
        }

        if (primaryWeaponDamage == 0)
        {
            CombinedWeaponDamage = 0;
        }
        else if (secondaryWeaponDamage == 0)
        {
            CombinedWeaponDamage = primaryWeaponDamage;
        }
        else
        {
            CombinedWeaponDamage = (primaryWeaponDamage * 0.8f) + (secondaryWeaponDamage * 0.8f);
        }
    }

    public RuntimeAnimatorController UnarmedAnimator;
    public AnimatorOverrideController OneHandedAndShieldAnimator;
    public AnimatorOverrideController DualWieldAnimator;
    public AnimatorOverrideController TwoHandedAnimator;
    public AnimatorOverrideController BowAnimator;

    [Serializable]
    public enum WeaponAttackTypes
    {
        OneHanded,
        TwoHanded,
        DualWield,
        Shield,
        Bow
    }
}

public enum WeaponRangeType
{
    None,
    Melee,
    Ranged,
    Shield
}


