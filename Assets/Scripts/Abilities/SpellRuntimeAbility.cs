public class SpellRuntimeAbility : RuntimeAbility
{
    public int AbilityDamage;
    public int AbilityDamageIncreasePerLevel;

    public SpellRuntimeAbility(SpellAbility data) : base(data)
    {
        AbilityDamage = data.AbilityDamage;
        AbilityDamageIncreasePerLevel = data.AbilityDamageIncreasePerLevel;
    }

    public override void Upgrade()
    {
        currentLevel++;
        AbilityDamage += AbilityDamageIncreasePerLevel;
    }
}
