using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeHitbox : Hitbox
{
    public float DamageTickDuration = 0.2f;    //We empty the damagedColliders list every tick
    public List<IDamageable> damagedColliders = new();

    public bool DamageOnTriggerStay;

    public void Init(int damage, LayerMask targetLayer, Entity controller, bool destroyHitboxOnHit = false, float DamageTickDuration = 0.2f, bool DamageOnTriggerStay = false)
    {
        _damage = damage;
        _targetLayer = targetLayer;
        _controller = controller;
        DestroyHitboxOnHit = destroyHitboxOnHit;
        this.DamageTickDuration = DamageTickDuration;
        this.DamageOnTriggerStay = DamageOnTriggerStay;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (DamageOnTriggerStay) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (damagedColliders.Contains(damageable)) return;
            damagedColliders.Add(damageable);
            damageable.TakeDamage(_damage, _controller);
            damagedColliders.Add(damageable);
            OnTargetDamaged?.Invoke();
        }

        if (DestroyHitboxOnHit) Destroy(gameObject);
    }

    public override void Start()
    {
        base.Start();

        if (DamageOnTriggerStay)
            StartCoroutine(EmptyDamageablesList());
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (!DamageOnTriggerStay) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (damagedColliders.Contains(damageable)) return;
            damagedColliders.Add(damageable);
            damageable.TakeDamage(_damage, _controller);
            damagedColliders.Add(damageable);
            OnTargetDamaged?.Invoke();
        }

        if (DestroyHitboxOnHit) Destroy(gameObject);
    }

    public override void ActivateHitbox(int damage)
    {
        _damage = damage;
        foreach (Collider coll in colls)
        {
            coll.enabled = true;
        }

        Invoke(nameof(DeactivateHitbox), 0.05f);
    }

    public override void DeactivateHitbox()
    {
        foreach (Collider coll in colls)
        {
            coll.enabled = false;
        }
    }

    private IEnumerator EmptyDamageablesList()
    {
        yield return new WaitForSeconds(DamageTickDuration);
        if (damagedColliders.Count != 0)
            damagedColliders.Clear();
        StartCoroutine(EmptyDamageablesList());
        yield return null;
    }
}
