using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class Projectile : MonoBehaviour
{
    private float _speed;
    private float _maximumProjectileDuration = 3f;
    private bool _doesProjectilePierceEnemies = false;
    float _elapsedTime = 0f;
    private DamageOverTimeHitbox _hitbox;

    private void Awake()
    {
        _hitbox = GetComponent<DamageOverTimeHitbox>();
    }

    private void OnDisable()
    {
        if (!_doesProjectilePierceEnemies)
            _hitbox.OnTargetDamaged -= DestroyProjectile;
    }

    public void Init(float speed, int damage, Entity spawner, List<StatusSO> appliedStatus, float maximumProjectileDuration = 3f, bool doesProjectilePierceEnemies = false)
    {
        _speed = speed;
        _maximumProjectileDuration = maximumProjectileDuration;
        _hitbox._damage = damage;
        _hitbox._controller = spawner;

        foreach (StatusSO s in appliedStatus)
        {
            _hitbox.statusesToApply.Add(s);
        }

        if (!_doesProjectilePierceEnemies)
            _hitbox.OnTargetDamaged += DestroyProjectile;
    }

    private void Update()
    {
        MoveProjectile();
    }

    private void MoveProjectile()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _maximumProjectileDuration)
        {
            DestroyProjectile(0);
        }
    }

    private void DestroyProjectile(int damage)
    {
        Destroy(gameObject);
    }
}
