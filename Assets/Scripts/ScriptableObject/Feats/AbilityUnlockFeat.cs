using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Ability Unlock Feat")]
public class AbilityUnlockFeat : FeatSO
{
    public AbilitySO AbilityToUnlock;

    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller)
    {
        controller.AbilityController.UnlockAbility(AbilityToUnlock);

    }

    public override string GenerateStatDescriptionString(int currentFeatLevel)
    {
        if (currentFeatLevel == 0)
        {
            return $"Next Level: {FeatUnlockDescription}";
        }
        else if (currentFeatLevel == MaximumFeatLevel)
        {
            return $"Maximum Level Reached";
        }
        else
        {
            if (AbilityToUnlock is WeaponAbility weaponAbility)
            {
                return $"Next Level: {FeatUpgradeDescription} from {((weaponAbility.WeaponDamagePercentage + (weaponAbility.WeaponDamageIncreasePerLevel * (currentFeatLevel - 1))) * 100f).ToString("0")}% to {((weaponAbility.WeaponDamagePercentage + (weaponAbility.WeaponDamageIncreasePerLevel * currentFeatLevel)) * 100f).ToString("0")}%";
            }
            else
            {
                return $"Next Level: {FeatUpgradeDescription}";
            }
        }
    }

    public override string GenerateNextLevelString(int currentFeatLevel)
    {
        if (currentFeatLevel == 0)
        {
            return $"Next Level: {FeatUnlockDescription}";
        }
        else if (currentFeatLevel == MaximumFeatLevel)
        {
            return $"Maximum Level Reached";
        }
        else
        {
            if (AbilityToUnlock is WeaponAbility weaponAbility)
            {
                return $"Next Level: {FeatUpgradeDescription} from {((weaponAbility.WeaponDamagePercentage + (weaponAbility.WeaponDamageIncreasePerLevel * (currentFeatLevel - 1))) * 100f).ToString("0")}% to {((weaponAbility.WeaponDamagePercentage + (weaponAbility.WeaponDamageIncreasePerLevel * currentFeatLevel)) * 100f).ToString("0")}%";
            }
            else
            {
                return $"Next Level: {FeatUpgradeDescription}";
            }
        }
    }
}
