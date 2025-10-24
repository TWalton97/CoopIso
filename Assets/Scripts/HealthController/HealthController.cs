using UnityEngine;
using System;

public class HealthController : MonoBehaviour, IDamageable
{
    public int MaximumHealth = 10;
    public int CurrentHealth;

    public Action<int> OnTakeDamage;
    public Action<int> OnHeal;
    public Action OnDie;

    private void Awake()
    {
        CurrentHealth = MaximumHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, MaximumHealth);
        OnTakeDamage?.Invoke(damageAmount);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int HealAmount, bool canOverHeal = false)
    {
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
        Debug.Log(gameObject.name + " has died");
        OnDie?.Invoke();
    }

    public void Heal(int healAmount)
    {

    }
}
