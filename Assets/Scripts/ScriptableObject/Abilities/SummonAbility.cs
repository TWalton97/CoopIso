using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Summon Ability")]
public class SummonAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public SummonAbilityBehaviour abilityBehaviourPrefab;

    public int[] DMG_PerLevel;
    public int[] HEALTH_PerLevel;
    public int[] NUM_SUMMONS_PerLevel;

    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new SummonRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        return "";
    }

    public override string GetCurrentLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        int val = DMG_PerLevel[currentLevel - 1];
        int healthVal = HEALTH_PerLevel[currentLevel - 1];
        int sumVal = NUM_SUMMONS_PerLevel[currentLevel - 1];

        s = s.Replace("{DMG_CURR}", val.ToString());
        s = s.Replace("{HEALTH_CURR}", healthVal.ToString());
        s = s.Replace("{NUM_SUMMONS_CURR}", sumVal.ToString());

        return s;
    }

    public override string GetNextLevelDescription(int currentLevel)
    {
        string s = NextLevelDescription;

        int next = DMG_PerLevel[currentLevel];

        int healthValNext = HEALTH_PerLevel[currentLevel];

        int sumValNext = NUM_SUMMONS_PerLevel[currentLevel];

        s = s.Replace("{DMG_NEXT}", next.ToString());

        s = s.Replace("{HEALTH_NEXT}", healthValNext.ToString());

        s = s.Replace("{NUM_SUMMONS_NEXT}", sumValNext.ToString());

        return s;
    }
}
