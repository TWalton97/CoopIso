using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Chance Effect")]
public class ChanceEffectSO : GemEffectSO
{
    private GemContext _context;
    [Range(0, 1f)] public float chance = 0.2f;
    public List<GemEffectSO> childEffects;

    public override void Apply(GemContext context)
    {
        _context = context;
        context.playerContext.PlayerController.OnHitTarget += OnHit;
    }

    public override void Deregister()
    {
        _context.playerContext.PlayerController.OnHitTarget -= OnHit;
    }

    private void OnHit(HitData hitData)
    {
        if (hitData.target.IsDead) return;

        if (Random.value <= chance)
        {
            foreach (var effect in childEffects)
            {
                effect.ApplyOnce(_context, hitData.target);
            }
        }
    }

    public override void ApplyOnce(GemContext context, Entity target)
    {

    }
}


