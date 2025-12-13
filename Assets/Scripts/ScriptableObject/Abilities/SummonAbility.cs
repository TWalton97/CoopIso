using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Summon Ability")]
public class SummonAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public SummonAbilityBehaviour abilityBehaviourPrefab;

    public int[] DamagePerLevel;
    public int[] HealthPerLevel;
    public int[] MaximumSummonsPerLevel;

    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new SummonRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        return "";
    }

    public override string GetLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        int val = DamagePerLevel[currentLevel - 1];
        int healthVal = HealthPerLevel[currentLevel - 1];
        int sumVal = MaximumSummonsPerLevel[currentLevel - 1];

        s = s.Replace("{DMG}", val.ToString());
        s = s.Replace("{HEALTH}", healthVal.ToString());
        s = s.Replace("{NUM_SUMMONS}", sumVal.ToString());

        return s;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        string s = NextLevelDescription;

        int curr = DamagePerLevel[currentLevel - 1];
        int next = DamagePerLevel[currentLevel];

        int healthValCurr = HealthPerLevel[currentLevel - 1];
        int healthValNext = HealthPerLevel[currentLevel];

        int sumValCurr = MaximumSummonsPerLevel[currentLevel - 1];
        int sumValNext = MaximumSummonsPerLevel[currentLevel];

        s = s.Replace("{DMG_CURR}", curr.ToString());
        s = s.Replace("{DMG_NEXT}", next.ToString());

        s = s.Replace("{HEALTH_CURR}", healthValCurr.ToString());
        s = s.Replace("{HEALTH_NEXT}", healthValNext.ToString());

        s = s.Replace("{NUM_SUMMONS_CURR}", sumValCurr.ToString());
        s = s.Replace("{NUM_SUMMONS_NEXT}", sumValNext.ToString());

        return s;
    }
}
