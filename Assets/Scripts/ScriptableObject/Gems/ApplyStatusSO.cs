using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Apply Status Effect")]
public class ApplyStatusSO : GemEffectSO
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
        if (target.TryGetComponent(out StatusController statusController))
        {
            statusController.ApplyStatus(Status, context.playerContext.PlayerController);
        }
    }
}


