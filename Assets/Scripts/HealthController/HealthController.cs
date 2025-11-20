using UnityEngine;
using System;
using System.Collections;

public class HealthController : MonoBehaviour, IDamageable
{
    public int MaximumHealth = 10;
    public int CurrentHealth;

    public Action<int, Entity> OnTakeDamage;
    public Action<int> OnHeal;
    public Action OnDie;
    public Action<int, Entity> OnMaximumHealthChanged;

    public bool PrintDamageTaken = false;
    protected bool IsDead = false;

    private void Awake()
    {
        CurrentHealth = MaximumHealth;
    }

    public virtual void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck = false)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, MaximumHealth);
        OnTakeDamage?.Invoke(damageAmount, controller);

        if (PrintDamageTaken)
        {
            Debug.Log(gameObject.name + " took " + damageAmount + " damage from " + controller.gameObject.name);
        }

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

    public IEnumerator RestoreHealthOverDuration(int amountOfHealth, int duration, Action endAction)
    {
        int amountOfHealthPerInterval = amountOfHealth / duration;

        for (int i = 0; i < duration; i++)
        {
            Heal(amountOfHealthPerInterval);
            yield return new WaitForSeconds(1);
        }
        endAction?.Invoke();
        yield return null;
    }

    public void IncreaseMaximumHealth(int amountOfHealth)
    {
        MaximumHealth += amountOfHealth;
        CurrentHealth += amountOfHealth;
        OnMaximumHealthChanged?.Invoke(0, null);
    }
}
