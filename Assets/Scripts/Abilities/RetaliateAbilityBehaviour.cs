using System.Collections.Generic;
using UnityEngine;

public class RetaliateAbilityBehaviour : SpellAbilityBehaviour
{
    public ParticleSystem RetaliateVFX;
    public DamageOverTimeHitbox RetaliateHitbox;
    public LayerMask TargetLayer;

    public override void Initialize(NewPlayerController player, RuntimeAbility runtime, List<StatusSO> statuses = null)
    {
        base.Initialize(player, runtime, statuses);
        player.HealthController.OnBlock += Activate;
    }

    public override void OnUse()
    {

    }
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {

    }

    public override void OnChannelTick(float deltaTime)
    {

    }

    private void OnDestroy()
    {
        player.HealthController.OnBlock -= Activate;
    }

    public void Activate()
    {
        if (Time.time - player.HealthController.BlockTime > 0.5f)
            return;

        RetaliateVFX.Play();
        int damage = player.WeaponController.instantiatedSecondaryWeapon.ItemData.ShieldArmorAmount * (runtime.Damage / 100);
        RetaliateHitbox.Init(damage, TargetLayer, player);
        RetaliateHitbox.ActivateHitbox(damage);
    }
}
