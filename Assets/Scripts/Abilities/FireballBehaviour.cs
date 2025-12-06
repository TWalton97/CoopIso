using UnityEngine;

public class FireballBehaviour : ProjectileSpellAbilityBehaviour
{
    public Projectile FireballProjectile;

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        SpawnProjectiles();
    }

    public override void OnUse()
    {

    }

    public void SpawnProjectiles()
    {
        float totalAngle = Mathf.Clamp((runtime.NumberOfProjectiles - 1) * 30f, 0f, 180f);
        float startAngle = -totalAngle / 2;

        float angleIncrement = 0f;
        if (runtime.NumberOfProjectiles > 1)
        {
            angleIncrement = totalAngle / (runtime.NumberOfProjectiles - 1);
        }

        for (int i = 0; i < runtime.NumberOfProjectiles; i++)
        {
            float currentAngle = startAngle + i * angleIncrement;
            Quaternion spreadRotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Quaternion newRotation = player.transform.rotation * spreadRotation;

            Projectile proj = Instantiate(FireballProjectile, player.transform.position + player.transform.forward + Vector3.up, newRotation);
            proj.Init(runtime.ProjectileSpeed, runtime.Damage, player, runtime.Statuses, 3, false);
        }
    }
}
