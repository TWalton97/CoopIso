using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private float _speed;
    private float _rotationSpeed;
    private Vector3 _targetPos;
    public GameObject target;
    private float _maximumProjectileDuration = 3f;
    float _elapsedTime = 0f;
    private Hitbox _hitbox;
    public LayerMask enemyLayer;

    private void Awake()
    {
        _hitbox = GetComponent<Hitbox>();
    }

    public void Init(float speed, float rotationSpeed, Vector3 targetPos, float maximumProjectileDuration = 3f)
    {
        _speed = speed;
        _rotationSpeed = rotationSpeed;
        _targetPos = new Vector3(targetPos.x, 1f, targetPos.z);
        _maximumProjectileDuration = maximumProjectileDuration;
    }

    private void Update()
    {
        MoveProjectile();
        RotateTowardsTarget();

        if (target == null)
        {
            LookForValidTarget();
        }
    }

    private void MoveProjectile()
    {
        if (transform.position.y > 2f)
        {
            transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
        }
        transform.Translate(transform.forward * _speed * Time.deltaTime);
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _maximumProjectileDuration)
        {
            DestroyProjectile();
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 targetDir;
        Vector3 newDir;

        if (target == null)
        {
            targetDir = (transform.position - _targetPos).normalized;
            newDir = Vector3.RotateTowards(transform.forward, targetDir, _rotationSpeed * Time.deltaTime, 0f);
        }
        else
        {
            targetDir = (transform.position - new Vector3(target.transform.position.x, 1f, target.transform.position.z)).normalized;
            newDir = Vector3.RotateTowards(transform.forward, targetDir, _rotationSpeed * Time.deltaTime, 0f);
        }


        transform.rotation = Quaternion.LookRotation(newDir);
    }

    private void LookForValidTarget()
    {
        Collider[] overlappingColliders = Physics.OverlapSphere(transform.position, 5f, enemyLayer);
        if (overlappingColliders.Length == 0) return;

        foreach (Collider coll in overlappingColliders)
        {
            if (coll.TryGetComponent(out HealthController controller))
            {
                target = controller.gameObject;
            }
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
