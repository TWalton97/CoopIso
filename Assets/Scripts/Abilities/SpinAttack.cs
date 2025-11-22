using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : BaseAbility
{
    public DamageOverTimeHitbox SpinAttackHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;

    protected SpinAttack(NewPlayerController player, ResourceController resourceController) : base(player, resourceController)
    {
        this.player = player;
        this.resourceController = resourceController;
    }

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

    public override bool CanUse()
    {
        if (resourceController.resource.resourceType == abilityData.ResourceType && resourceController.resource.RemoveResource(abilityData.ResourceAmount)) return true;

        return false;
    }

    public int CalculateDamagePerTick()
    {
        WeaponAbility weaponAbility = abilityData as WeaponAbility;
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * weaponAbility.WeaponDamagePercentage);
    }
}
