using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public AttackSO attack;
    public bool canAttack = true;

    public void CallAttack(Transform transform, Vector3 attackDir)
    {
        if (!canAttack) return;
        canAttack = false;
        attack.Attack(transform, attackDir);
        Invoke(nameof(ResetAttackCooldown), attack.attackCooldown);
    }

    private void ResetAttackCooldown() => canAttack = true;
}
