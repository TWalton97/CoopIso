public class SpellRuntimeAbility : RuntimeAbility
{
    public SpellAbility data;
    public int Damage;

    public SpellRuntimeAbility(SpellAbility data) : base(data)
    {
        this.data = data;
        Damage = data.DamagePerLevel[currentLevel - 1];
    }

    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DamagePerLevel[currentLevel - 1];
    }
}
