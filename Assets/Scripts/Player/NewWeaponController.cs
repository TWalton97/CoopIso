using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class NewWeaponController : Singleton<NewWeaponController>
{
    private NewPlayerController newPlayerController;
    public Animator animator;

    public Transform mainHandTransform;
    public Transform offHandTransform;

    public Weapon instantiatedPrimaryWeapon;
    public Weapon instantiatedSecondaryWeapon;
    private WeaponAttackTypes currentWeaponAttackType;
    public bool primaryWeaponAttackCompleted = false;
    private CountdownTimer comboCounter;    //We use this to determine when to reset our current attack combo
    private float comboPeriod = 3;
    public bool canAttack = true;
    public bool HasShieldEquipped;

    protected override void Awake()
    {
        base.Awake();
        comboCounter = new CountdownTimer(comboPeriod);
        newPlayerController = GetComponent<NewPlayerController>();
    }

    void OnEnable()
    {
        comboCounter.OnTimerStop += () => primaryWeaponAttackCompleted = false;
    }

    void OnDisable()
    {
        comboCounter.OnTimerStop -= () => primaryWeaponAttackCompleted = false;
    }

    void Update()
    {
        comboCounter.Tick(Time.deltaTime);
    }

    public void Attack(Action OnActionCompleted)
    {
        if (instantiatedPrimaryWeapon == null) return;
        if (!canAttack) return;

        if (primaryWeaponAttackCompleted)
        {
            if (currentWeaponAttackType == WeaponAttackTypes.DualWield)
            {
                instantiatedSecondaryWeapon.Enter(() => canAttack = true);
                canAttack = false;
                comboCounter.Start();
                primaryWeaponAttackCompleted = !primaryWeaponAttackCompleted;
                return;
            }
        }

        instantiatedPrimaryWeapon.Enter(OnActionCompleted);
        canAttack = false;
        comboCounter.Start();
        primaryWeaponAttackCompleted = !primaryWeaponAttackCompleted;
    }

    public void EquipOneHandedWeapon(GameObject weaponPrefab)
    {
        //If the player's main hand is open, equip it there
        //Otherwise try to equip in offhand
        //Otherwise just replace main hand
        if (instantiatedPrimaryWeapon == null)
        {
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
            UpdateAnimator();
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            UnequipWeapon(Weapon.WeaponHand.MainHand);
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
            UpdateAnimator();
        }
        else if (instantiatedSecondaryWeapon == null)
        {
            instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
            instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
            instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand);
            UpdateAnimator();
        }
        else
        {
            UnequipWeapon(Weapon.WeaponHand.MainHand);
            instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
            instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
            instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
            instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
            UpdateAnimator();
        }
    }

    public void EquipTwoHandedWeapon(GameObject weaponPrefab)
    {
        //Unequip both existing weapons if they exist
        UnequipWeapon(Weapon.WeaponHand.MainHand);
        UnequipWeapon(Weapon.WeaponHand.OffHand);

        //Create the object in the player's main hand
        instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
        instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
        instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
        UpdateAnimator();
    }

    public void EquipOffhand(GameObject weaponPrefab)
    {
        //If there's already a secondary weapon
        //Replace it
        UnequipWeapon(Weapon.WeaponHand.OffHand);
        instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
        instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
        instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand);
        UpdateAnimator();

        if (instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.Shield)
        {
            HasShieldEquipped = true;
        }
    }

    private void UpdateAnimator()
    {
        primaryWeaponAttackCompleted = false;

        if (instantiatedPrimaryWeapon == null) return;
        //Need to check when we equip weapons if main hand is empty, and equip there if it is
        if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            animator.runtimeAnimatorController = TwoHandedAnimator;
            currentWeaponAttackType = WeaponAttackTypes.TwoHanded;
        }
        else if (instantiatedSecondaryWeapon != null)
        {
            if (instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
            {
                animator.runtimeAnimatorController = DualWieldAnimator;
                currentWeaponAttackType = WeaponAttackTypes.DualWield;
            }
        }
        else
        {
            animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
            currentWeaponAttackType = WeaponAttackTypes.OneHanded;
        }
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

    public AnimatorOverrideController OneHandedAndShieldAnimator;
    public AnimatorOverrideController DualWieldAnimator;
    public AnimatorOverrideController TwoHandedAnimator;

    [System.Serializable]
    public enum WeaponAttackTypes
    {
        OneHanded,
        TwoHanded,
        DualWield,
        Shield
    }
}


