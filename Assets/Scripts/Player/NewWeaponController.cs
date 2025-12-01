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
        public WeaponSetType weaponSetType;
        public bool hasPrimaryWeaponData;
        public bool hasSecondaryWeaponData;
        public ItemData PrimaryWeaponData;
        public ItemData SecondaryWeaponData;
        public Weapon InstantiatedPrimaryWeapon;
        public Weapon InstantiatedSecondaryWeapon;
    }

    public WeaponSet OneHandedWeaponSet;
    public WeaponSet TwoHandedWeaponSet;
    public WeaponSet RangedWeaponSet;

    public WeaponSet ActiveWeaponSet;

    public enum WeaponSetType
    {
        OneHanded,
        TwoHanded,
        Ranged
    }

    public float CombinedWeaponDamage;

    public Action<string> OnWeaponUnequipped;

    protected void Awake()
    {
        comboCounter = new CountdownTimer(comboPeriod);
        OneHandedWeaponSet.weaponSetType = WeaponSetType.OneHanded;
        TwoHandedWeaponSet.weaponSetType = WeaponSetType.TwoHanded;
        RangedWeaponSet.weaponSetType = WeaponSetType.Ranged;
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

    public void EquipWeapon(ItemData itemData, Weapon.WeaponHand weaponHand = Weapon.WeaponHand.MainHand)
    {
        if (itemData.itemType == ItemType.OneHanded)
        {
            EquipWeaponToWeaponSet(itemData, weaponHand, OneHandedWeaponSet);
        }
        else if (itemData.itemType == ItemType.Offhand)
        {
            EquipWeaponToWeaponSet(itemData, Weapon.WeaponHand.OffHand, OneHandedWeaponSet);
        }
        else if (itemData.itemType == ItemType.TwoHanded)
        {
            EquipWeaponToWeaponSet(itemData, weaponHand, TwoHandedWeaponSet);
        }
        else if (itemData.itemType == ItemType.Bow)
        {
            EquipWeaponToWeaponSet(itemData, weaponHand, RangedWeaponSet);
        }
    }

    public void UnequipWeapon(ItemData itemData)
    {
        if (itemData.itemType == ItemType.OneHanded || itemData.itemType == ItemType.Offhand)
        {
            RemoveWeaponFromWeaponSet(itemData, OneHandedWeaponSet);
        }
        else if (itemData.itemType == ItemType.TwoHanded)
        {
            RemoveWeaponFromWeaponSet(itemData, TwoHandedWeaponSet);
        }
        else if (itemData.itemType == ItemType.Bow)
        {
            RemoveWeaponFromWeaponSet(itemData, RangedWeaponSet);
        }
    }

    public void RemoveWeaponFromWeaponSet(ItemData itemData, WeaponSet weaponSet)
    {
        bool isActiveWeaponSet = false;

        if (weaponSet.PrimaryWeaponData == itemData)
        {
            if (weaponSet.InstantiatedPrimaryWeapon != null)
            {
                if (instantiatedPrimaryWeapon != null && instantiatedPrimaryWeapon == weaponSet.InstantiatedPrimaryWeapon)
                {
                    isActiveWeaponSet = true;
                    instantiatedPrimaryWeapon = null;
                }
                Destroy(weaponSet.InstantiatedPrimaryWeapon.gameObject);
                weaponSet.InstantiatedPrimaryWeapon = null;
            }

            weaponSet.PrimaryWeaponData = null;
            weaponSet.hasPrimaryWeaponData = false;
            OnWeaponUnequipped?.Invoke(itemData.itemID);
        }
        else if (weaponSet.SecondaryWeaponData == itemData)
        {
            if (weaponSet.InstantiatedSecondaryWeapon != null)
            {
                if (instantiatedSecondaryWeapon != null && instantiatedSecondaryWeapon == weaponSet.InstantiatedSecondaryWeapon)
                {
                    isActiveWeaponSet = true;
                    instantiatedSecondaryWeapon = null;
                }
                Destroy(weaponSet.InstantiatedSecondaryWeapon.gameObject);
                weaponSet.InstantiatedSecondaryWeapon = null;
            }

            weaponSet.SecondaryWeaponData = null;
            weaponSet.hasSecondaryWeaponData = false;
            OnWeaponUnequipped?.Invoke(itemData.itemID);
        }

        if (isActiveWeaponSet)
            SetActiveWeaponSet(weaponSet);
    }

    private void EquipWeaponToWeaponSet(ItemData itemData, Weapon.WeaponHand weaponHand, WeaponSet weaponSet)
    {
        if (weaponHand == Weapon.WeaponHand.MainHand)
        {
            if (weaponSet.hasPrimaryWeaponData)
            {
                OnWeaponUnequipped?.Invoke(weaponSet.PrimaryWeaponData.itemID);
            }
            weaponSet.PrimaryWeaponData = itemData;
            weaponSet.hasPrimaryWeaponData = true;
        }
        else
        {
            if (weaponSet.hasSecondaryWeaponData)
            {
                OnWeaponUnequipped?.Invoke(weaponSet.SecondaryWeaponData.itemID);
            }
            weaponSet.SecondaryWeaponData = itemData;
            weaponSet.hasSecondaryWeaponData = true;
        }

        SetActiveWeaponSet(weaponSet);
    }


    public void CycleActiveWeaponSetUp()
    {
        if (!canAttack) return;

        if (ActiveWeaponSet.weaponSetType == OneHandedWeaponSet.weaponSetType)
        {
            if (RangedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
            else if (TwoHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
        }
        else if (ActiveWeaponSet.weaponSetType == TwoHandedWeaponSet.weaponSetType)
        {
            if (OneHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
            }
            else if (RangedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
        }
        else if (ActiveWeaponSet.weaponSetType == RangedWeaponSet.weaponSetType)
        {
            if (TwoHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
            else if (OneHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
                return;
            }
        }
    }
    public void CycleActiveWeaponSetDown()
    {
        if (!canAttack) return;

        if (ActiveWeaponSet.weaponSetType == OneHandedWeaponSet.weaponSetType)
        {
            if (TwoHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
            else if (RangedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
        }
        else if (ActiveWeaponSet.weaponSetType == TwoHandedWeaponSet.weaponSetType)
        {
            if (RangedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(RangedWeaponSet);
                return;
            }
            else if (OneHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
            }
        }
        else if (ActiveWeaponSet.weaponSetType == RangedWeaponSet.weaponSetType)
        {
            if (OneHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(OneHandedWeaponSet);
                return;
            }
            else if (TwoHandedWeaponSet.hasPrimaryWeaponData)
            {
                SetActiveWeaponSet(TwoHandedWeaponSet);
            }
        }
    }

    //If I have a weapon equipped to preview and equip something else, the new one doesn't show up
    public void SetActiveWeaponSet(WeaponSet weaponSet)
    {
        if (weaponSet.InstantiatedPrimaryWeapon != null)
        {
            weaponSet.InstantiatedPrimaryWeapon.gameObject.SetActive(true);
        }
        else if (weaponSet.hasPrimaryWeaponData)
        {
            instantiatedPrimaryWeapon = Instantiate(weaponSet.PrimaryWeaponData.objectPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponSet.PrimaryWeaponData.objectPrefab.transform.rotation;
            instantiatedPrimaryWeapon.Init(newPlayerController.PlayerContext, Weapon.WeaponHand.MainHand, weaponSet.PrimaryWeaponData);
            weaponSet.InstantiatedPrimaryWeapon = instantiatedPrimaryWeapon;
        }

        if (weaponSet.InstantiatedSecondaryWeapon != null)
        {
            weaponSet.InstantiatedSecondaryWeapon.gameObject.SetActive(true);
        }
        else if (weaponSet.hasSecondaryWeaponData)
        {
            instantiatedSecondaryWeapon = Instantiate(weaponSet.SecondaryWeaponData.objectPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
            instantiatedSecondaryWeapon.transform.localRotation = weaponSet.SecondaryWeaponData.objectPrefab.transform.rotation;
            instantiatedSecondaryWeapon.Init(newPlayerController.PlayerContext, Weapon.WeaponHand.OffHand, weaponSet.SecondaryWeaponData);
            weaponSet.InstantiatedSecondaryWeapon = instantiatedSecondaryWeapon;
        }

        if (ActiveWeaponSet.InstantiatedPrimaryWeapon != null && ActiveWeaponSet.InstantiatedPrimaryWeapon != weaponSet.InstantiatedPrimaryWeapon)
        {
            ActiveWeaponSet.InstantiatedPrimaryWeapon.gameObject.SetActive(false);
        }

        if (ActiveWeaponSet.InstantiatedSecondaryWeapon != null && ActiveWeaponSet.InstantiatedSecondaryWeapon != weaponSet.InstantiatedSecondaryWeapon)
        {
            ActiveWeaponSet.InstantiatedSecondaryWeapon.gameObject.SetActive(false);
        }

        GameObject weaponPrefab = null;
        if (weaponSet.InstantiatedPrimaryWeapon != null)
            weaponPrefab = weaponSet.PrimaryWeaponData.objectPrefab;

        newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponPrefab);

        weaponPrefab = null;
        if (weaponSet.InstantiatedSecondaryWeapon != null)
            weaponPrefab = weaponSet.SecondaryWeaponData.objectPrefab;

        newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.OffHand, weaponPrefab);

        instantiatedPrimaryWeapon = weaponSet.InstantiatedPrimaryWeapon;
        instantiatedSecondaryWeapon = weaponSet.InstantiatedSecondaryWeapon;

        ActiveWeaponSet = weaponSet;

        CalculateWeaponDamage();
        UpdateAnimator();
    }

    public void EquipStarterItems(Item mainHand, Item offHand)
    {
        if (StarterWeaponsEquipped) return;
        StarterWeaponsEquipped = true;

        if (mainHand != null)
        {
            Item starterSword = Instantiate(mainHand);
            starterSword.itemData.itemQuality = ItemQuality.Shoddy;
            starterSword.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterSword.itemData);
            newPlayerController.PlayerContext.InventoryController.AddItemToInventory(starterSword.itemData, true);
            Destroy(starterSword.gameObject);
        }

        if (offHand != null)
        {
            Item starterShield = Instantiate(offHand);
            starterShield.itemData.itemQuality = ItemQuality.Shoddy;
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
                        SpawnedItemDataBase.SpawnedItemData spawnedShieldData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedSecondaryWeapon.itemID);
                        if (spawnedShieldData.itemData.data is ShieldSO shieldData)
                            newPlayerController.HealthController.UpdateArmorAmount(shieldData.ArmorAmount);
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
        if (attackProfile == bowAttackProfile)
        {
            animator.SetBool("HasBow", true);
        }
        else
        {
            animator.SetBool("HasBow", false);
        }
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

    public bool CanEquipOffhand(ItemData itemData)
    {
        if (itemData.itemType == ItemType.OneHanded)
        {
            return true;
        }

        if (itemData.itemType == ItemType.TwoHanded && newPlayerController.PlayerStatsBlackboard.TwoHandedMastery)
        {
            return true;
        }

        return false;
    }

    [ContextMenu("Apply Bow Attack Profile")]
    public void ApplyBowAttackProfile()
    {
        ApplyAttackProfile(bowAttackProfile);
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


