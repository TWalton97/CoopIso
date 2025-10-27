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
    public NewPlayerController PlayerController;
    public float MovementSpeed;
    public float CurrentSpeed;

    [Header("Attack")]
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

    private void UpdateHealthStats(int amount, Entity controller)
    {
        if (HealthController == null) return;
        MaximumHealth = HealthController.MaximumHealth;
        CurrentHealth = HealthController.CurrentHealth;
    }

    private void UpdateMovementSpeed()
    {
        if (PlayerController == null) return;
        MovementSpeed = PlayerController._movementSpeed;
        CurrentSpeed = Mathf.Floor(PlayerController.Rigidbody.velocity.magnitude);
    }


    private void UpdateResourceStats()
    {
        if (ResourceController == null) return;
        ResourceType = ResourceController.resource.resourceType;
        ResourceMax = ResourceController.resource.resourceMax;
        ResourceCurrent = ResourceController.resource.resourceCurrent;
        ResourceMin = ResourceController.resource.resourceMin;
    }
}
