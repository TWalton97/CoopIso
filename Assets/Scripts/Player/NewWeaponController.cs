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
    public int numAttacks;

    public bool canAttack = true;
    public bool HasShieldEquipped;
    private Action OnActionCompleted;

    private bool StarterWeaponsEquipped = false;

    public AttackProfile unarmedAttackProfile;
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
    public float CombinedWeaponAttackSpeed;

    public Action<string> OnWeaponUnequipped;

    public bool CanBlock => !(instantiatedPrimaryWeapon != null && instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.Bow);

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
        if (itemData.ItemSO.ItemType == ItemType.OneHanded)
        {
            EquipWeaponToWeaponSet(itemData, weaponHand, OneHandedWeaponSet);
        }
        else if (itemData.ItemSO.ItemType == ItemType.Offhand)
        {
            EquipWeaponToWeaponSet(itemData, Weapon.WeaponHand.OffHand, OneHandedWeaponSet);
        }
        else if (itemData.ItemSO.ItemType == ItemType.TwoHanded)
        {
            EquipWeaponToWeaponSet(itemData, weaponHand, TwoHandedWeaponSet);
        }
        else if (itemData.ItemSO.ItemType == ItemType.Bow)
        {
            EquipWeaponToWeaponSet(itemData, weaponHand, RangedWeaponSet);
        }
    }

    public void UnequipWeapon(ItemData itemData)
    {
        if (itemData.ItemSO.ItemType == ItemType.OneHanded || itemData.ItemSO.ItemType == ItemType.Offhand)
        {
            RemoveWeaponFromWeaponSet(itemData, OneHandedWeaponSet);
        }
        else if (itemData.ItemSO.ItemType == ItemType.TwoHanded)
        {
            RemoveWeaponFromWeaponSet(itemData, TwoHandedWeaponSet);
        }
        else if (itemData.ItemSO.ItemType == ItemType.Bow)
        {
            RemoveWeaponFromWeaponSet(itemData, RangedWeaponSet);
        }
    }

    public void RemoveWeaponFromWeaponSet(ItemData itemData, WeaponSet weaponSet)
    {
        if (weaponSet.PrimaryWeaponData == itemData)
        {
            weaponSet.PrimaryWeaponData = null;
            weaponSet.hasPrimaryWeaponData = false;
            OnWeaponUnequipped?.Invoke(itemData.ItemID);
        }
        else if (weaponSet.SecondaryWeaponData == itemData)
        {
            weaponSet.SecondaryWeaponData = null;
            weaponSet.hasSecondaryWeaponData = false;
            OnWeaponUnequipped?.Invoke(itemData.ItemID);
        }

        if (ActiveWeaponSet.weaponSetType == weaponSet.weaponSetType)
            SetActiveWeaponSet(weaponSet);
    }

    private void EquipWeaponToWeaponSet(ItemData itemData, Weapon.WeaponHand weaponHand, WeaponSet weaponSet)
    {
        if (weaponHand == Weapon.WeaponHand.MainHand)
        {
            if (weaponSet.hasPrimaryWeaponData)
            {
                UnequipWeapon(weaponSet.PrimaryWeaponData);
            }
            weaponSet.PrimaryWeaponData = itemData;
            weaponSet.hasPrimaryWeaponData = true;
        }
        else
        {
            if (weaponSet == OneHandedWeaponSet && !weaponSet.hasPrimaryWeaponData && itemData.ItemSO.ItemType != ItemType.Offhand)
            {
                EquipWeaponToWeaponSet(itemData, Weapon.WeaponHand.MainHand, weaponSet);
                return;
            }

            if (weaponSet.hasSecondaryWeaponData)
            {
                UnequipWeapon(weaponSet.SecondaryWeaponData);
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
        if (instantiatedPrimaryWeapon != null)
        {
            foreach (GemSocket gemSocket in instantiatedPrimaryWeapon.ItemData.socketedGems)
            {
                foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                {
                    if (gemEffect.AppliesTo == gemSocket.SlotType)
                        gemEffect.Deregister();
                }
            }
            Destroy(instantiatedPrimaryWeapon.gameObject);
            instantiatedPrimaryWeapon = null;
        }

        if (weaponSet.hasPrimaryWeaponData)
        {
            instantiatedPrimaryWeapon = Instantiate(weaponSet.PrimaryWeaponData.ItemPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.Data = weaponSet.PrimaryWeaponData.ItemSO;
            instantiatedPrimaryWeapon.ItemData = weaponSet.PrimaryWeaponData;
            instantiatedPrimaryWeapon.transform.localRotation = weaponSet.PrimaryWeaponData.ItemPrefab.transform.rotation;
            instantiatedPrimaryWeapon.Init(newPlayerController.PlayerContext, Weapon.WeaponHand.MainHand, weaponSet.PrimaryWeaponData);
            foreach (GemSocket gemSocket in instantiatedPrimaryWeapon.ItemData.socketedGems)
            {
                if (gemSocket.Gem != null)
                {
                    GemContext gemContext = new GemContext();
                    gemContext.playerContext = newPlayerController.PlayerContext;
                    gemContext.itemData = weaponSet.PrimaryWeaponData;
                    gemContext.slotType = gemSocket.SlotType;

                    foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                    {
                        if (gemEffect.AppliesTo == gemSocket.SlotType)
                            gemEffect.Apply(gemContext);
                    }
                }
            }
        }

        if (instantiatedSecondaryWeapon != null)
        {
            if (instantiatedSecondaryWeapon.ItemData.ItemSO is ShieldSO)
            {
                newPlayerController.HealthController.UpdateArmorAmount(-instantiatedSecondaryWeapon.ItemData.ShieldArmorAmount);
            }

            foreach (GemSocket gemSocket in instantiatedSecondaryWeapon.ItemData.socketedGems)
            {
                foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                {
                    if (gemEffect.AppliesTo == gemSocket.SlotType)
                        gemEffect.Deregister();
                }
            }

            Destroy(instantiatedSecondaryWeapon.gameObject);
            instantiatedSecondaryWeapon = null;
        }

        if (weaponSet.hasSecondaryWeaponData)
        {
            instantiatedSecondaryWeapon = Instantiate(weaponSet.SecondaryWeaponData.ItemPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
            instantiatedSecondaryWeapon.Data = weaponSet.SecondaryWeaponData.ItemSO;
            instantiatedSecondaryWeapon.ItemData = weaponSet.SecondaryWeaponData;
            instantiatedSecondaryWeapon.transform.localRotation = weaponSet.SecondaryWeaponData.ItemPrefab.transform.rotation;
            instantiatedSecondaryWeapon.Init(newPlayerController.PlayerContext, Weapon.WeaponHand.OffHand, weaponSet.SecondaryWeaponData);

            foreach (GemSocket gemSocket in instantiatedSecondaryWeapon.ItemData.socketedGems)
            {
                if (gemSocket.Gem != null)
                {
                    GemContext gemContext = new GemContext();
                    gemContext.playerContext = newPlayerController.PlayerContext;
                    gemContext.itemData = weaponSet.SecondaryWeaponData;
                    gemContext.slotType = gemSocket.SlotType;

                    foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                    {
                        if (gemEffect.AppliesTo == gemSocket.SlotType)
                        {
                            gemEffect.Apply(gemContext);
                        }
                    }
                }
            }
        }

        GameObject weaponPrefab = null;
        if (weaponSet.hasPrimaryWeaponData)
            weaponPrefab = weaponSet.PrimaryWeaponData.ItemPrefab;

        newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.MainHand, weaponPrefab);

        weaponPrefab = null;
        if (weaponSet.hasSecondaryWeaponData)
            weaponPrefab = weaponSet.SecondaryWeaponData.ItemPrefab;

        newPlayerController.PlayerContext.PlayerPreviewManager.EquipWeaponToPlayer(newPlayerController.PlayerContext.PlayerIndex, Weapon.WeaponHand.OffHand, weaponPrefab);

        ActiveWeaponSet = weaponSet;

        if (instantiatedSecondaryWeapon != null && ActiveWeaponSet.SecondaryWeaponData.ItemSO.ItemType == ItemType.Offhand)
        {
            HasShieldEquipped = true;
            if (ActiveWeaponSet.SecondaryWeaponData.ItemSO is ShieldSO)
            {
                newPlayerController.HealthController.UpdateArmorAmount(ActiveWeaponSet.SecondaryWeaponData.ShieldArmorAmount);
            }

        }
        else
        {
            HasShieldEquipped = false;
        }

        CalculateWeaponDamage();
        CalculateWeaponAttackSpeed();
        UpdateAnimator();
    }

    public void EquipStarterItems(ItemSO mainHand, ItemSO offHand)
    {
        if (StarterWeaponsEquipped) return;
        StarterWeaponsEquipped = true;

        if (mainHand != null)
        {
            ItemData itemData = SpawnedItemDataBase.Instance.CreateItemData(mainHand, ItemQuality.Shoddy);
            newPlayerController.PlayerContext.InventoryController.AddItemToInventory(itemData, true);
        }

        if (offHand != null)
        {
            ItemData itemData = SpawnedItemDataBase.Instance.CreateItemData(offHand, ItemQuality.Shoddy);
            newPlayerController.PlayerContext.InventoryController.AddItemToInventory(itemData, true, true);
        }
    }

    public void Attack(Action OnActionCompleted, bool shootFromAiming = false)
    {
        if (instantiatedPrimaryWeapon.ItemData.ItemSO is WeaponSO weaponData)
        {
            if (numAttacks >= weaponData.NumberOfAttacksInCombo)
            {
                numAttacks = 0;
            }
        }

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
            newPlayerController.Animator.SetBool("ShootFromAiming", true);
            instantiatedPrimaryWeapon.Enter(OnActionCompleted, numAttacks);
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
            ApplyAttackProfile(unarmedAttackProfile);
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
                        ItemData itemData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedSecondaryWeapon.itemID);
                        if (itemData.ItemSO is ShieldSO shieldData)
                            newPlayerController.HealthController.UpdateArmorAmount(shieldData.ArmorAmount);
                    }
                    Destroy(instantiatedSecondaryWeapon.gameObject);
                    HasShieldEquipped = false;
                    instantiatedSecondaryWeapon = null;
                }
                break;
        }
        CalculateWeaponDamage();
        CalculateWeaponAttackSpeed();
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
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("Attack_Default_1", attackProfile.attack1);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("Attack_Default_2", attackProfile.attack2);
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName("Attack_Default_3", attackProfile.attack3);
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

    private void CalculateWeaponAttackSpeed()
    {
        float? primarySpeed = GetWeaponSpeed(instantiatedPrimaryWeapon);
        float? secondarySpeed = (HasShieldEquipped)
            ? null
            : GetWeaponSpeed(instantiatedSecondaryWeapon);

        if (primarySpeed.HasValue && secondarySpeed.HasValue)
        {
            // Dual wield → slower weapon
            CombinedWeaponAttackSpeed = Mathf.Min(primarySpeed.Value, secondarySpeed.Value);
        }
        else if (primarySpeed.HasValue)
        {
            // One-handed weapon
            CombinedWeaponAttackSpeed = primarySpeed.Value;
        }
        else if (secondarySpeed.HasValue)
        {
            // Edge case, but safe
            CombinedWeaponAttackSpeed = secondarySpeed.Value;
        }
        else
        {
            // No weapons
            CombinedWeaponAttackSpeed = 1f;
        }
    }

    private float? GetWeaponSpeed(Weapon weaponInstance)
    {
        if (weaponInstance == null)
            return null;

        if (weaponInstance.ItemData.ItemSO is WeaponSO weaponSO)
            return weaponSO.GetAttackSpeedMultiplier();

        return null;
    }

    public bool CanEquipOffhand(ItemSO itemSO)
    {
        if (itemSO.ItemType == ItemType.OneHanded)
        {
            return true;
        }

        if (itemSO.ItemType == ItemType.TwoHanded && newPlayerController.PlayerStatsBlackboard.TwoHandedMastery)
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


