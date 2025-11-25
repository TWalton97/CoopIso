using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Spell Ability")]
public class SpellAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public int AbilityDamage;
    public int AbilityDamageIncreasePerLevel;
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
        return LevelDescriptionTemplate;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        return UpgradeDescriptionTemplate;
    }
}
