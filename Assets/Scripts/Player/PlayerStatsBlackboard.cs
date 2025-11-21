using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerStatsBlackboard : MonoBehaviour
{
    [Header("Health")]
    public PlayerHealthController HealthController;

    public float MaximumHealth;
    public float CurrentHealth;

    public Action OnHealthChanged;

    [Header("Defenses")]
    public int ArmorAmount;

    [Header("Movement")]
    public NewPlayerController PlayerController;
    public float MovementSpeed;
    public float CurrentSpeed;

    [Header("Attack")]
    public NewWeaponController WeaponController;

    public float AttacksPerSecond;

    [Header("Resources")]
    public ResourceController ResourceController;
    public Resources.ResourceType ResourceType;
    public float ResourceMax;
    public float ResourceCurrent;
    public float ResourceMin;

    [Header("Equipment Requirements")]
    public bool CanEquipShields = false;
    public ArmorType armorType;

    [Header("Attack Masteries")]
    public bool UnarmedMastery = false;
    public bool OneHandedMastery = false;
    public bool TwoHandedMastery = false;
    public bool DualWieldMastery = false;
    public bool BowMastery = false;

    private void Start()
    {
        UpdateHealthStats(0, null);
        UpdateMovementSpeed();
        UpdateAttackStats();
        UpdateResourceStats();
    }

    private void OnEnable()
    {
        HealthController.OnTakeDamage += UpdateHealthStats;
        WeaponController.OnWeaponUpdated += UpdateAttackStats;
        ResourceController.resource.OnResourceValueChanged += UpdateResourceStats;
        HealthController.OnMaximumHealthChanged += UpdateHealthStats;
        HealthController.OnArmorAmountChanged += UpdateArmorStats;
    }

    private void OnDisable()
    {
        HealthController.OnTakeDamage -= UpdateHealthStats;
        WeaponController.OnWeaponUpdated -= UpdateAttackStats;
        ResourceController.resource.OnResourceValueChanged -= UpdateResourceStats;
        HealthController.OnMaximumHealthChanged -= UpdateHealthStats;
        HealthController.OnArmorAmountChanged -= UpdateArmorStats;
    }

    private void UpdateHealthStats(int amount = 0, Entity controller = null)
    {
        if (HealthController == null) return;
        MaximumHealth = HealthController.MaximumHealth;
        CurrentHealth = HealthController.CurrentHealth;
    }

    private void UpdateHealthStats()
    {
        if (HealthController == null) return;
        MaximumHealth = HealthController.MaximumHealth;
        CurrentHealth = HealthController.CurrentHealth;
    }

    private void UpdateArmorStats()
    {
        if (HealthController == null) return;
        ArmorAmount = HealthController.ArmorAmount;
    }

    private void UpdateMovementSpeed()
    {
        if (PlayerController == null) return;
        MovementSpeed = PlayerController._movementSpeed;
        CurrentSpeed = Mathf.Floor(PlayerController.Rigidbody.velocity.magnitude);
    }

    private void UpdateAttackStats()
    {
        float attacksPerSecond = 0.00f;
        int numWeapons = 0;

        if (WeaponController.instantiatedPrimaryWeapon == null)
        {
            AttacksPerSecond = 1.00f;
            return;
        }

        if (WeaponController.instantiatedPrimaryWeapon != null && WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(BowSO))
        {
            SpawnedItemDataBase.SpawnedBowData spawnedWeaponsData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(WeaponController.instantiatedPrimaryWeapon.itemID) as SpawnedItemDataBase.SpawnedBowData;
            attacksPerSecond += spawnedWeaponsData.attacksPerSecond;
            numWeapons++;
        }

        if (WeaponController.instantiatedPrimaryWeapon != null && WeaponController.instantiatedPrimaryWeapon.Data.GetType() == typeof(WeaponDataSO))
        {
            SpawnedItemDataBase.SpawnedWeaponsData spawnedWeaponsData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(WeaponController.instantiatedPrimaryWeapon.itemID) as SpawnedItemDataBase.SpawnedWeaponsData;
            attacksPerSecond += spawnedWeaponsData.attacksPerSecond;
            numWeapons++;
        }

        if (WeaponController.instantiatedSecondaryWeapon != null && WeaponController.instantiatedSecondaryWeapon.Data.GetType() == typeof(WeaponDataSO))
        {
            SpawnedItemDataBase.SpawnedWeaponsData spawnedWeaponsData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(WeaponController.instantiatedSecondaryWeapon.itemID) as SpawnedItemDataBase.SpawnedWeaponsData;
            attacksPerSecond += spawnedWeaponsData.attacksPerSecond;
            numWeapons++;
        }

        if (numWeapons != 0)
        {
            attacksPerSecond /= numWeapons;
        }

        AttacksPerSecond = attacksPerSecond;
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
