using UnityEngine;
using System;

public class HealthController : MonoBehaviour, IDamageable
{
    public int MaximumHealth = 10;
    public int CurrentHealth;

    public Action<int, BaseUnitController> OnTakeDamage;
    public Action<int> OnHeal;
    public Action OnDie;

    private bool IsDead = false;

    private void Awake()
    {
        CurrentHealth = MaximumHealth;
    }

    public void TakeDamage(int damageAmount, BaseUnitController controller)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, MaximumHealth);
        OnTakeDamage?.Invoke(damageAmount, controller);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int HealAmount, bool canOverHeal = false)
    {
        if (IsDead) return;

        if (canOverHeal)
        {
            CurrentHealth += HealAmount;
        }
        else
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + HealAmount, 0, MaximumHealth);
        }

        OnHeal?.Invoke(HealAmount);
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        Debug.Log(gameObject.name + " has died");
        OnDie?.Invoke();
    }
}
