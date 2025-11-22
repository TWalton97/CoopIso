using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewManager : Singleton<PlayerPreviewManager>
{
    public PreviewPlayerController playerOneObject;
    public PreviewPlayerController playerTwoObject;

    public void EquipArmorToPlayer(int index, ItemType itemType, GameObject prefab)
    {
        if (index == 0)
        {
            playerOneObject.EquipArmorToSlot(itemType, prefab);
        }
        else if (index == 1)
        {
            playerTwoObject.EquipArmorToSlot(itemType, prefab);
        }
    }

    public void UnequipArmorFromPlayer(int index, ItemType itemType)
    {
        if (index == 0)
        {
            playerOneObject.UnequipArmor(itemType);
        }
        else if (index == 1)
        {
            playerTwoObject.UnequipArmor(itemType);
        }
    }

    public void EquipWeaponToPlayer(int index, Weapon.WeaponHand weaponHand, GameObject prefab)
    {
        if (index == 0)
        {
            playerOneObject.EquipWeaponToSlot(weaponHand, prefab);
        }
        else if (index == 1)
        {
            playerTwoObject.EquipWeaponToSlot(weaponHand, prefab);
        }
    }

    public void UnequipWeaponFromPlayer(int index, Weapon.WeaponHand weaponHand)
    {
        if (index == 0)
        {
            playerOneObject.UnequipWeapon(weaponHand);
        }
        else if (index == 1)
        {
            playerTwoObject.UnequipWeapon(weaponHand);
        }
    }
}
