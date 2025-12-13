public class SpellRuntimeAbility : RuntimeAbility
{
    public SpellAbility data;
    public int Damage;

    public SpellRuntimeAbility(SpellAbility data) : base(data)
    {
        this.data = data;
        Damage = data.DMG_PerLevel[currentLevel - 1];
    }

    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DMG_PerLevel[currentLevel - 1];
    }
}
