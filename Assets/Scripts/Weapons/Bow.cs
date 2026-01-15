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
        BowAnimator.SetFloat("AttackSpeedMultiplier", PlayerContext.PlayerController.Animator.GetFloat("AttackSpeedMultiplier"));
        BowAnimator.SetTrigger("Fire");
    }
    public override void ActivateHitbox(int comboNumber)
    {
        if (ItemData.ItemSO is WeaponSO weaponData)
        {
            int rolledDamage = Random.Range(ItemData.MinDamage, ItemData.MaxDamage + 1) + PlayerContext.PlayerController.PlayerStatsBlackboard.GetBonusDamage(weaponAttackType);
            SpawnProjectiles(1, rolledDamage);
        }
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
            Quaternion newRotation = PlayerContext.PlayerController.transform.rotation * spreadRotation;

            Projectile proj = Instantiate(projectilePrefab, arrowSpawnPos.position, newRotation);
            proj.Init(projectileSpeed, rolledDamage, PlayerContext.PlayerController, new(), 3, false);
        }
    }

}


