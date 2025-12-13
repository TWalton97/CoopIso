using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Passive Unlock Feat")]

public class PassiveUnlockFeat : FeatSO
{
    public PassiveUnlock passiveUnlock;
    [TextArea] public string FeatDescription;
    public string[] FeatUpgradeDescriptionPerLevel;

    public override string GetCurrentLevelDescription(int level)
    {
        if (level == 0) return "";

        return FeatUpgradeDescriptionPerLevel[level - 1];
    }

    public override string GetNextLevelDescription(int level)
    {
        if (level >= FeatUpgradeDescriptionPerLevel.Length) return "Maximum level reached";

        return "Next Level: " + FeatUpgradeDescriptionPerLevel[level];
    }

    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {
        UnlockPassive(controller, CurrentFeatLevel);
    }

    private void UnlockPassive(NewPlayerController controller, int currentFeatLevel)
    {
        switch (passiveUnlock)
        {
            case PassiveUnlock.UnarmedMastery:
                controller.PlayerStatsBlackboard.UnarmedMastery = true;
                break;
            case PassiveUnlock.OneHandedMastery:
                controller.PlayerStatsBlackboard.OneHandedMastery = true;
                break;
            case PassiveUnlock.TwoHandedMastery:
                controller.PlayerStatsBlackboard.TwoHandedMastery = true;
                break;
            case PassiveUnlock.DualWieldMastery:
                controller.PlayerStatsBlackboard.DualWieldMastery = true;
                break;
            case PassiveUnlock.BowMastery:
                controller.PlayerStatsBlackboard.BowMastery = true;
                break;
            case PassiveUnlock.ArmorMastery:
                if (currentFeatLevel == 1)
                {
                    controller.PlayerStatsBlackboard.armorType = ArmorType.Light;
                }
                else if (currentFeatLevel == 2)
                {
                    controller.PlayerStatsBlackboard.armorType = ArmorType.Medium;
                    controller.PlayerStatsBlackboard.CanEquipShields = true;
                }
                else if (currentFeatLevel == 3)
                {
                    controller.PlayerStatsBlackboard.armorType = ArmorType.Heavy;
                }
                break;
        }
    }
}

public enum PassiveUnlock
{
    UnarmedMastery,
    OneHandedMastery,
    TwoHandedMastery,
    DualWieldMastery,
    BowMastery,
    ArmorMastery
}
