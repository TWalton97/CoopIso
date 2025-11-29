using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreviewWindow : MonoBehaviour
{
    public TMP_Text ItemName;
    public TMP_Text ItemType;
    public TMP_Text ItemStats;
    public TMP_Text ItemAffixes;
    public TMP_Text ItemDescription;

    public void FillPreviewWindow(ItemData itemData, InventoryController inventoryController)
    {
        ItemName.text = itemData.itemName;
        ItemType.text = itemData.itemType.ToString();
        ItemDescription.text = itemData.itemDescription;

        if (itemData.data is WeaponDataSO)
        {
            WeaponDataSO weaponData = itemData.data as WeaponDataSO;
            SpawnedItemDataBase.SpawnedWeaponsData spawnedWeaponsData = inventoryController.PlayerContext.UserInterfaceController.playerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedWeaponsData;
            ItemStats.text =
            "Weapon Damage: " + weaponData.WeaponMinDamage + "-" + weaponData.WeaponMaxDamage + " <color=#FFE500>(" + spawnedWeaponsData.weaponMinDamage + "-" + spawnedWeaponsData.weaponMaxDamage + ")</color>" +
            "\nAttacks per Second: " + weaponData.AttacksPerSecond + " <color=#FFE500>(" + spawnedWeaponsData.attacksPerSecond.ToString("0.00") + ")</color>" +
            "\nDPS: " + ((spawnedWeaponsData.weaponMinDamage + spawnedWeaponsData.weaponMaxDamage) / 2 * spawnedWeaponsData.attacksPerSecond).ToString("0.00");
            ItemAffixes.text = BuildAffixString(itemData);
        }
        else if (itemData.data is ShieldSO)
        {
            ShieldSO shieldData = itemData.data as ShieldSO;
            SpawnedItemDataBase.SpawnedShieldData spawnedShieldData = inventoryController.PlayerContext.UserInterfaceController.playerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedShieldData;
            ItemStats.text =
            "BlockAngle: " + shieldData.BlockAngle + " <color=#FFE500>(" + spawnedShieldData.blockAngle + ")</color>" +
            "\nBlockAmount: " + shieldData.BlockAmount + " <color=#FFE500>(" + spawnedShieldData.blockAmount + ")</color>" +
            "\nArmor Amount: " + shieldData.ArmorAmount + " <color=#FFE500>(" + spawnedShieldData.armorAmount + ")</color>";
            ItemAffixes.text = BuildAffixString(itemData);
        }
        else if (itemData.data is BowSO)
        {
            BowSO weaponData = itemData.data as BowSO;
            SpawnedItemDataBase.SpawnedBowData spawnedWeaponsData = inventoryController.PlayerContext.UserInterfaceController.playerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedBowData;
            ItemStats.text =
            "Weapon Damage: " + weaponData.WeaponMinDamage + "-" + weaponData.WeaponMaxDamage + " <color=#FFE500>(" + spawnedWeaponsData.weaponMinDamage + "-" + spawnedWeaponsData.weaponMaxDamage + ")</color>" +
            "\nAttacks per Second: " + weaponData.AttacksPerSecond + " <color=#FFE500>(" + spawnedWeaponsData.attacksPerSecond.ToString("0.00") + ")</color>" +
            "\nNumber of Projectiles: " + weaponData.NumberOfProjectiles + " <color=#FFE500>(" + spawnedWeaponsData.numberOfProjectiles + ")</color>" +
            "\nDPS: " + ((spawnedWeaponsData.weaponMinDamage + spawnedWeaponsData.weaponMaxDamage) / 2 * spawnedWeaponsData.attacksPerSecond).ToString("0.00");
            ItemAffixes.text = BuildAffixString(itemData);
        }
        else if (itemData.data is ArmorSO)
        {
            ArmorSO armorData = itemData.data as ArmorSO;
            SpawnedItemDataBase.SpawnedArmorData spawnedWeaponsData = inventoryController.PlayerContext.UserInterfaceController.playerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            ItemStats.text =
            "Armor Type: " + spawnedWeaponsData.armorType.ToString() +
            "\nArmor Amount: " + armorData.ArmorAmount + " <color=#FFE500>(" + spawnedWeaponsData.armorAmount + ")</color>";
            ItemAffixes.text = BuildAffixString(itemData);
        }
        else
        {
            ItemStats.text = "";
        }
    }

    public string BuildAffixString(ItemData itemData)
    {
        string fullString = "";

        for (int i = 0; i < itemData.affixes.Count; i++)
        {
            if (i == 0)
            {
                fullString += AffixStringBuilder.BuildStringBasedOnAffix(itemData.affixes[i]);
            }
            else
            {
                fullString += "\n" + AffixStringBuilder.BuildStringBasedOnAffix(itemData.affixes[i]);
            }
        }

        return fullString;
    }

}
