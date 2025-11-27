using System;
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class Entity : MonoBehaviour, IDamageable
{

    public EntityStatsSO EntityData;
    public EntityStatus EntityStatus;
    public HealthController HealthController;

    public bool IsBlocking;

    public virtual void Awake()
    {
        HealthController = GetComponent<HealthController>();
    }

    public virtual void Die()
    {
        HealthController.Die();
    }

    public virtual void Heal(int healAmount, bool canOverHeal = false)
    {
        HealthController.Heal(healAmount);
    }

    public virtual void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck, bool isCritical = false)
    {
        HealthController.TakeDamage(damageAmount, controller, bypassBlockCheck, isCritical);
    }

    public virtual void ApplyStats()
    {
        if (EntityData == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing EntityData");
        }
        HealthController.IncreaseMaximumHealth(EntityData.MaximumHealth);
    }
}

[System.Serializable]
public class EntityStatus
{
    public string GUID;
    public Vector3 WorldPosition;
    public bool IsDead;

    public EntityStatus(string _guid, Vector3 _worldPosition, bool _isDead)
    {
        GUID = _guid;
        WorldPosition = _worldPosition;
        IsDead = _isDead;
    }
}
