using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : AbilityBehaviour<WeaponRuntimeAbility>
{
    public DamageOverTimeHitbox SpinAttackHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;

    public override void OnEnter()
    {
        instantiatedHitbox = Instantiate(SpinAttackHitbox, player.transform.position, Quaternion.identity, player.transform);
        instantiatedHitbox.Init(CalculateDamagePerTick(), Physics.AllLayers, player, false, 0.2f);
    }

    public override void OnExit()
    {
        if (instantiatedHitbox == null) return;
        Destroy(instantiatedHitbox.gameObject);
    }

    public override bool CanUse(ResourceController resourceController)
    {
        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }

    public int CalculateDamagePerTick()
    {
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * runtime.WeaponDamagePercentage);
    }

    public override void OnUse()
    {

    }
}
