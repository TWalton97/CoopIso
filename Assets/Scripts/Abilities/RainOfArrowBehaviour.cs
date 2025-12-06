using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RainOfArrowBehaviour : WeaponAbilityBehaviour
{
    public DamageOverTimeHitbox SpinAttackHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;
    public float CurrentWeaponDamagePercentage;
    public float HitboxSpawnDelay;
    public float HitboxDuration;
    public float HitboxMoveSpeed;
    public LayerMask EnemyLayer;
    public LayerMask WallLayer;
    public override void OnUse()
    {

    }
    public override void OnEnter()
    {
        CurrentWeaponDamagePercentage = runtime.WeaponDamage;
        StartCoroutine(SpawnHitboxAfterDelay());
    }

    public override void OnExit()
    {

    }

    public override int CalculateDamagePerTick()
    {
        return Mathf.CeilToInt(player.WeaponController.CombinedWeaponDamage * runtime.WeaponDamage);
    }

    private IEnumerator SpawnHitboxAfterDelay()
    {
        yield return new WaitForSeconds(HitboxSpawnDelay);
        // RaycastHit hit;
        Vector3 spawnPos;
        Vector3 dir;
        spawnPos = transform.position + transform.forward;
        dir = transform.forward;

        instantiatedHitbox = Instantiate(SpinAttackHitbox, spawnPos, Quaternion.identity, null);
        instantiatedHitbox.Init(CalculateDamagePerTick(), Physics.AllLayers, player, false, runtime.TickRate, runtime.DealsDamageOverTime, statuses);
        StartCoroutine(MoveHitbox(dir));
        Destroy(instantiatedHitbox.gameObject, HitboxDuration);
        yield return null;
    }

    private IEnumerator MoveHitbox(Vector3 dir)
    {
        while (instantiatedHitbox != null)
        {
            instantiatedHitbox.transform.Translate(dir * HitboxMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
