using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Apply Buff Effect")]
public class ApplyBuffSO : GemEffectSO
{
    private GemContext _context;
    public StatusSO Status;

    public override void Apply(GemContext context)
    {
        _context = context;
    }

    public override void Deregister()
    {
    }

    public override void ApplyOnce(GemContext context, Entity target)
    {
        context.playerContext.PlayerController.StatusController.ApplyStatus(Status, context.playerContext.PlayerController);
    }
}


