using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Stat Increase Feat")]
public class StatIncreaseFeat : FeatSO
{
    public StatToIncrease statToIncrease;
    public float[] ValueIncreasePerLevel;
    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {
        IncreaseStat(CurrentFeatLevel, controller.FeatsController);
    }

    public void IncreaseStat(int CurrentFeatLevel, FeatsController controller)
    {
        int safeIndex = Mathf.Clamp(CurrentFeatLevel, 0, ValueIncreasePerLevel.Length - 1);
        switch (statToIncrease)
        {
            case StatToIncrease.Health:
                controller.playerHealthController.IncreaseMaximumHealth((int)ValueIncreasePerLevel[safeIndex]);
                break;
            case StatToIncrease.MovementSpeed:
                controller.newPlayerController.IncreaseMovementSpeed(ValueIncreasePerLevel[safeIndex]);
                break;
        }
    }
}

[Serializable]
public enum StatToIncrease
{
    Health,
    MovementSpeed
}
