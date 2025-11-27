using UnityEngine;
using System;
using System.Collections;

public class HealthController : MonoBehaviour, IDamageable
{
    public Entity entity;
    public int MaximumHealth = 10;
    public int CurrentHealth;
    public int ArmorAmount;
    public int BlockAngle;

    public Action<int, Entity> OnTakeDamage;
    public Action<int> OnHeal;
    public Action OnDie;
    public Action OnMaximumHealthChanged;
    public Action OnArmorAmountChanged;

    public bool PrintDamageTaken = false;
    public bool DisplayDamageNumbers = true;
    protected bool IsDead = false;

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    public void Init(int MaximumHealth)
    {
        this.MaximumHealth = MaximumHealth;
        CurrentHealth = MaximumHealth;
    }

    public virtual void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck = false, bool isCritical = false)
    {
        if (!bypassBlockCheck)
        {
            if (entity != null && entity.IsBlocking)
            {
                if (CheckAngleToAttacker(controller.gameObject, BlockAngle))
                {
                    DamageNumberManager.Instance.SpawnText("Blocked", transform.position + Vector3.up);
                    return;
                }
            }
        }

        if (IsDead) return;


        CurrentHealth = Mathf.Clamp(CurrentHealth - ApplyArmorReduction(damageAmount), 0, MaximumHealth);
        OnTakeDamage?.Invoke(damageAmount, controller);

        if (DisplayDamageNumbers)
            DamageNumberManager.Instance.SpawnNumber(damageAmount, transform.position + Vector3.up, isCritical);

        if (PrintDamageTaken)
            Debug.Log($"{gameObject.name} has taken {damageAmount} from {controller.name}");

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
        OnMaximumHealthChanged?.Invoke();
    }

    private bool CheckAngleToAttacker(GameObject attacker, float blockAngle)
    {
        var directionToPlayer = attacker.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        if (!(angleToPlayer < blockAngle / 2f))
        {
            return false;
        }
        return true;
    }

    private void Block(int damageAmount, Entity controller, SpawnedItemDataBase.SpawnedShieldData shieldData)
    {
        int newDamageAmount = Mathf.Clamp(damageAmount - shieldData.blockAmount, 0, damageAmount);
        DamageNumberManager.Instance.SpawnText($"Blocked", transform.position + Vector3.up);
        TakeDamage(newDamageAmount, controller, true);
    }

    public void UpdateArmorAmount(int amount)
    {
        ArmorAmount += amount;
        OnArmorAmountChanged?.Invoke();
    }

    private int ApplyArmorReduction(int baseDamage, int K = 12)
    {
        if (baseDamage <= 0) return 0;
        if (ArmorAmount <= 0) return baseDamage;
        int damage = Mathf.Clamp(baseDamage * K / (K + ArmorAmount), 1, baseDamage);
        return damage;
    }
}
