using System.Collections;

public interface IDamageable
{
    public void TakeDamage(int damageAmount, BaseUnitController controller);
    public void Heal(int healAmount, bool canOverHeal = false);
    public void Die();
}
