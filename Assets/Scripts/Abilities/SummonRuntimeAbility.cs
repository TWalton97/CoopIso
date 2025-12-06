using UnityEngine;
public class SummonRuntimeAbility : RuntimeAbility
{
    private SummonAbility data;
    public int Damage;
    public SummonRuntimeAbility(SummonAbility data) : base(data)
    {
        this.data = data;
        this.Damage = data.DamagePerLevel[currentLevel - 1];
    }


    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DamagePerLevel[currentLevel - 1];
    }
}