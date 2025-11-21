using UnityEngine;
[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Armor Data/Body")]
public class ArmorSO : ItemSO
{
    public ArmorType ArmorType;
    public int ArmorAmount;

    public override bool CheckItemRequirements(PlayerStatsBlackboard playerStatsBlackboard)
    {
        if (ArmorType == ArmorType.Heavy)
        {
            if (playerStatsBlackboard.armorType == ArmorType.Heavy)
            {
                return true;
            }
        }
        else if (ArmorType == ArmorType.Medium)
        {
            if (playerStatsBlackboard.armorType == ArmorType.Heavy || playerStatsBlackboard.armorType == ArmorType.Medium)
            {
                return true;
            }
        }
        else if (ArmorType == ArmorType.Light)
        {
            if (playerStatsBlackboard.armorType == ArmorType.Heavy || playerStatsBlackboard.armorType == ArmorType.Medium || playerStatsBlackboard.armorType == ArmorType.Light)
            {
                return true;
            }
        }
        return false;
    }
}

public enum ArmorType
{
    None,
    Light,
    Medium,
    Heavy
}
