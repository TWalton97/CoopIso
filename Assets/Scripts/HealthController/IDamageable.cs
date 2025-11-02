using System.Collections;

public interface IDamageable
{
    public void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck = false);
    public void Heal(int healAmount, bool canOverHeal = false);
    public void Die();
}
