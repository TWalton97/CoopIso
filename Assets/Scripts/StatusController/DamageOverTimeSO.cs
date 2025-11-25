using System;
using UnityEngine;

public abstract class DamageOverTimeSO : StatusSO
{
    public GameObject debuffVFX;
    private GameObject vfxRoot;
    private ParticleSystem[] spawnedVFX;
    public float tickRate;
    public bool SpreadOnExit = false;
    public LayerMask EnemyLayer;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);
        if (debuffVFX != null)
        {
            vfxRoot = Instantiate(debuffVFX, target.transform.position, Quaternion.identity);
            spawnedVFX = vfxRoot.GetComponentsInChildren<ParticleSystem>(true);
            instance.spawnedVFX = vfxRoot;
        }
        instance.nextTickTime = instance.data.baseDuration - tickRate;
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {
        if (instance.remainingDuration <= instance.nextTickTime)
        {
            instance.tickTimer = 0;

            int damage = CalculateDamage(instance, target);

            if (target.TryGetComponent(out HealthController healthController))
            {
                healthController.TakeDamage(damage, instance.source);
                if (spawnedVFX != null)
                {
                    foreach (ParticleSystem ps in spawnedVFX)
                    {
                        ps.Play();
                    }
                }
            }
            instance.nextTickTime -= tickRate;
        }

        if (instance.spawnedVFX != null)
            instance.spawnedVFX.transform.position = target.transform.position;
    }

    public override void OnExit(StatusInstance instance, StatusController target)
    {
        if (instance.spawnedVFX != null)
        {
            Destroy(instance.spawnedVFX.gameObject);
        }

        if (SpreadOnExit && instance.remainingDuration > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(target.transform.position, 8f, EnemyLayer);
            if (colliders.Length == 0) return;
            foreach (Collider coll in colliders)
            {
                if (coll.TryGetComponent(out StatusController controller))
                {
                    if (controller == target) return;
                    if (!controller.HasStatusApplied(this))
                    {
                        controller.ApplyStatus(this, instance.source);
                    }
                }
            }
        }
    }

    protected abstract int CalculateDamage(StatusInstance Instance, StatusController target);
}
