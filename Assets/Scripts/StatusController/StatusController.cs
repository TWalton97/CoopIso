using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    public List<StatusInstance> activeStatuses = new();

    void Update()
    {
        float dt = Time.deltaTime;

        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            var instance = activeStatuses[i];

            instance.remainingDuration -= dt;
            instance.tickTimer += dt;

            instance.data.OnTick(instance, this, dt);

            if (instance.IsExpired)
            {
                instance.data.OnExit(instance, this);
                activeStatuses.RemoveAt(i);
            }
        }
    }

    public void ApplyStatus(StatusSO statusData, Entity source)
    {
        var existing = activeStatuses.Find(s => s.data.statusID == statusData.statusID);

        if (existing != null)
        {
            if (statusData.isStackable)
            {
                existing.stacks++;
            }

            if (statusData.refreshDurationOnReapply)
            {
                existing.remainingDuration = statusData.baseDuration;
                if (statusData is DamageOverTimeSO dotSO)
                {
                    existing.nextTickTime = statusData.baseDuration - dotSO.tickRate;
                }
            }

            existing.tickTimer = 0f;

            return;
        }

        StatusInstance newInstance = new StatusInstance(statusData, source);
        activeStatuses.Add(newInstance);

        statusData.OnEnter(newInstance, this);
    }

    public void ApplyStatus(StatusSO statusData, Entity source, int initialHitDamage)
    {
        var existing = activeStatuses.Find(s => s.data.statusID == statusData.statusID);

        if (existing != null)
        {
            if (statusData.isStackable)
            {
                existing.stacks++;
            }

            if (statusData.refreshDurationOnReapply)
            {
                existing.remainingDuration = statusData.baseDuration;
                if (statusData is DamageOverTimeSO dotSO)
                {
                    existing.nextTickTime = statusData.baseDuration - dotSO.tickRate;
                    if (initialHitDamage > existing.initialHitDamage)
                    {
                        existing.initialHitDamage = initialHitDamage;
                    }
                }
            }

            existing.tickTimer = 0f;

            return;
        }

        StatusInstance newInstance = new StatusInstance(statusData, source);
        newInstance.initialHitDamage = initialHitDamage;
        activeStatuses.Add(newInstance);

        statusData.OnEnter(newInstance, this);
    }

    public void RemoveStatus(string statusID)
    {
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            if (activeStatuses[i].data.statusID == statusID)
            {
                activeStatuses[i].data.OnExit(activeStatuses[i], this);
                activeStatuses.RemoveAt(i);
            }
        }
    }

    public void RemoveAllStatuses()
    {
        for (int i = activeStatuses.Count - 1; i >= 0; i--)
        {
            activeStatuses[i].data.OnExit(activeStatuses[i], this);
            activeStatuses.RemoveAt(i);
        }
    }
}
