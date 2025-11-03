using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class Projectile : MonoBehaviour
{
    private float _speed;
    private float _maximumProjectileDuration = 3f;
    private bool _doesProjectilePierceEnemies = false;
    float _elapsedTime = 0f;
    private Hitbox _hitbox;

    private void Awake()
    {
        _hitbox = GetComponent<Hitbox>();
    }

    private void OnDisable()
    {
        if (!_doesProjectilePierceEnemies)
            _hitbox.OnTargetDamaged -= DestroyProjectile;
    }

    public void Init(float speed, int damage, Entity spawner, float maximumProjectileDuration = 3f, bool doesProjectilePierceEnemies = false)
    {
        _speed = speed;
        _maximumProjectileDuration = maximumProjectileDuration;
        _hitbox._damage = damage;
        _hitbox._controller = spawner;

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
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
