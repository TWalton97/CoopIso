using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Hitbox : MonoBehaviour
{
    public int _damage;
    public LayerMask _targetLayer;
    public Action<int> OnTargetDamaged;
    public List<IDamageable> damagedColliders = new();
    protected Collider[] colls;
    public Entity _controller;
    public bool DestroyHitboxOnHit = true;

    public virtual void Init(int damage, LayerMask targetLayer, Entity controller, bool destroyHitboxOnHit = true)
    {
        _damage = damage;
        _targetLayer = targetLayer;
        _controller = controller;
        DestroyHitboxOnHit = destroyHitboxOnHit;
    }

    private void Awake()
    {
        colls = GetComponents<Collider>();
        _targetLayer = Physics.AllLayers;
    }

    public virtual void Start()
    {
        if (_controller == null)
        {
            _controller = GetComponentInParent<Entity>();
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsInLayerMask(other.gameObject)) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (damagedColliders.Contains(damageable)) return;

            if (_controller is NewPlayerController newPlayerController && newPlayerController.PlayerStatsBlackboard.IsCritical())
            {
                int dam = newPlayerController.PlayerStatsBlackboard.CalculateCritical(_damage);
                damageable.TakeDamage(dam, _controller, false, true);
                damagedColliders.Add(damageable);
                OnTargetDamaged?.Invoke(dam);
            }
            else
            {
                damageable.TakeDamage(_damage, _controller);
                damagedColliders.Add(damageable);
                OnTargetDamaged?.Invoke(_damage);
            }
        }

        if (DestroyHitboxOnHit) Destroy(gameObject);
    }

    public virtual void ActivateHitbox(int damage)
    {
        _damage = damage;
        foreach (Collider coll in colls)
        {
            coll.enabled = true;
        }

        Invoke(nameof(DeactivateHitbox), 0.15f);
    }

    public virtual void DeactivateHitbox()
    {
        foreach (Collider coll in colls)
        {
            coll.enabled = false;
        }

        damagedColliders.Clear();
    }

    public bool IsInLayerMask(GameObject obj)
    {
        return (_targetLayer.value & (1 << obj.layer)) != 0;
    }
}
