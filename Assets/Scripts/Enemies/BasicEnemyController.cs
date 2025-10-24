using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(HealthController))]
public class BasicEnemyController : MonoBehaviour
{
    private HealthController HealthController;
    [SerializeField] private GameObject target;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        HealthController = GetComponent<HealthController>();
    }

    private void Update()
    {
        agent.SetDestination(target.transform.position);
    }

    private void OnEnable()
    {
        HealthController.OnDie += Die;
    }

    private void OnDisable()
    {
        HealthController.OnDie -= Die;
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
