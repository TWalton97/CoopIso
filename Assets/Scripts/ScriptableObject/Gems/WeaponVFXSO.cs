using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Weapon VFX")]
public class WeaponVFXSO : GemEffectSO
{
    public ParticleSystem WeaponVFX;
    private GemContext _context;

    public override void Apply(GemContext context)
    {
        _context = context;

        if (context.playerContext.PlayerController.WeaponController.instantiatedPrimaryWeapon.ItemData == context.itemData)
        {
            ApplyVFXToWeapon(context.playerContext.PlayerController.WeaponController.instantiatedPrimaryWeapon.gameObject);
        }
        else if (context.playerContext.PlayerController.WeaponController.instantiatedSecondaryWeapon.ItemData == context.itemData)
        {
            ApplyVFXToWeapon(context.playerContext.PlayerController.WeaponController.instantiatedSecondaryWeapon.gameObject);
        }
    }

    private void ApplyVFXToWeapon(GameObject weapon)
    {
        Mesh mesh = null;
        Quaternion rotation = Quaternion.identity;
        if (weapon.TryGetComponent(out MeshFilter meshFilter))
        {
            mesh = meshFilter.sharedMesh;
            rotation = meshFilter.transform.rotation;
        }
        if (weapon.GetComponentInChildren<SkinnedMeshRenderer>() != null)
        {
            SkinnedMeshRenderer smr = weapon.GetComponentInChildren<SkinnedMeshRenderer>();
            mesh = new Mesh();
            smr.BakeMesh(mesh);
            rotation = smr.transform.rotation;
        }

        ParticleSystem ps = GameObject.Instantiate(WeaponVFX, weapon.transform.position, rotation, weapon.transform);
        var shape = ps.shape;
        shape.mesh = mesh;
    }


    public override void Deregister()
    {

    }

    public override void ApplyOnce(GemContext context, Entity target)
    {

    }
}