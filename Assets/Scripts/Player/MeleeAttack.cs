using UnityEngine;
[CreateAssetMenu(fileName = "Attacks", menuName = "Attacks/PlayerAttacks/MeleeAttack")]
public class MeleeAttack : AttackSO
{
    public Hitbox hitbox;
    public override void Attack(Transform transform, Vector3 attackDir)
    {
        Hitbox _hitbox = Instantiate(hitbox, transform.position + attackDir, transform.rotation);
        _hitbox.ActivateHitbox(0.2f);
        Destroy(_hitbox.gameObject, 0.2f);
    }
}
