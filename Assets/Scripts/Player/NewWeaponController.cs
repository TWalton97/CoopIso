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

    public Item StartingMainHandWeapon;
    public Item StartingOffHandWeapon;
    private bool StarterWeaponsEquipped = false;

    protected void Awake()
    {
        comboCounter = new CountdownTimer(comboPeriod);
    }

    void Start()
    {
        newPlayerController.AnimationStatusTracker.OnAnimationCompleted += ResetAttack;
        EquipStarterItems();
    }

    void OnEnable()
    {
        comboCounter.OnTimerStop += () => numAttacks = 0;
    }

    void OnDisable()
    {
        newPlayerController.AnimationStatusTracker.OnAnimationCompleted -= ResetAttack;
        comboCounter.OnTimerStop -= () => numAttacks = 0;
    }

    void Update()
    {
        comboCounter.Tick(Time.deltaTime);
    }

    public void EquipStarterItems()
    {
        if (StarterWeaponsEquipped) return;
        StarterWeaponsEquipped = true;

        if (StartingMainHandWeapon != null)
        {
            Item starterSword = Instantiate(StartingMainHandWeapon);
            starterSword.itemData.itemID = SpawnedItemDataBase.Instance.RegisterItemToDatabase(starterSword.itemData);
            newPlayerController.InventoryController.FindEquippedSlotOfType(Slot.MainHand)[0].EquipGear(starterSword.itemData);
            Destroy(starterSword.gameObject);
        }

        if (StartingOffHandWeapon != null)
        {
            Item starterShield = Instantiate(StartingOffHandWeapon);
            starterShield.itemData.itemID = SpawnedItemDataBase.Instance.RegisterItemToDatabase(starterShield.itemData);
            newPlayerController.InventoryController.FindEquippedSlotOfType(Slot.OffHand)[0].EquipGear(starterShield.itemData);
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

        WeaponDataSO weaponData = instantiatedPrimaryWeapon.Data as WeaponDataSO;
        if (numAttacks >= weaponData.NumberOfAttacksInCombo)
        {
            numAttacks = 0;
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
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData.itemID);
            UpdateAnimator();
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            UnequipWeapon(Weapon.WeaponHand.MainHand);
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData.itemID);
            UpdateAnimator();
        }
        else if (instantiatedSecondaryWeapon == null)
        {
            instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
            instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
            instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand, itemData.itemID);
            UpdateAnimator();
        }
        else
        {
            UnequipWeapon(Weapon.WeaponHand.MainHand);
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData.itemID);
            UpdateAnimator();
        }
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
        instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand, itemData.itemID);
        UpdateAnimator();
        OnWeaponUpdated?.Invoke();
    }

    public void EquipOffhand(GameObject weaponPrefab, ItemData itemData)
    {
        //If there's already a secondary weapon
        //Replace it
        UnequipWeapon(Weapon.WeaponHand.OffHand);
        instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
        instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
        instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand, itemData.itemID);
        if (instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.Shield)
        {
            HasShieldEquipped = true;
        }
        UpdateAnimator();
        OnWeaponUpdated?.Invoke();
    }

    public void UpdateAnimator()
    {
        primaryWeaponAttackCompleted = false;
        canAttack = true;

        if (instantiatedPrimaryWeapon == null)
        {
            animator.SetBool("DualWielding", false);
            animator.runtimeAnimatorController = UnarmedAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && HasShieldEquipped)
        {
            animator.SetBool("DualWielding", false);
            animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && instantiatedSecondaryWeapon == null)
        {
            animator.SetBool("DualWielding", false);
            animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
        {
            animator.SetBool("DualWielding", true);
            animator.runtimeAnimatorController = DualWieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            animator.SetBool("DualWielding", false);
            animator.runtimeAnimatorController = TwoHandedAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.Bow)
        {
            animator.SetBool("DualWielding", false);
            animator.runtimeAnimatorController = BowAnimator;
        }
        OnWeaponUpdated?.Invoke();
    }

    public void UnequipWeapon(Weapon.WeaponHand weaponHand)
    {
        switch (weaponHand)
        {
            case Weapon.WeaponHand.MainHand:
                if (instantiatedPrimaryWeapon != null)
                {
                    Destroy(instantiatedPrimaryWeapon.gameObject);
                    instantiatedPrimaryWeapon = null;
                }
                break;
            case Weapon.WeaponHand.OffHand:
                if (instantiatedSecondaryWeapon != null)
                {
                    Destroy(instantiatedSecondaryWeapon.gameObject);
                    HasShieldEquipped = false;
                    instantiatedSecondaryWeapon = null;
                }
                break;
        }
    }

    public RuntimeAnimatorController UnarmedAnimator;
    public AnimatorOverrideController OneHandedAndShieldAnimator;
    public AnimatorOverrideController DualWieldAnimator;
    public AnimatorOverrideController TwoHandedAnimator;
    public AnimatorOverrideController BowAnimator;

    [System.Serializable]
    public enum WeaponAttackTypes
    {
        OneHanded,
        TwoHanded,
        DualWield,
        Shield,
        Bow
    }
}


