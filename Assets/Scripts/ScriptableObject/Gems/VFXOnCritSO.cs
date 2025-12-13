using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Crit VFX Effect")]
public class VFXOnCritSO : GemEffectSO
{
    public GameObject HitVFX;
    private GemContext _context;

    public override void Apply(GemContext context)
    {
        _context = context;
        context.playerContext.PlayerController.OnCritTarget += SpawnVFX;
    }

    private void SpawnVFX(Entity entity)
    {
        GameObject vfx = GameObject.Instantiate(HitVFX, entity.transform.position + Vector3.up, Quaternion.identity);
        Destroy(vfx, 2f);
    }

    public override void Deregister()
    {
        _context.playerContext.PlayerController.OnCritTarget -= SpawnVFX;
    }

    public override void ApplyOnce(GemContext context, Entity target)
    {

    }
}


