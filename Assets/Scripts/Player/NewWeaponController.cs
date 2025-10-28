using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWeaponController : MonoBehaviour
{
    public Animator animator;
    public Weapon weapon;
    public Weapon equippedWeapon;
    public List<Weapon> weapons;

    public void Start()
    {
        //EquipWeapon(weapons[0]);
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (equippedWeapon != null)
        {
            UnequipWeapon();
        }

        foreach (GameObject obj in weapon.WeaponObjects)
        {
            obj.SetActive(true);
        }

        //animator.runtimeAnimatorController = weapon.AnimatorController;

        equippedWeapon = weapon;
    }

    public void UnequipWeapon()
    {
        foreach (GameObject obj in equippedWeapon.WeaponObjects)
        {
            obj.SetActive(false);
        }
    }

    [ContextMenu("Equip Weapon One")]
    public void EquipWeaponOne()
    {
        EquipWeapon(weapons[0]);
    }

    [ContextMenu("Equip Weapon Two")]
    public void EquipWeaponTwo()
    {
        EquipWeapon(weapons[1]);
    }

    [System.Serializable]
    public class Weapon
    {
        public string WeaponName;
        public GameObject[] WeaponObjects;
        public AnimatorOverrideController AnimatorController;

        [System.Serializable]
        public class AttackInfo
        {
            public float comboTime;
            public float movementSpeedDuringAttack;
        }
        public AttackInfo[] AttackInfos;
    }
}


