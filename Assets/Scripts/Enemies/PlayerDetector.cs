using UnityEngine;
using Utilities;

public class PlayerDetector : MonoBehaviour
{
    private HealthController healthController;

    [SerializeField] float detectionAngle = 60f; // Cone in front of enemy
    [SerializeField] float detectionRadius = 10f; // Large circle around enemy
    [SerializeField] float innerDetectionRadius = 5f; // Small circle around enemy
    [SerializeField] float detectionCooldown = 1f; // Time between detections
    [SerializeField] float attackRange = 2f; // Distance from enemy to player to attack
    [SerializeField] float leashRange;  //Distance the target needs be to stop chasing
    [SerializeField] float aggroRange;
    [SerializeField] private LayerMask PlayerLayer;

    public Transform Player;

    CountdownTimer detectionTimer;
    CountdownTimer aggroTimer;

    IDetectionStrategy detectionStrategy;

    public bool DisplayGizmos = false;

    private bool tookDamageFromTarget = false;

    void Awake()
    {
        healthController = GetComponent<HealthController>();
    }

    void Start()
    {
        detectionTimer = new CountdownTimer(detectionCooldown);
        aggroTimer = new CountdownTimer(detectionCooldown);
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius, innerDetectionRadius);
    }

    void OnEnable()
    {
        healthController.OnTakeDamage += SetTargetToDamager;
    }

    void OnDisable()
    {
        healthController.OnTakeDamage -= SetTargetToDamager;
    }

    private void SetTargetToDamager(int damage, Entity damager)
    {
        if (Player != null) return;

        Player = damager.transform;
        tookDamageFromTarget = true;
    }

    private void FindPlayer()
    {
        if (Player == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, aggroRange, PlayerLayer);
            if (colliders.Length == 0)
            {
                Player = null;
                return;
            }

            foreach (Collider coll in colliders)
            {
                if (coll.GetComponent<NewPlayerController>() != null)
                {
                    Player = coll.transform;
                    return;
                }
            }
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) > leashRange)
        {
            Player = null;
        }
    }

    void Update()
    {
        // if (Player == null)
        // {
        //     FindPlayer();
        // }
        TickPlayerDetection();
        aggroTimer.Tick(Time.deltaTime);
        detectionTimer.Tick(Time.deltaTime);
    }

    private void TickPlayerDetection()
    {
        if (aggroTimer.IsRunning) return;

        FindPlayer();

        aggroTimer.Start();
    }

    public bool CanDetectPlayer()
    {
        if (Player == null) return false;
        if (tookDamageFromTarget) return true;
        return detectionTimer.IsRunning || detectionStrategy.Execute(Player, transform, detectionTimer);
    }

    public bool CanAttackPlayer()
    {
        if (Player == null) return false;
        var directionToPlayer = Player.position - transform.position;
        return directionToPlayer.magnitude <= attackRange;
    }

    public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) => this.detectionStrategy = detectionStrategy;

    void OnDrawGizmos()
    {
        if (!DisplayGizmos) return;

        Gizmos.color = Color.red;

        // Draw a spheres for the radii
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawWireSphere(transform.position, innerDetectionRadius);

        // Calculate our cone directions
        Vector3 forwardConeDirection = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
        Vector3 backwardConeDirection = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;

        // Draw lines to represent the cone
        Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
        Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
    }
}
