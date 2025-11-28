using System;
using Unity.VisualScripting;
using UnityEngine;
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

    [Serializable]
    public class WeaponSet
    {
        public bool hasPrimaryWeaponData;
        public bool hasSecondaryWeaponData;
        public ItemData PrimaryWeaponData;
        public ItemData SecondaryWeaponData;

        public bool HasPrimaryWeaponData()
        {
            return hasPrimaryWeaponData;
        }

        public bool HasSecondaryWeaponData()
        {
            return hasSecondaryWeaponData;
        }
    }

    public WeaponSet OneHandedWeaponSet;
    public WeaponSet TwoHandedWeaponSet;
    public WeaponSet RangedWeaponSet;

    public WeaponSet ActiveWeaponSet;

    public float CombinedWeaponDamage;

    public Action<string> OnWeaponUnequipped;

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

    public void EquipWeapon(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.OneHanded:
                if (!OneHandedWeaponSet.HasPrimaryWeaponData())
                {
                    UpdateOneHandedWeaponSet(itemData, Weapon.WeaponHand.MainHand, true);
                }
                else
                {
                    UpdateOneHandedWeaponSet(itemData, Weapon.WeaponHand.OffHand, true);
                }
                break;
            case ItemType.TwoHanded:
                if (!TwoHandedWeaponSet.HasPrimaryWeaponData())
                {
                    UpdateTwoHandedWeaponSet(itemData, Weapon.WeaponHand.MainHand, true);
                }
                else if (newPlayerController.PlayerStatsBlackboard.TwoHandedMastery == true)
                {
                    UpdateTwoHandedWeaponSet(itemData, Weapon.WeaponHand.OffHand, true);
                }
                break;
            case ItemType.Bow:
                UpdateRangedWeaponSet(itemData, Weapon.WeaponHand.MainHand, true);
                break;
            case ItemType.Offhand:
                UpdateOneHandedWeaponSet(itemData, Weapon.WeaponHand.OffHand, true);
                break;
        }
    }
    public void UnequipWeapon(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.OneHanded:
                if (OneHandedWeaponSet.PrimaryWeaponData == itemData)
                {
                    UpdateOneHandedWeaponSet(null, Weapon.WeaponHand.MainHand);
                }
                else if (OneHandedWeaponSet.SecondaryWeaponData == itemData)
                {
                    UpdateOneHandedWeaponSet(null, Weapon.WeaponHand.OffHand);
                }
                break;
            case ItemType.TwoHanded:
                if (TwoHandedWeaponSet.PrimaryWeaponData == itemData)
                {
                    UpdateTwoHandedWeaponSet(null, Weapon.WeaponHand.MainHand);
                }
                else if (TwoHandedWeaponSet.SecondaryWeaponData == itemData)
                {
                    UpdateTwoHandedWeaponSet(null, Weapon.WeaponHand.OffHand);
                }
                break;
            case ItemType.Bow:
                if (RangedWeaponSet.PrimaryWeaponData == itemData)
                {
                    UpdateRangedWeaponSet(null, Weapon.WeaponHand.MainHand);
                }
                break;
            case ItemType.Offhand:
                if (OneHandedWeaponSet.SecondaryWeaponData == itemData)
                {
                    UpdateOneHandedWeaponSet(null, Weapon.WeaponHand.OffHand);
                }
                break;
        }
    }

    public void UpdateOneHandedWeaponSet(ItemData itemData, Weapon.WeaponHand weaponHand, bool equipSet = false)
    {
        if (weaponHand == Weapon.WeaponHand.MainHand)
        {
            if (itemData == null)
            {
                OneHandedWeaponSet.hasPrimaryWeaponData = false;
            }
            else
            {
                OneHandedWeaponSet.hasPrimaryWeaponData = true;
            }

            if (OneHandedWeaponSet.PrimaryWeaponData != null)
            {
                OnWeaponUnequipped?.Invoke(OneHandedWeaponSet.PrimaryWeaponData.itemID);
            }
            OneHandedWeaponSet.PrimaryWeaponData = itemData;
        }

        if (weaponHand == Weapon.WeaponHand.OffHand)
        {
            if (itemData == null)
            {
                OneHandedWeaponSet.hasSecondaryWeaponData = false;
            }
            else
            {
                OneHandedWeaponSet.hasSecondaryWeaponData = true;
            }

            if (OneHandedWeaponSet.SecondaryWeaponData != null)
            {
                OnWeaponUnequipped?.Invoke(OneHandedWeaponSet.PrimaryWeaponData.itemID);
            }
            OneHandedWeaponSet.SecondaryWeaponData = itemData;
        }

        SetActiveWeaponSet(OneHandedWeaponSet);
    }
    public void UpdateTwoHandedWeaponSet(ItemData itemData, Weapon.WeaponHand weaponHand, bool equipSet = false)
    {
        if (weaponHand == Weapon.WeaponHand.MainHand)
        {
            if (TwoHandedWeaponSet.PrimaryWeaponData != null)
            {
                OnWeaponUnequipped?.Invoke(TwoHandedWeaponSet.PrimaryWeaponData.itemID);
            }
            TwoHandedWeaponSet.PrimaryWeaponData = itemData;
        }

        if (weaponHand == Weapon.WeaponHand.OffHand)
        {
            if (TwoHandedWeaponSet.SecondaryWeaponData != null)
            {
                OnWeaponUnequipped?.Invoke(TwoHandedWeaponSet.SecondaryWeaponData.itemID);
            }
            TwoHandedWeaponSet.SecondaryWeaponData = itemData;
        }

        SetActiveWeaponSet(TwoHandedWeaponSet);
    }
    public void UpdateRangedWeaponSet(ItemData itemData, Weapon.WeaponHand weaponHand, bool equipSet = false)
    {

        if (weaponHand == Weapon.WeaponHand.MainHand)
        {
            if (RangedWeaponSet.PrimaryWeaponData != null)
            {
                if (itemData == null)
                {
                    RangedWeaponSet.hasPrimaryWeaponData = false;
                }
                else
                {
                    RangedWeaponSet.hasPrimaryWeaponData = true;
                }

                OnWeaponUnequipped?.Invoke(RangedWeaponSet.PrimaryWeaponData.itemID);
            }
            RangedWeaponSet.PrimaryWeaponData = itemData;
        }

        if (weaponHand == Weapon.WeaponHand.OffHand)
        {
            if (RangedWeaponSet.SecondaryWeaponData != null)
            {
                OnWeaponUnequipped?.Invoke(RangedWeaponSet.SecondaryWeaponData.itemID);
            }
            RangedWeaponSet.SecondaryWeaponData = itemData;
        }

        //if (equipSet)
        SetActiveWeaponSet(RangedWeaponSet);
    }
    public void CycleActiveWeaponSetUp()
    {
        if (ActiveWeaponSet == OneHandedWeaponSet)
        {
            if (RangedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
            else if (TwoHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
        }
        else if (ActiveWeaponSet == TwoHandedWeaponSet)
        {
            if (OneHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
            }
            else if (RangedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
        }
        else if (ActiveWeaponSet == RangedWeaponSet)
        {
            if (TwoHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
            else if (OneHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
                return;
            }
        }
    }
    public void CycleActiveWeaponSetDown()
    {
        if (ActiveWeaponSet == OneHandedWeaponSet)
        {
            if (TwoHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
            else if (RangedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
        }
        else if (ActiveWeaponSet == TwoHandedWeaponSet)
        {
            if (RangedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
            else if (OneHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
            }
        }
        else if (ActiveWeaponSet == RangedWeaponSet)
        {
            if (OneHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
                return;
            }
            else if (TwoHandedWeaponSet.HasPrimaryWeaponData())
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
        }
    }
    public void SetActiveWeaponSet(WeaponSet weaponSet)
    {
        ActiveWeaponSet.PrimaryWeaponData = null;
        ActiveWeaponSet.SecondaryWeaponData = null;

        ActiveWeaponSet.PrimaryWeaponData = weaponSet.PrimaryWeaponData;
        ActiveWeaponSet.SecondaryWeaponData = weaponSet.SecondaryWeaponData;

        EquipWeaponSet(weaponSet);
    }
    public void EquipWeaponSet(WeaponSet weaponSet)
    {
        if (instantiatedPrimaryWeapon != null)
        {
            newPlayerController.PlayerContext.PlayerPreviewManager.UnequipWeaponFromPlayer(newPlayerController.PlayerInputController.playerIndex, Weapon.WeaponHand.MainHand);
            Destroy(instantiatedPrimaryWeapon.gameObject);
            instantiatedPrimaryWeapon = null;
        }

        if (weaponSet.hasPrimaryWeaponData)
        {
            instantiatedPrimaryWeapon = Instantiate(weaponSet.PrimaryWeaponData.objectPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponSet.PrimaryWeaponData.objectPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, weaponSet.PrimaryWeaponData);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponSet.PrimaryWeaponData.objectPrefab);
        }

        if (instantiatedSecondaryWeapon != null)
        {
            newPlayerController.PlayerContext.PlayerPreviewManager.UnequipWeaponFromPlayer(newPlayerController.PlayerInputController.playerIndex, Weapon.WeaponHand.OffHand);
            Destroy(instantiatedSecondaryWeapon.gameObject);
            instantiatedSecondaryWeapon = null;
        }

        if (weaponSet.hasSecondaryWeaponData)
        {
            instantiatedSecondaryWeapon = Instantiate(weaponSet.SecondaryWeaponData.objectPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
            instantiatedSecondaryWeapon.transform.localRotation = weaponSet.SecondaryWeaponData.objectPrefab.transform.rotation;
            instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
            instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand, weaponSet.SecondaryWeaponData);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.OffHand, weaponSet.SecondaryWeaponData.objectPrefab);
        }

        UpdateAnimator();
    }
    public void EquipStarterItems(Item mainHand, Item offHand)
    {
        if (StarterWeaponsEquipped) return;
        StarterWeaponsEquipped = true;

        if (mainHand != null)
        {
            Item starterSword = Instantiate(mainHand);
            starterSword.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterSword.itemData);
            newPlayerController.PlayerContext.InventoryController.AddItemToInventory(starterSword.itemData, true);
            Destroy(starterSword.gameObject);
        }

        if (offHand != null)
        {
            Item starterShield = Instantiate(offHand);
            starterShield.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterShield.itemData);
            newPlayerController.PlayerContext.InventoryController.AddItemToInventory(starterShield.itemData, true);
            Destroy(starterShield.gameObject);
        }
    }
    public void Attack(Action OnActionCompleted, bool shootFromAiming = false)
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

        if (shootFromAiming)
        {
            instantiatedPrimaryWeapon.Enter(OnActionCompleted, numAttacks);
            newPlayerController.Animator.SetBool("ShootFromAiming", true);
            numAttacks++;
            canAttack = false;
        }
        else
        {
            instantiatedPrimaryWeapon.Enter(OnActionCompleted, numAttacks);
            newPlayerController.Animator.SetInteger("counter", numAttacks);
            newPlayerController.Animator.SetTrigger("Attack");
            numAttacks++;
            canAttack = false;
            comboCounter.Start();
        }



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
                        newPlayerController.HealthController.UpdateArmorAmount(-spawnedShieldData.armorAmount);
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
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("BlockingLoop", attackProfile.altAttack);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("PunchLeft", attackProfile.attack1);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("PunchRight", attackProfile.attack2);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("PunchLeft", attackProfile.attack3);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("SpinAttack_TwoWeapons", attackProfile.comboAttack);
    }

    private void CalculateWeaponDamage()
    {
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


