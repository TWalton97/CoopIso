using UnityEngine;

public class LifeDrainBehaviour : SpellAbilityBehaviour
{
    public float TickRate;
    public DamageOverTimeHitbox LifeDrainHitbox;
    private DamageOverTimeHitbox instantiatedHitbox;

    public override void OnEnter()
    {
        instantiatedHitbox = Instantiate(LifeDrainHitbox, player.transform.position, player.transform.rotation, player.transform);
        instantiatedHitbox.Init(runtime.Damage, Physics.AllLayers, player, false, TickRate, true, statuses);
        instantiatedHitbox.OnTargetDamaged += Heal;
    }

    public override void OnExit()
    {
        if (instantiatedHitbox == null) return;
        Destroy(instantiatedHitbox.gameObject);
    }

    private void Heal(int amount)
    {
        player.Heal(amount / 2);
    }
}
