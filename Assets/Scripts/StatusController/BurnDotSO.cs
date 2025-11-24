using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Flat Damage Over Time Debuff")]
public class BurnDotSO : DamageOverTimeSO
{
    public int damagePerTick = 2;

    protected override int CalculateDamage(StatusInstance instance, StatusController target)
    {
        Debug.Log($"Calculating damage based on number of stacks. DamagePerTick = {damagePerTick}. Number of stacks = {instance.stacks}. Total damage per tick = {damagePerTick * instance.stacks}");
        return damagePerTick * instance.stacks;
    }
}
