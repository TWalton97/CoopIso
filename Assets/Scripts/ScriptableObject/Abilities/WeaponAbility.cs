using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Weapon Ability")]
public class WeaponAbility : AbilitySO
{
    public List<StatusSO> AppliedStatuses;
    public WeaponRangeType RequiredWeaponRangeType;
    public WeaponAbilityBehaviour abilityBehaviourPrefab;

    public float[] DMG_PerLevel;

    public bool DealsDamageOverTime = false;
    public float[] TICK_PerLevel;

    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new WeaponRuntimeAbility(this);
    }

    public override string GetCurrentLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        float dmg = DMG_PerLevel[currentLevel - 1];
        float tick = 0;
        if (DealsDamageOverTime)
        {
            tick = TICK_PerLevel[currentLevel - 1];
        }

        s = s.Replace("{DMG_CURR}", (dmg * 100).ToString() + "%");
        if (DealsDamageOverTime)
        {
            s = s.Replace("{TICK_CURR}", tick.ToString());
        }

        return s;
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        string s = CurrentLevelDescription;

        float tick = 0;
        if (DealsDamageOverTime)
        {
            tick = TICK_PerLevel[currentLevel - 1];
        }

        s = s.Replace("{DMG_NEXT}", damage.ToString());
        if (DealsDamageOverTime)
        {
            s = s.Replace("{TICK_NEXT}", tick.ToString());
        }

        return s;
    }

    public override string GetNextLevelDescription(int currentLevel)
    {
        if (currentLevel >= DMG_PerLevel.Length)
            return "Max level reached.";

        string s = NextLevelDescription;

        float next = DMG_PerLevel[currentLevel];

        s = s.Replace("{DMG_NEXT}", (next * 100).ToString() + "%");

        if (DealsDamageOverTime)
        {
            next = TICK_PerLevel[currentLevel];

            s = s.Replace("{TICK_NEXT}", next.ToString());
        }

        return s;
    }
}
