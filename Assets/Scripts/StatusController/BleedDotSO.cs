using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Attack Scaling Damage Over Time Debuff")]
public class BleedDotSO : DamageOverTimeSO
{
    public float percentPerTick = 0.2f;
    public float bleedRemainder = 0;

    protected override int CalculateDamage(StatusInstance instance, StatusController target)
    {
        float baseHit = instance.initialHitDamage * percentPerTick * instance.stacks;

        // Add fractional remainder from previous ticks
        float total = baseHit + bleedRemainder;

        // Damage applied this tick must be an integer
        int damageThisTick = Mathf.FloorToInt(total);

        // Store the leftover fraction for next tick
        bleedRemainder = total - damageThisTick;

        return damageThisTick;
    }
}
