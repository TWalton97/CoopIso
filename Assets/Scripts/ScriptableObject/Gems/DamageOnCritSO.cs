using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Crit Effect")]
public class DamageOnCritSO : GemEffectSO
{
    public int Damage;
    private GemContext _context;

    public override void Apply(GemContext context)
    {
        _context = context;
        context.playerContext.PlayerController.OnCritTarget += ApplyDamage;
    }

    private void ApplyDamage(Entity entity)
    {
        entity.TakeDamage(Damage, _context.playerContext.PlayerController, false);
    }

    public override void Deregister()
    {
        _context.playerContext.PlayerController.OnCritTarget -= ApplyDamage;
    }

    public override void ApplyOnce(GemContext context, Entity target)
    {

    }
}
