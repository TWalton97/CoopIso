using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Movement Speed Buff")]

public class MovementSpeedBuffSO : StatusSO
{
    public GameObject VFX;
    public int slowPercentage;

    private float startChaseSpeed;
    public float startWanderSpeed;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);
        if (target.TryGetComponent(out Enemy enemy))
        {
            startWanderSpeed = enemy.wanderSpeed;
            startChaseSpeed = enemy.chaseSpeed;

            enemy.wanderSpeed *= (100 - slowPercentage) * 0.01f;
            enemy.chaseSpeed *= (100 - slowPercentage) * 0.01f;
        }
        if (VFX != null)
        {
            var vfx = Instantiate(VFX, target.transform.position, Quaternion.identity);
            instance.spawnedVFX = vfx;
        }
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {
        instance.spawnedVFX.transform.position = target.transform.position;
        base.OnTick(instance, target, deltaTime);
    }

    public override void OnExit(StatusInstance instance, StatusController target)
    {
        if (target.TryGetComponent(out Enemy enemy))
        {
            enemy.wanderSpeed = startWanderSpeed;
            enemy.chaseSpeed = startChaseSpeed;
        }

        if (instance.spawnedVFX != null)
        {
            Destroy(instance.spawnedVFX.gameObject);
        }
    }
}
