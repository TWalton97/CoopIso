using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class Entity : MonoBehaviour, IDamageable
{
    public HealthController healthController { get; private set; }

    public virtual void Awake()
    {
        healthController = GetComponent<HealthController>();
    }
    public virtual void Die()
    {
        healthController.Die();
    }

    public virtual void Heal(int healAmount, bool canOverHeal = false)
    {
        healthController.Heal(healAmount);
    }

    public virtual void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck)
    {
        healthController.TakeDamage(damageAmount, controller);
    }
}
