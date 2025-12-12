using System.Collections.Generic;
using UnityEngine;

public class RetaliateAbilityBehaviour : SpellAbilityBehaviour
{
    public ParticleSystem RetaliateVFX;
    public DamageOverTimeHitbox RetaliateHitbox;
    public LayerMask TargetLayer;
    private float nextActivateTime;

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
        if (Time.time < nextActivateTime)
            return;

        if (Time.time - player.HealthController.BlockTime > 1f)
            return;

        nextActivateTime = Time.time + 7f;
        RetaliateVFX.Play();
        RetaliateHitbox.Init(runtime.Damage, TargetLayer, player);
        RetaliateHitbox.ActivateHitbox(runtime.Damage);
    }
}
