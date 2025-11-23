using System.Collections.Generic;
using UnityEngine;

public class BuffAbilityBehaviour : AbilityBehaviour<BuffRuntimeAbility>
{
    public LayerMask PlayerLayer;
    public float radius;
    public StatusSO Status;

    private List<StatusController> statusControllers = new();

    public override void OnEnter()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, PlayerLayer);
        foreach (Collider coll in colliders)
        {
            if (coll.TryGetComponent(out StatusController statusController))
            {
                if (!statusControllers.Contains(statusController))
                {
                    statusControllers.Add(statusController);
                }
            }
        }

        foreach (StatusController s in statusControllers)
        {
            s.ApplyStatus(Status);
        }

        statusControllers.Clear();
    }

    public override void OnExit()
    {

    }

    public override void OnUse()
    {
    }

    public override bool CanUse(ResourceController resourceController)
    {
        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }
}
