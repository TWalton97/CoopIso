using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class NewWeaponController : MonoBehaviour
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

    private void Awake()
    {
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

    public void EquipWeapon(WeaponAttackTypes weaponAttackType, Weapon primaryWeapon, Weapon secondaryWeapon = null)
    {
        UnequipOldWeapons();

        switch (weaponAttackType)
        {
            case WeaponAttackTypes.OneHandedAndShield:
                animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
                instantiatedPrimaryWeapon = Instantiate(primaryWeapon, mainHandTransform.position, Quaternion.identity, mainHandTransform);
                instantiatedPrimaryWeapon.transform.localRotation = primaryWeapon.transform.rotation;
                instantiatedSecondaryWeapon = Instantiate(secondaryWeapon, offHandTransform.position, Quaternion.identity, offHandTransform);
                instantiatedSecondaryWeapon.transform.localRotation = secondaryWeapon.transform.rotation;

                instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
                instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
                break;

            case WeaponAttackTypes.DualWield:
                animator.runtimeAnimatorController = DualWieldAnimator;
                instantiatedPrimaryWeapon = Instantiate(primaryWeapon, mainHandTransform.position, Quaternion.identity, mainHandTransform);
                instantiatedPrimaryWeapon.transform.localRotation = primaryWeapon.transform.rotation;
                instantiatedSecondaryWeapon = Instantiate(secondaryWeapon, offHandTransform.position, Quaternion.identity, offHandTransform);
                instantiatedSecondaryWeapon.transform.localRotation = secondaryWeapon.transform.rotation;

                instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
                instantiatedSecondaryWeapon.SetPlayer(newPlayerController);
                break;

            case WeaponAttackTypes.TwoHanded:
                animator.runtimeAnimatorController = TwoHandedAnimator;
                instantiatedPrimaryWeapon = Instantiate(primaryWeapon, mainHandTransform.position, primaryWeapon.transform.rotation, mainHandTransform);
                instantiatedPrimaryWeapon.transform.localRotation = primaryWeapon.transform.rotation;

                instantiatedPrimaryWeapon.SetPlayer(newPlayerController);
                break;
        }

        instantiatedPrimaryWeapon.Init(Weapon.WeaponHand.MainHand);
        if (secondaryWeapon != null)
        {
            instantiatedSecondaryWeapon.Init(Weapon.WeaponHand.OffHand);
        }

        currentWeaponAttackType = weaponAttackType;
    }

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

    public AnimatorOverrideController OneHandedAndShieldAnimator;
    public AnimatorOverrideController DualWieldAnimator;
    public AnimatorOverrideController TwoHandedAnimator;

    [System.Serializable]
    public enum WeaponAttackTypes
    {
        OneHandedAndShield,
        DualWield,
        TwoHanded
    }
}


