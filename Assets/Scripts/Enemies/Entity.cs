using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class Entity : MonoBehaviour, IDamageable
{
    protected HealthController healthController;

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

    public virtual void TakeDamage(int damageAmount, BaseUnitController controller)
    {
        healthController.TakeDamage(damageAmount, controller);
    }
}
