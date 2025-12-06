using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Armor Buff")]
public class ArmorBuffSO : StatusSO
{
    public GameObject armorBuffVFX;
    public int armorIncrease = 10;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);

        if (target.TryGetComponent(out Entity controller))
        {
            controller.HealthController.UpdateArmorAmount(armorIncrease);
            if (armorBuffVFX != null)
            {
                var vfx = Instantiate(armorBuffVFX, target.transform.position, Quaternion.identity);
                instance.spawnedVFX = vfx;
            }
        }
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {
        instance.spawnedVFX.transform.position = target.transform.position;
        base.OnTick(instance, target, deltaTime);
    }

    public override void OnExit(StatusInstance instance, StatusController target)
    {
        if (target.TryGetComponent(out Entity controller))
        {
            controller.HealthController.UpdateArmorAmount(-armorIncrease);

            if (instance.spawnedVFX != null)
            {
                Destroy(instance.spawnedVFX.gameObject);
            }
        }
    }
}
