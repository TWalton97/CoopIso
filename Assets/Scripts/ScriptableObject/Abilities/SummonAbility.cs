using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Summon Ability")]
public class SummonAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public SummonAbilityBehaviour abilityBehaviourPrefab;

    public int[] DamagePerLevel;

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
        string s = LevelDescriptionTemplate;

        int val = DamagePerLevel[currentLevel - 1];

        s = s.Replace("{DMG}", val.ToString());

        return s;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        string s = UpgradeDescriptionTemplate;

        int curr = DamagePerLevel[currentLevel - 1];
        int next = DamagePerLevel[currentLevel];

        s = s.Replace("{DMG_CURR}", curr.ToString());
        s = s.Replace("{DMG_NEXT}", next.ToString());

        return s;
    }
}
