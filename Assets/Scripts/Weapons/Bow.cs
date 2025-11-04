using UnityEngine;

public class Bow : Weapon
{
    public Transform arrowSpawnPos;
    public Projectile projectilePrefab;
    public float projectileSpeed;
    public override void ActivateHitbox()
    {
        int rolledDamage = Random.Range(spawnedWeaponData.weaponMinDamage, spawnedWeaponData.weaponMaxDamage);
        Projectile proj = Instantiate(projectilePrefab, arrowSpawnPos.position, newPlayerController.transform.rotation);
        proj.Init(projectileSpeed, rolledDamage, newPlayerController, 3, false);
    }

    // private Quaternion GetRotationTowardsTarget()
    // {
    //     Vector3 DirToTarget = newPlayerController.transform.position + newPlayerController.transform.forward - arrowSpawnPos.position;
    //     Quaternion targetRotation = Quaternion.LookRotation(DirToTarget, Vector3.up);
    //     targetRotation.x = 0;
    //     targetRotation.z = 0;
    //     return targetRotation;
    // }
}


