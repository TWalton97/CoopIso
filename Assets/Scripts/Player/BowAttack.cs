using UnityEngine;

[CreateAssetMenu(fileName = "Attacks", menuName = "Attacks/PlayerAttacks/BowAttack")]
public class BowAttack : AttackSO
{
    public Projectile projectile;
    public float projectileSpeed;
    public override void Attack(Transform transform, Vector3 attackDir)
    {
        Projectile _projectile = Instantiate(projectile, transform.position + attackDir, Quaternion.identity);
        _projectile.Init(projectileSpeed, attackDir);
    }
}
