using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Attack Scaling Damage Over Time Debuff")]
public class BleedDotSO : DamageOverTimeSO
{
    public float percentPerTick = 0.2f;

    protected override int CalculateDamage(StatusInstance instance, StatusController target)
    {
        // Flat damage, scales with stacks if desired
        int baseHit = instance.initialHitDamage;

        int damage = (int)(baseHit * percentPerTick);

        return damage * instance.stacks;
    }
}
