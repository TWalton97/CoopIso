using UnityEngine;
using System.Collections.Generic;
using System;

public class Hitbox : MonoBehaviour
{
    public int _damage;
    private LayerMask _targetLayer;
    public Action OnTargetDamaged;
    private Collider[] colls;
    public Entity _controller;
    public bool DestroyHitboxOnHit = true;

    public void Init(int damage, LayerMask targetLayer, Entity controller)
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

    void Start()
    {
        if (_controller == null)
        {
            _controller = GetComponentInParent<Entity>();
        }
    }

    private void OnEnable()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_targetLayer & (1 << other.gameObject.layer)) == 0) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(_damage, _controller);
            OnTargetDamaged?.Invoke();
            if (DestroyHitboxOnHit) Destroy(gameObject);
        }
    }

    public void ActivateHitbox(int damage)
    {
        _damage = damage;
        foreach (Collider coll in colls)
        {
            coll.enabled = true;
        }

        Invoke(nameof(DeactivateHitbox), 0.2f);
    }

    private void DeactivateHitbox()
    {
        foreach (Collider coll in colls)
        {
            coll.enabled = false;
        }
    }
}
