using System.Collections;

public interface IDamageable
{
    public void TakeDamage(int damageAmount);
    public void Heal(int healAmount);
    public void Die();
}
