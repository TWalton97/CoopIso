using UnityEngine;

public class RubyWeaponEffect : IGemEffect
{
    private Entity target;
    private NewPlayerController _controller;
    private GameObject _hitVFX;
    private HitData _hitData;

    //This reflects 3 damage whenever the owner takes damage

    public void Initialize(ItemData item, NewPlayerController controller, GameObject hitVFX)
    {
        _controller = controller;
        _hitVFX = hitVFX;
        controller.OnHitTarget += CheckDamageTaken;
    }

    private void CheckDamageTaken(HitData hitData)
    {
        if (hitData.isCritical)
        {
            _hitData = hitData;
            target = hitData.target;
            Activate();
        }
    }

    public void Activate()
    {
        float damage = _hitData.damageAmount * 0.5f;
        target.TakeDamage((int)damage, _controller, false, false);
        if (_hitVFX != null)
        {
            GameObject vfx = Object.Instantiate(_hitVFX, target.transform.position + Vector3.up, Quaternion.identity);
            GameObject.Destroy(vfx, 2f);
        }
    }

    public void Disable()
    {
        _controller.OnHitTarget -= CheckDamageTaken;
    }
}
