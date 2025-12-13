using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Spell Ability")]
public class SpellAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public int[] DamagePerLevel;
    public SpellAbilityBehaviour abilityBehaviourPrefab;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new SpellRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override string GetLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        float dam = DamagePerLevel[currentLevel - 1];

        s = s.Replace("{DAM}", dam.ToString());

        return s;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        string s = NextLevelDescription;

        float curr = DamagePerLevel[currentLevel - 1];
        float next = DamagePerLevel[currentLevel];

        s = s.Replace("{DAM_CURR}", curr.ToString());
        s = s.Replace("{DAM_NEXT}", next.ToString());

        return s;
    }
}
