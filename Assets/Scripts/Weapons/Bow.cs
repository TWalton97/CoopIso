using UnityEngine;

public class Bow : Weapon
{
    public Animator BowAnimator;
    public Transform arrowSpawnPos;
    public Projectile projectilePrefab;
    public float projectileSpeed;
    public override void Enter(System.Action endAction, int attackNum)
    {
        base.Enter(endAction, attackNum);
        BowAnimator.SetFloat("AttackSpeedMultiplier", newPlayerController.Animator.GetFloat("AttackSpeedMultiplier"));
        BowAnimator.SetTrigger("Fire");
    }
    public override void ActivateHitbox()
    {
        int rolledDamage = Random.Range(spawnedWeaponData.weaponMinDamage, spawnedWeaponData.weaponMaxDamage);
        Projectile proj = Instantiate(projectilePrefab, arrowSpawnPos.position, newPlayerController.transform.rotation);
        proj.Init(projectileSpeed, rolledDamage, newPlayerController, 3, false);
    }

}


