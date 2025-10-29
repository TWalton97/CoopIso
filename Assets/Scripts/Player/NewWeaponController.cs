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

    public void Attack()
    {
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

        instantiatedPrimaryWeapon.Enter(() => canAttack = true);
        canAttack = false;
        comboCounter.Start();
        primaryWeaponAttackCompleted = !primaryWeaponAttackCompleted;
    }

    public void EquipMainHandWeapon(GameObject weaponPrefab)
    {
        UnequipWeapon(Weapon.WeaponHand.MainHand);
        instantiatedPrimaryWeapon = Instantiate(weaponPrefab, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
        instantiatedPrimaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
        instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
        UpdateAnimator();
    }

    public void EquipOffHandWeapon(GameObject weaponPrefab)
    {
        UnequipWeapon(Weapon.WeaponHand.OffHand);
        instantiatedSecondaryWeapon = Instantiate(weaponPrefab, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
        instantiatedSecondaryWeapon.transform.localRotation = weaponPrefab.transform.rotation;
        instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
        instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand);
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        //If it's two-handed, use two-handed
        //If it's not two-handed and you have a secondary weapon, use dual wield
        //Otherwise use one-handed
        if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            animator.runtimeAnimatorController = TwoHandedAnimator;
            currentWeaponAttackType = WeaponAttackTypes.TwoHanded;
        }
        else if (instantiatedSecondaryWeapon != null)
        {
            animator.runtimeAnimatorController = DualWieldAnimator;
            currentWeaponAttackType = WeaponAttackTypes.DualWield;
        }
        else
        {
            animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
            currentWeaponAttackType = WeaponAttackTypes.OneHanded;
        }
    }

    // public void EquipWeapon(WeaponAttackTypes weaponAttackType, GameObject primaryWeapon, GameObject secondaryWeapon = null)
    // {
    //     UnequipOldWeapons();

    //     switch (weaponAttackType)
    //     {
    //         case WeaponAttackTypes.OneHandedAndShield:
    //             animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
    //             instantiatedPrimaryWeapon = Instantiate(primaryWeapon, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
    //             instantiatedPrimaryWeapon.transform.localRotation = primaryWeapon.transform.rotation;
    //             instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
    //             if (secondaryWeapon != null)
    //             {
    //                 instantiatedSecondaryWeapon = Instantiate(secondaryWeapon, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
    //                 instantiatedSecondaryWeapon.transform.localRotation = secondaryWeapon.transform.rotation;
    //                 instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
    //             }
    //             break;

    //         case WeaponAttackTypes.DualWield:
    //             animator.runtimeAnimatorController = DualWieldAnimator;
    //             instantiatedPrimaryWeapon = Instantiate(primaryWeapon, mainHandTransform.position, Quaternion.identity, mainHandTransform).GetComponent<Weapon>();
    //             instantiatedPrimaryWeapon.transform.localRotation = primaryWeapon.transform.rotation;
    //             instantiatedSecondaryWeapon = Instantiate(secondaryWeapon, offHandTransform.position, Quaternion.identity, offHandTransform).GetComponent<Weapon>();
    //             instantiatedSecondaryWeapon.transform.localRotation = secondaryWeapon.transform.rotation;

    //             instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
    //             instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
    //             break;

    //         case WeaponAttackTypes.TwoHanded:
    //             animator.runtimeAnimatorController = TwoHandedAnimator;
    //             instantiatedPrimaryWeapon = Instantiate(primaryWeapon, mainHandTransform.position, primaryWeapon.transform.rotation, mainHandTransform).GetComponent<Weapon>();
    //             instantiatedPrimaryWeapon.transform.localRotation = primaryWeapon.transform.rotation;

    //             instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
    //             break;
    //     }

    //     instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
    //     if (secondaryWeapon != null)
    //     {
    //         instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand);
    //     }

    //     currentWeaponAttackType = weaponAttackType;
    // }

    private void UnequipOldWeapons()
    {
        if (instantiatedPrimaryWeapon != null)
        {
            Destroy(instantiatedPrimaryWeapon.gameObject);
            instantiatedPrimaryWeapon = null;
        }

        if (instantiatedSecondaryWeapon != null)
        {
            Destroy(instantiatedSecondaryWeapon.gameObject);
            instantiatedSecondaryWeapon = null;
        }
    }

    private void UnequipWeapon(Weapon.WeaponHand weaponHand)
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
        DualWield
    }
}


