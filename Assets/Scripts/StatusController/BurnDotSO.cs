using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Flat Damage Over Time Debuff")]
public class BurnDotSO : DamageOverTimeSO
{
    public int damagePerTick = 2;

    protected override int CalculateDamage(StatusInstance instance, StatusController target)
    {
        return damagePerTick * instance.stacks;
    }
}
