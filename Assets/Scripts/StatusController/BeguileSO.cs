using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Beguile Debuff")]
public class BeguileSO : StatusSO
{
    public GameObject VFX;

    private LayerMask storedLayermask;
    public LayerMask NewTargetLayer;

    private int startingLayer;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);
        if (target.TryGetComponent(out Enemy enemy))
        {
            storedLayermask = enemy.targetLayer;
            enemy.targetLayer = NewTargetLayer;

            startingLayer = enemy.gameObject.layer;
            enemy.gameObject.layer = LayerMask.NameToLayer("FriendlyNPC");
            enemy.ClearDamageTable();
        }

        if (VFX != null)
        {
            var vfx = Instantiate(VFX, target.transform.position, Quaternion.identity, target.transform);
            instance.spawnedVFX = vfx;
        }
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {

    }

    public override void OnExit(StatusInstance instance, StatusController target)
    {
        if (target.TryGetComponent(out Enemy enemy))
        {
            enemy.targetLayer = storedLayermask;
            enemy.gameObject.layer = startingLayer;
            enemy.ClearDamageTable();
        }

        if (instance.spawnedVFX != null)
        {
            Destroy(instance.spawnedVFX.gameObject);
        }
    }
}
