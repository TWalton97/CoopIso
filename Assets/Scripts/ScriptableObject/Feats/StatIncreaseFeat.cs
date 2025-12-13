using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Stat Increase Feat")]
public class StatIncreaseFeat : FeatSO
{
    public StatToIncrease Stat;
    public float[] ValuePerLevel;
    [TextArea] public string FeatDescription;

    public bool isPercentage = false;

    public override string GetCurrentLevelDescription(int level)
    {
        if (level == 0) return $"Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)}";

        if (isPercentage)
        {
            return $"Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)} by {CalculateCurrentValue(level)}" + "%";
        }

        return $"Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)} by {CalculateCurrentValue(level)}";
    }

    public override string GetNextLevelDescription(int level)
    {
        if (level >= ValuePerLevel.Length) return "Maximum level reached";

        if (isPercentage)
        {
            return $"Next Level: Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)} by {ValuePerLevel[level]}" + "%";
        }

        return $"Next Level: Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)} by {ValuePerLevel[level]}";
    }

    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {
        IncreaseStat(CurrentFeatLevel, controller.FeatsController);
    }

    public void IncreaseStat(int CurrentFeatLevel, FeatsController controller)
    {
        int safeIndex = Mathf.Clamp(CurrentFeatLevel - 1, 0, ValuePerLevel.Length - 1);
        switch (Stat)
        {
            case StatToIncrease.MaximumHealth:
                controller.PlayerController.HealthController.IncreaseMaximumHealth((int)ValuePerLevel[safeIndex]);
                break;
            case StatToIncrease.MovementSpeed:
                controller.PlayerController.IncreaseMovementSpeed(ValuePerLevel[safeIndex]);
                break;
            case StatToIncrease.CriticalChance:
                controller.PlayerController.PlayerStatsBlackboard.CriticalChance += ValuePerLevel[safeIndex];
                break;
            case StatToIncrease.CriticalDamage:
                controller.PlayerController.PlayerStatsBlackboard.CriticalDamage += ValuePerLevel[safeIndex];
                break;
            case StatToIncrease.HealthRegen:
                controller.PlayerController.HealthController.HealthRegenPerSecond += ValuePerLevel[safeIndex];
                controller.PlayerController.PlayerStatsBlackboard.UpdateHealthStats();
                break;
            case StatToIncrease.MaximumMana:
                controller.PlayerController.ResourceController.resource.resourceMax += ValuePerLevel[safeIndex];
                controller.PlayerController.PlayerStatsBlackboard.UpdateResourceStats();
                break;
            case StatToIncrease.ManaRegen:
                controller.PlayerController.ResourceController.resource.RegenPerSecond += ValuePerLevel[safeIndex];
                controller.PlayerController.PlayerStatsBlackboard.UpdateResourceStats();
                break;
            case StatToIncrease.MaximumCarryWeight:
                controller.PlayerController.PlayerStatsBlackboard.WeightMax += ValuePerLevel[safeIndex];
                break;
            case StatToIncrease.AttackSpeed:
                controller.PlayerController.PlayerStatsBlackboard.AttackSpeedMultiplier += ValuePerLevel[safeIndex] * 0.01f;
                break;
        }
    }

    public float CalculateCurrentValue(int CurrentFeatLevel)
    {
        float val = 0;
        for (int i = 0; i < CurrentFeatLevel; i++)
        {
            val += ValuePerLevel[i];
        }
        return val;
    }
}

[Serializable]
public enum StatToIncrease
{
    MaximumHealth,
    MovementSpeed,
    CriticalChance,
    CriticalDamage,
    HealthRegen,
    MaximumMana,
    ManaRegen,
    MaximumCarryWeight,
    AttackSpeed
}
