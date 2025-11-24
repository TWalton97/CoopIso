using System;
using UnityEngine;

public abstract class DamageOverTimeSO : StatusSO
{
    public GameObject debuffVFX;
    private GameObject vfxRoot;
    private ParticleSystem[] spawnedVFX;
    public float tickRate;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);
        if (debuffVFX != null)
        {
            vfxRoot = Instantiate(debuffVFX, target.transform.position, Quaternion.identity);
            spawnedVFX = vfxRoot.GetComponentsInChildren<ParticleSystem>(true);
            instance.spawnedVFX = vfxRoot;
        }
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {
        if (instance.tickTimer >= tickRate)
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
    }

    protected abstract int CalculateDamage(StatusInstance Instance, StatusController target);
}
