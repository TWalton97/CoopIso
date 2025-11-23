using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainOfArrowBehaviour : WeaponAbilityBehaviour
{
    public DamageOverTimeHitbox SpinAttackHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;
    public float CurrentWeaponDamagePercentage;
    public float HitboxSpawnDelay;
    public float HitboxDuration;
    public LayerMask ObstructionLayers;
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

    }

    public int CalculateDamagePerTick()
    {
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * runtime.WeaponDamagePercentage);
    }

    private IEnumerator SpawnHitboxAfterDelay()
    {
        yield return new WaitForSeconds(HitboxSpawnDelay);
        RaycastHit hit;
        Vector3 spawnPos;
        if (Physics.SphereCast(player.transform.position + Vector3.up, 1f, player.transform.forward, out hit, 10f, ObstructionLayers))
        {
            spawnPos = new Vector3(hit.point.x, 0f, hit.point.z);
        }
        else
        {
            spawnPos = transform.position + transform.forward * 10f;
            spawnPos.y = 0;
        }

        instantiatedHitbox = Instantiate(SpinAttackHitbox, spawnPos, Quaternion.identity, null);
        instantiatedHitbox.Init(CalculateDamagePerTick(), Physics.AllLayers, player, false, 0.5f, true);
        Destroy(instantiatedHitbox.gameObject, HitboxDuration);
        yield return null;
    }
}
