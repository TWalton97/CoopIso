using UnityEngine;
using System;

public class PlayerStatsBlackboard : MonoBehaviour
{
    [Header("Health")]
    public HealthController HealthController;

    public float MaximumHealth;
    public float CurrentHealth;

    public Action OnHealthChanged;

    [Header("Movement")]
    public PlayerController PlayerController;
    public float MovementSpeed;
    public float CurrentSpeed;

    [Header("Attack")]
    public WeaponController WeaponController;
    public float AttackCooldown;

    [Header("Resources")]
    public ResourceController ResourceController;
    public Resources.ResourceType ResourceType;
    public float ResourceMax;
    public float ResourceCurrent;
    public float ResourceMin;

    private void Start()
    {
        UpdateHealthStats(0, null);
        UpdateMovementSpeed();
        UpdateAttackStats();
        UpdateResourceStats();
    }

    void Update()
    {
        UpdateMovementSpeed();
    }

    private void OnEnable()
    {
        HealthController.OnTakeDamage += UpdateHealthStats;
        ResourceController.resource.OnResourceValueChanged += UpdateResourceStats;
    }

    private void OnDisable()
    {
        HealthController.OnTakeDamage -= UpdateHealthStats;
        ResourceController.resource.OnResourceValueChanged -= UpdateResourceStats;
    }

    private void UpdateHealthStats(int amount, BaseUnitController controller)
    {
        MaximumHealth = HealthController.MaximumHealth;
        CurrentHealth = HealthController.CurrentHealth;
    }

    private void UpdateMovementSpeed()
    {
        MovementSpeed = PlayerController._movementSpeed;
        CurrentSpeed = Mathf.Floor(PlayerController.Rigidbody.velocity.magnitude);
    }

    private void UpdateAttackStats()
    {
        AttackCooldown = WeaponController.attack.attackCooldown;
    }

    private void UpdateResourceStats()
    {
        ResourceType = ResourceController.resource.resourceType;
        ResourceMax = ResourceController.resource.resourceMax;
        ResourceCurrent = ResourceController.resource.resourceCurrent;
        ResourceMin = ResourceController.resource.resourceMin;
    }
}
