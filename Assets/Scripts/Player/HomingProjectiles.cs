using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/HomingProjectilesAbility")]
public class HomingProjectiles : BaseAbility
{
    public int numProjectiles;
    public float durationBetweenProjectiles;
    public float projectileSpeed;
    public float projectileRotationSpeed;

    public HomingProjectile homingProjectile;
    private Transform _transform;
    private Vector3 _pos;

    public override void ActivateAbility(Vector3 pos, Transform transform)
    {
        _transform = transform;
        _pos = pos;
        SpawnProjectile();
        SpawnProjectile();
        SpawnProjectile();
        SpawnProjectile();
        SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        Vector3 spawnPos = _transform.position + Random.insideUnitSphere;
        Vector3 dirToPos = _transform.position - spawnPos;
        Quaternion spawnRot = Quaternion.LookRotation(dirToPos, Vector3.up);
        HomingProjectile proj = Instantiate(homingProjectile, spawnPos, spawnRot);
        proj.Init(projectileSpeed, projectileRotationSpeed, _pos);
    }
}
