using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class Projectile : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;
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

    public void Init(float speed, Vector3 direction, float maximumProjectileDuration = 3f, bool doesProjectilePierceEnemies = false)
    {
        _speed = speed;
        _direction = direction;
        _maximumProjectileDuration = maximumProjectileDuration;

        if (!_doesProjectilePierceEnemies)
            _hitbox.OnTargetDamaged += DestroyProjectile;
    }

    private void Update()
    {
        MoveProjectile();
    }

    private void MoveProjectile()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
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
