using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Weapon Ability")]
public class WeaponAbility : AbilitySO
{
    public List<StatusSO> AppliedStatuses;
    public WeaponRangeType RequiredWeaponRangeType;
    public WeaponAbilityBehaviour abilityBehaviourPrefab;

    public float[] DamagePercentPerLevel;

    public bool DealsDamageOverTime = false;
    public float[] TickRatePerLevel;

    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new WeaponRuntimeAbility(this);
    }

    public override string GetLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        float dmg = DamagePercentPerLevel[currentLevel - 1];
        float tick = 0;
        if (DealsDamageOverTime)
        {
            tick = TickRatePerLevel[currentLevel - 1];
        }

        s = s.Replace("{DMG}", (dmg * 100).ToString() + "%");
        if (DealsDamageOverTime)
        {
            s = s.Replace("{TICK}", tick.ToString());
        }

        return s;
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        string s = CurrentLevelDescription;

        float tick = 0;
        if (DealsDamageOverTime)
        {
            tick = TickRatePerLevel[currentLevel - 1];
        }

        s = s.Replace("{DMG}", damage.ToString());
        if (DealsDamageOverTime)
        {
            s = s.Replace("{TICK}", tick.ToString());
        }

        return s;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        if (currentLevel >= DamagePercentPerLevel.Length)
            return "Max level reached.";

        string s = NextLevelDescription;

        float curr = DamagePercentPerLevel[currentLevel - 1];
        float next = DamagePercentPerLevel[currentLevel];

        s = s.Replace("{DMG_CURR}", (curr * 100).ToString() + "%");
        s = s.Replace("{DMG_NEXT}", (next * 100).ToString() + "%");

        if (DealsDamageOverTime)
        {
            curr = TickRatePerLevel[currentLevel - 1];
            next = TickRatePerLevel[currentLevel];

            s = s.Replace("{TICK_CURR}", curr.ToString());
            s = s.Replace("{TICK_NEXT}", next.ToString());
        }

        return s;
    }
}
