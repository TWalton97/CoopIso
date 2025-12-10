using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Movement Speed Buff")]

public class MovementSpeedBuffSO : StatusSO
{
    public GameObject VFX;
    public int speedIncreasePercentage;

    private float startChaseSpeed;
    public float startWanderSpeed;

    private float speedChange;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);

        if (target.TryGetComponent(out Enemy enemy))
        {
            startWanderSpeed = enemy.wanderSpeed;
            startChaseSpeed = enemy.chaseSpeed;

            enemy.wanderSpeed *= (100 + speedIncreasePercentage) * 0.01f;
            enemy.chaseSpeed *= (100 + speedIncreasePercentage) * 0.01f;

            enemy.AttackSpeedMultiplier += speedIncreasePercentage * 0.01f;

        }
        else if (target.TryGetComponent(out NewPlayerController player))
        {
            startChaseSpeed = player._maximumMovementSpeed;

            speedChange = player.EntityData.MovementSpeed * (speedIncreasePercentage * 0.01f);
            player._maximumMovementSpeed += speedChange;

            player.AttackSpeedMultiplier += speedIncreasePercentage * 0.01f;
            player.PlayerStatsBlackboard.UpdateAttackStats();
        }
        if (VFX != null)
        {
            var vfx = Instantiate(VFX, target.transform.position, Quaternion.identity);
            instance.spawnedVFX = vfx;
        }
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {
        instance.spawnedVFX.transform.position = target.transform.position;
        base.OnTick(instance, target, deltaTime);
    }

    public override void OnExit(StatusInstance instance, StatusController target)
    {
        if (target.TryGetComponent(out Enemy enemy))
        {
            enemy.wanderSpeed = startWanderSpeed;
            enemy.chaseSpeed = startChaseSpeed;

            enemy.AttackSpeedMultiplier -= speedIncreasePercentage * 0.01f;
        }
        else if (target.TryGetComponent(out NewPlayerController player))
        {
            player._maximumMovementSpeed -= speedChange;
            player.AttackSpeedMultiplier -= speedIncreasePercentage * 0.01f;
            player.PlayerStatsBlackboard.UpdateAttackStats();
            //player._maximumMovementSpeed = startChaseSpeed;
        }

        if (instance.spawnedVFX != null)
        {
            Destroy(instance.spawnedVFX.gameObject);
        }
    }
}
