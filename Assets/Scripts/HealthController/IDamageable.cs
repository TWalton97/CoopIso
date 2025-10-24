using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damageAmount);
    public void Heal(int healAmount);
    public void Die();
}
