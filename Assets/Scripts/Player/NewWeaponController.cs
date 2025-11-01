using System;
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
    public bool primaryWeaponAttackCompleted = false;

    private CountdownTimer comboCounter;    //We use this to determine when to reset our current attack combo
    private float comboPeriod = 3;
    int numAttacks;

    public bool canAttack = true;
    public bool HasShieldEquipped;
    private Action OnActionCompleted;

    protected void Awake()
    {
        comboCounter = new CountdownTimer(comboPeriod);
        newPlayerController = GetComponent<NewPlayerController>();
    }

    void Start()
    {
        newPlayerController.AnimationStatusTracker.OnAnimationCompleted += ResetAttack;
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
            if (numAttacks >= 3)
            {
                numAttacks = 0;
            }
            return;
        }

        instantiatedPrimaryWeapon.Enter(OnActionCompleted, numAttacks);
        numAttacks++;
        canAttack = false;
        comboCounter.Start();

        //If the combo exceeds 3, reset
        if (numAttacks >= instantiatedPrimaryWeapon.Data.NumberOfAttacksInCombo)
        {
            numAttacks = 0;
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        OnActionCompleted.Invoke();
    }

    public void EquipOneHandedWeapon(GameObject weaponPrefab)
    {
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
        if (instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.Shield)
        {
            HasShieldEquipped = true;
        }
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        primaryWeaponAttackCompleted = false;
        canAttack = true;

        //If you have no weapons equipped, enter unarmed

        if (instantiatedPrimaryWeapon == null && instantiatedSecondaryWeapon == null)
        {
            animator.runtimeAnimatorController = UnarmedAnimator;
        }
        else if (instantiatedPrimaryWeapon == null && HasShieldEquipped)
        {
            animator.runtimeAnimatorController = UnarmedAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded && HasShieldEquipped)
        {
            animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
        {
            animator.runtimeAnimatorController = DualWieldAnimator;
        }
        else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        {
            animator.runtimeAnimatorController = TwoHandedAnimator;
        }

        // if (instantiatedPrimaryWeapon == null)
        //     {
        //         animator.runtimeAnimatorController = UnarmedAnimator;
        //     }
        //     else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
        //     {
        //         animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        //     }
        //     else if (instantiatedPrimaryWeapon.weaponAttackType == WeaponAttackTypes.TwoHanded)
        //     {
        //         animator.runtimeAnimatorController = TwoHandedAnimator;
        //     }
        //     else if (instantiatedSecondaryWeapon != null)
        //     {
        //         if (instantiatedSecondaryWeapon.weaponAttackType == WeaponAttackTypes.OneHanded)
        //         {
        //             animator.runtimeAnimatorController = DualWieldAnimator;
        //         }
        //     }
        //     else
        //     {
        //         animator.runtimeAnimatorController = OneHandedAndShieldAnimator;
        //     }
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

    [System.Serializable]
    public enum WeaponAttackTypes
    {
        OneHanded,
        TwoHanded,
        DualWield,
        Shield
    }
}


