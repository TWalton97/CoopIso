using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Stat Increase Feat")]
public class StatIncreaseFeat : FeatSO
{
    public StatToIncrease Stat;
    public float[] ValuePerLevel;
    [TextArea] public string FeatDescription;

    public override string GetCurrentEffect(int level)
    {
        if (level == 0) return $"Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)}";

        return $"Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)} by {CalculateCurrentValue(level)}";
    }

    public override string GetNextLevelEffect(int level)
    {
        if (level >= ValuePerLevel.Length) return "Maximum level reached";

        return $"Next Level: Increases {Utilities.EnumExtension.ConvertToDisplayName(Stat)} by {ValuePerLevel[level]}";
    }

    public override string GetDescription()
    {
        return FeatDescription;
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
                controller.playerHealthController.IncreaseMaximumHealth((int)ValuePerLevel[safeIndex]);
                break;
            case StatToIncrease.MovementSpeed:
                controller.newPlayerController.IncreaseMovementSpeed(ValuePerLevel[safeIndex]);
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
    MovementSpeed
}
