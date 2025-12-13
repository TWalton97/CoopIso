using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Spell Ability")]
public class SpellAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public int[] DMG_PerLevel;
    public SpellAbilityBehaviour abilityBehaviourPrefab;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new SpellRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override string GetCurrentLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        float dam = DMG_PerLevel[currentLevel - 1];

        s = s.Replace("{DMG_CURR}", dam.ToString());

        return s;
    }

    public override string GetNextLevelDescription(int currentLevel)
    {
        string s = NextLevelDescription;

        float next = DMG_PerLevel[currentLevel];

        s = s.Replace("{DMG_NEXT}", next.ToString());

        return s;
    }
}
