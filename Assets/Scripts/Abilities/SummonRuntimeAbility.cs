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
        this.Damage = data.DMG_PerLevel[currentLevel - 1];
        this.Health = data.HEALTH_PerLevel[currentLevel - 1];
        this.MaxSummons = data.NUM_SUMMONS_PerLevel[currentLevel - 1];
    }


    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DMG_PerLevel[currentLevel - 1];
        Health = data.HEALTH_PerLevel[currentLevel - 1];
        MaxSummons = data.NUM_SUMMONS_PerLevel[currentLevel - 1];
    }
}