using System.Collections.Generic;
using UnityEngine;

public class BuffAbilityBehaviour : AbilityBehaviour<BuffRuntimeAbility>
{
    public WeaponRangeType weaponRangeType;
    public LayerMask TargetLayer;
    public float radius;
    public StatusSO Status;

    private List<StatusController> statusControllers = new();

    public int NumberOfTargets = -1;
    public BuffApplicationTime buffApplicationTime = BuffApplicationTime.OnExit;

    public override void OnEnter()
    {
        if (Status is ArmorBuffSO armorBuffSO)
        {
            armorBuffSO.baseDuration = runtime.Duration;
            armorBuffSO.armorIncrease = (int)runtime.BuffAmount;
        }

        if (Status is BurnDotSO burnDotSO)
        {
            burnDotSO.damagePerTick = (int)runtime.BuffAmount;
            NumberOfTargets = (int)runtime.Duration;
        }

        if (Status is MovementSpeedBuffSO movementSpeedBuffSO)
        {
            movementSpeedBuffSO.speedIncreasePercentage = (int)runtime.BuffAmount;
            movementSpeedBuffSO.baseDuration = runtime.Duration;
        }

        if (buffApplicationTime == BuffApplicationTime.OnEnter)
        {
            ApplyBuff();
        }
    }

    public override void OnExit()
    {
        if (buffApplicationTime == BuffApplicationTime.OnExit)
        {
            ApplyBuff();
        }
    }

    public override void OnUse()
    {
    }

    public override bool CanUse(ResourceController resourceController)
    {
        if (weaponRangeType == WeaponRangeType.Melee || weaponRangeType == WeaponRangeType.Ranged)
        {
            if (resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon == null || resourceController.newPlayerController.WeaponController.instantiatedPrimaryWeapon.weaponRangeType != weaponRangeType)
            {
                return false;
            }
        }
        else if (weaponRangeType == WeaponRangeType.Shield)
        {
            if (!resourceController.newPlayerController.WeaponController.HasShieldEquipped)
            {
                return false;
            }
        }

        return resourceController.resource.resourceType == runtime.resourceType && resourceController.resource.RemoveResource(runtime.resourceAmount);
    }

    public void ApplyBuff()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, TargetLayer);

        int numTargets = colliders.Length;

        if (NumberOfTargets != -1)
        {
            numTargets = Mathf.Clamp(NumberOfTargets, 0, colliders.Length);
        }

        for (int i = 0; i < numTargets; i++)
        {
            if (colliders[i].TryGetComponent(out StatusController statusController))
            {
                if (!statusControllers.Contains(statusController))
                {
                    statusControllers.Add(statusController);
                }
            }
        }

        foreach (StatusController s in statusControllers)
        {
            s.ApplyStatus(Status, player);
        }

        statusControllers.Clear();
    }
}

public enum BuffApplicationTime
{
    OnEnter,
    OnExit
}
