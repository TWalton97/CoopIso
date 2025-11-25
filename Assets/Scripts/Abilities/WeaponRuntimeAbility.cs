public class WeaponRuntimeAbility : RuntimeAbility
{
    private WeaponAbility data;
    public float WeaponDamage;
    public bool DealsDamageOverTime = false;
    public float TickRate;

    public WeaponRuntimeAbility(WeaponAbility data) : base(data)
    {
        this.data = data;
        WeaponDamage = data.DamagePercentPerLevel[currentLevel - 1];
        DealsDamageOverTime = data.DealsDamageOverTime;
        if (DealsDamageOverTime)
            TickRate = data.TickRatePerLevel[currentLevel - 1];
    }

    public override void Upgrade()
    {
        currentLevel++;
        WeaponDamage = data.DamagePercentPerLevel[currentLevel - 1];
        DealsDamageOverTime = data.DealsDamageOverTime;
        if (DealsDamageOverTime)
            TickRate = data.TickRatePerLevel[currentLevel - 1];
    }
}
