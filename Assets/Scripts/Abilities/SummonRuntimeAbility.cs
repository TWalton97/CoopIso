using UnityEngine;
public class SummonRuntimeAbility : RuntimeAbility
{
    private SummonAbility data;
    public int Damage;
    public int Health;
    public int MaxSummons;
    public SummonRuntimeAbility(SummonAbility data) : base(data)
    {
        this.data = data;
        this.Damage = data.DamagePerLevel[currentLevel - 1];
        this.Health = data.HealthPerLevel[currentLevel - 1];
        this.MaxSummons = data.MaximumSummonsPerLevel[currentLevel - 1];
    }


    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DamagePerLevel[currentLevel - 1];
        Health = data.HealthPerLevel[currentLevel - 1];
        MaxSummons = data.MaximumSummonsPerLevel[currentLevel - 1];
    }
}