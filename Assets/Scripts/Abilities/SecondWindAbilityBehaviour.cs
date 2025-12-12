using System.Collections.Generic;
using UnityEngine;

public class SecondWindAbilityBehaviour : SpellAbilityBehaviour
{
    public ParticleSystem SecondWindVFX;
    private float nextActivateTime;

    public override void Initialize(NewPlayerController player, RuntimeAbility runtime, List<StatusSO> statuses = null)
    {
        base.Initialize(player, runtime, statuses);
        player.HealthController.OnTakeDamage += CheckHealthThreshold;
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
        player.HealthController.OnTakeDamage -= CheckHealthThreshold;
    }

    private void CheckHealthThreshold(int damage, Entity entity)
    {
        if (player.HealthController.CurrentHealth < (float)player.HealthController.MaximumHealth / 4)
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (Time.time < nextActivateTime)
            return;

        float amountToHeal = player.HealthController.MaximumHealth * ((float)runtime.Damage / 100);
        StartCoroutine(player.HealthController.RestoreHealthOverDuration((int)amountToHeal, 2));
        nextActivateTime = Time.time + 60f;
        SecondWindVFX.Play();
    }
}
