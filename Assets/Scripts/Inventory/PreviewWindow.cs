using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreviewWindow : MonoBehaviour
{
    public TMP_Text ItemName;
    public TMP_Text ItemType;
    public TMP_Text ItemStats;

    public void FillPreviewWindow(ItemData itemData)
    {
        ItemName.text = itemData.itemName;
        ItemType.text = itemData.itemType.ToString();

        if (itemData.data is WeaponDataSO)
        {
            WeaponDataSO weaponData = itemData.data as WeaponDataSO;
            ItemStats.text =
            "Weapon Damage: " + weaponData.WeaponDamage +
            "\nNumber of Attacks: " + weaponData.NumberOfAttacksInCombo +
            "\nMovement Speed: " + weaponData.MovementSpeedDuringAttack;
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
            ItemStats.text =
            "BlockAngle: " + shieldData.BlockAngle +
            "\nBlockAmount: " + shieldData.BlockAmount;
        }
        else
        {
            ItemStats.text = "";
        }
    }
}
