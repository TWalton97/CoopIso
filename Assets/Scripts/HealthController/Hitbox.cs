using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    [SerializeField] private int _damage;
    private readonly List<IDamageable> damageables = new List<IDamageable>();
    private LayerMask _targetLayer;
    public Action OnTargetDamaged;
    private Collider[] colls;
    private BaseUnitController _controller;

    public void Init(int damage, LayerMask targetLayer, BaseUnitController controller)
    {
        _damage = damage;
        _targetLayer = targetLayer;
        _controller = controller;
    }

    private void Awake()
    {
        colls = GetComponents<Collider>();
        _targetLayer = Physics.AllLayers;
    }

    private void OnEnable()
    {
        if (damageables.Count == 0) return;

        damageables.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_targetLayer & (1 << other.gameObject.layer)) == 0) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (damageables.Contains(damageable)) return;

            damageables.Add(damageable);
            damageable.TakeDamage(_damage, _controller);
            OnTargetDamaged?.Invoke();
            Destroy(gameObject);
        }
    }

    public void ActivateHitbox(float duration)
    {
        foreach (Collider coll in colls)
        {
            coll.enabled = true;
        }

        Invoke(nameof(DeactivateHitbox), duration);
    }

    private void DeactivateHitbox()
    {
        foreach (Collider coll in colls)
        {
            coll.enabled = false;
        }

        damageables.Clear();
    }
}
