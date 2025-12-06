using UnityEngine;

public class SpinAttackBehaviour : WeaponAbilityBehaviour
{
    public DamageOverTimeHitbox SpinAttackHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;
    public float CurrentWeaponDamagePercentage;
    public override void OnUse()
    {

    }
    public override void OnEnter()
    {
        CurrentWeaponDamagePercentage = runtime.WeaponDamage;
        instantiatedHitbox = Instantiate(SpinAttackHitbox, player.transform.position, Quaternion.identity, player.transform);
        instantiatedHitbox.Init(CalculateDamagePerTick(), Physics.AllLayers, player, false, runtime.TickRate, runtime.DealsDamageOverTime, statuses);
    }

    public override void OnExit()
    {
        if (instantiatedHitbox == null) return;
        Destroy(instantiatedHitbox.gameObject);
    }

    public override int CalculateDamagePerTick()
    {
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * runtime.WeaponDamage);
    }

    public override void OnChannelTick(float deltaTime)
    {

    }
}
