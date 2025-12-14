using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/On Take Damage Chance Effect")]
public class OnTakeDamageChanceEffectSO : GemEffectSO
{
    private GemContext _context;
    [Range(0, 1f)] public float chance = 0.2f;
    public List<GemEffectSO> childEffects;

    public override void Apply(GemContext context)
    {
        _context = context;
        context.playerContext.PlayerController.HealthController.OnTakeDamage += OnHit;
    }

    public override void Deregister()
    {
        _context.playerContext.PlayerController.HealthController.OnTakeDamage -= OnHit;
    }

    private void OnHit(int damage, Entity entity)
    {
        if (entity.IsDead) return;

        if (Random.value <= chance)
        {
            foreach (var effect in childEffects)
            {
                effect.ApplyOnce(_context, entity);
            }
        }
    }

    public override void ApplyOnce(GemContext context, Entity target)
    {

    }
}


