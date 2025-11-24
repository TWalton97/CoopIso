using System.Collections;
using UnityEngine;

public class StompAttackBehaviour : WeaponAbilityBehaviour
{
    public DamageOverTimeHitbox SpinAttackHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;
    public float CurrentWeaponDamagePercentage;
    public float HitboxSpawnDelay;
    public override void OnUse()
    {

    }
    public override void OnEnter()
    {
        CurrentWeaponDamagePercentage = runtime.WeaponDamagePercentage;
        StartCoroutine(SpawnHitboxAfterDelay());
    }

    public override void OnExit()
    {
        if (instantiatedHitbox == null) return;
        Destroy(instantiatedHitbox.gameObject);
    }
    public int CalculateDamagePerTick()
    {
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * runtime.WeaponDamagePercentage);
    }

    private IEnumerator SpawnHitboxAfterDelay()
    {
        yield return new WaitForSeconds(HitboxSpawnDelay);
        instantiatedHitbox = Instantiate(SpinAttackHitbox, player.transform.position, player.transform.rotation);
        instantiatedHitbox.Init(CalculateDamagePerTick(), Physics.AllLayers, player, false, 0.2f, false, statuses);
        yield return null;
    }
}
