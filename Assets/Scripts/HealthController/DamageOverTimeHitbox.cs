using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeHitbox : Hitbox
{
    public float DamageTickDuration = 0.2f;
    public bool DamageOnTriggerStay;
    public List<StatusSO> statusesToApply;

    public AttackInstance attackInstance;


    public void Init(int damage, LayerMask targetLayer, Entity controller, bool destroyHitboxOnHit = false, float DamageTickDuration = 0.2f, bool DamageOnTriggerStay = false, List<StatusSO> status = null)
    {
        _damage = damage;
        _targetLayer = targetLayer;
        _controller = controller;
        DestroyHitboxOnHit = destroyHitboxOnHit;
        this.DamageTickDuration = DamageTickDuration;
        this.DamageOnTriggerStay = DamageOnTriggerStay;
        statusesToApply = status;

        attackInstance = new AttackInstance();
        attackInstance.Damage = damage;

        if (_controller is NewPlayerController newPlayerController)
        {
            attackInstance.IsCritical = newPlayerController.PlayerStatsBlackboard.IsCritical();
            if (attackInstance.IsCritical)
            {
                attackInstance.Damage = newPlayerController.PlayerStatsBlackboard.CalculateCritical(attackInstance.Damage);
            }
        }

        if (DamageOnTriggerStay)
            StartCoroutine(EmptyDamageablesList());
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (DamageOnTriggerStay) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (damagedColliders.Contains(damageable)) return;
            damagedColliders.Add(damageable);

            damageable.TakeDamage(attackInstance.Damage, _controller, false, attackInstance.IsCritical);
            OnTargetDamaged?.Invoke(attackInstance.Damage);

            HitData hitData = new HitData();
            hitData.target = other.GetComponent<Entity>();
            hitData.damageAmount = attackInstance.Damage;
            hitData.isCritical = attackInstance.IsCritical;
            _controller.OnHitTarget?.Invoke(hitData);

            if (other.gameObject.TryGetComponent(out StatusController statusController) && statusesToApply != null)
            {
                foreach (StatusSO status in statusesToApply)
                {
                    statusController.ApplyStatus(status, _controller, attackInstance.Damage);
                }
            }
            damagedColliders.Add(damageable);

        }

        if (DestroyHitboxOnHit) Destroy(gameObject);
    }

    public override void Start()
    {
        base.Start();
    }

    public virtual void OnTriggerStay(Collider other)
    {
        if (!DamageOnTriggerStay) return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (damagedColliders.Contains(damageable)) return;
            damagedColliders.Add(damageable);

            int damageToDeal = _damage;

            if (_controller is NewPlayerController newPlayerController && newPlayerController.PlayerStatsBlackboard.IsCritical())
            {
                damageToDeal = newPlayerController.PlayerStatsBlackboard.CalculateCritical(_damage);
                damageable.TakeDamage(damageToDeal, _controller, false, true);
                OnTargetDamaged?.Invoke(damageToDeal);

                HitData hitData = new HitData();
                hitData.target = other.GetComponent<Entity>();
                hitData.damageAmount = damageToDeal;
                hitData.isCritical = true;
                _controller.OnHitTarget?.Invoke(hitData);
            }
            else
            {
                damageable.TakeDamage(damageToDeal, _controller);
                OnTargetDamaged?.Invoke(damageToDeal);

                HitData hitData = new HitData();
                hitData.target = other.GetComponent<Entity>();
                hitData.damageAmount = damageToDeal;
                hitData.isCritical = false;
                _controller.OnHitTarget?.Invoke(hitData);
            }

            if (other.gameObject.TryGetComponent(out StatusController statusController) && statusesToApply != null)
            {
                foreach (StatusSO status in statusesToApply)
                {
                    statusController.ApplyStatus(status, _controller, damageToDeal);
                }
            }
            damagedColliders.Add(damageable);
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
        damagedColliders.Clear();
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

[Serializable]
public class AttackInstance
{
    public int Damage;
    public bool IsCritical;
}
