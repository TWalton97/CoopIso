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

    public void FillPreviewWindow(ItemData itemData)
    {
        ItemName.text = itemData.itemName;
        ItemType.text = itemData.itemType.ToString();
        ItemDescription.text = itemData.itemDescription;

        if (itemData.data is WeaponDataSO)
        {
            WeaponDataSO weaponData = itemData.data as WeaponDataSO;
            SpawnedItemDataBase.SpawnedWeaponsData spawnedWeaponsData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedWeaponsData;
            ItemStats.text =
            "Weapon Damage: " + spawnedWeaponsData.weaponMinDamage + "-" + spawnedWeaponsData.weaponMaxDamage +
            "\nAttacks per Second: " + spawnedWeaponsData.attacksPerSecond.ToString("0.00") +
            "\nMovement Speed: " + weaponData.MovementSpeedMultiplierDuringAttack +
            "\nDPS: " + ((spawnedWeaponsData.weaponMinDamage + spawnedWeaponsData.weaponMaxDamage) / 2 * spawnedWeaponsData.attacksPerSecond).ToString("0.00");
            ItemAffixes.text = BuildAffixString(itemData);
        }
        else if (itemData.data is PotionSO)
        {
            PotionSO potionData = itemData.data as PotionSO;
            ItemStats.text =
            "Resource Type: " + potionData.PotionData[0].ResourceToRestore +
            "\nAmount Restored: " + potionData.PotionData[0].AmountOfResourceToRestore +
            "\nRestore Duration: " + potionData.PotionData[0].RestoreDuration;
        }
        else if (itemData.data is ShieldSO)
        {
            ShieldSO shieldData = itemData.data as ShieldSO;
            List<ShieldAffix> shieldAffixes = AffixListConverter.ConvertListIntoShieldAffixes(itemData.affixes);
            ItemStats.text =
            "BlockAngle: " + (shieldData.BlockAngle + AffixStatCalculator.CalculateBlockAngle(shieldAffixes)).ToString() +
            "\nBlockAmount: " + (shieldData.BlockAmount + AffixStatCalculator.CalculateBlockAmount(shieldAffixes)).ToString();
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
