public class WeaponRuntimeAbility : RuntimeAbility
{
    public float WeaponDamagePercentage;
    public float WeaponDamageIncreasePerLevel;

    public WeaponRuntimeAbility(WeaponAbility data) : base(data)
    {
        WeaponDamagePercentage = data.WeaponDamagePercentage;
        WeaponDamageIncreasePerLevel = data.WeaponDamageIncreasePerLevel;
    }

    public override void Upgrade()
    {
        currentLevel++;
        //We need to c hange this based on the weaponAbility
        WeaponDamagePercentage += WeaponDamageIncreasePerLevel;
    }
}
