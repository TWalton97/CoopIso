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
        if (spawnedWeaponData.GetType() == typeof(SpawnedItemDataBase.SpawnedBowData))
        {
            SpawnedItemDataBase.SpawnedBowData weaponData = spawnedWeaponData as SpawnedItemDataBase.SpawnedBowData;
            int rolledDamage = Random.Range(weaponData.weaponMinDamage, weaponData.weaponMaxDamage);
            SpawnProjectiles(weaponData.numberOfProjectiles, rolledDamage);
        }
        //int rolledDamage = Random.Range(spawnedWeaponData.weaponMinDamage, spawnedWeaponData.weaponMaxDamage);
        // Projectile proj = Instantiate(projectilePrefab, arrowSpawnPos.position, newPlayerController.transform.rotation);
        // proj.Init(projectileSpeed, rolledDamage, newPlayerController, 3, false);
    }


    //When we spawn projectiles, we need to rotate them based on how many we're spawning
    public void SpawnProjectiles(int numberOfProjectiles, int rolledDamage)
    {
        float totalAngle = Mathf.Clamp((numberOfProjectiles - 1) * 30f, 0f, 180f);
        float startAngle = -totalAngle / 2;

        float angleIncrement = 0f;
        if (numberOfProjectiles > 1)
        {
            angleIncrement = totalAngle / (numberOfProjectiles - 1);
        }

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float currentAngle = startAngle + i * angleIncrement;
            Quaternion spreadRotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Quaternion newRotation = newPlayerController.transform.rotation * spreadRotation;

            Projectile proj = Instantiate(projectilePrefab, arrowSpawnPos.position, newRotation);
            proj.Init(projectileSpeed, rolledDamage, newPlayerController, 3, false);
        }
    }

}


