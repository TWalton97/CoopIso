using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerStatsBlackboard : MonoBehaviour
{
    [Header("Health")]
    public float MaximumHealth;
    public float CurrentHealth;
    public float HealthRegen;

    public Action OnHealthChanged;

    [Header("Defenses")]
    public int ArmorAmount;

    [Header("Movement")]
    public NewPlayerController PlayerController;
    public float MovementSpeed;
    public float CurrentSpeed;

    [Header("Attack")]
    public NewWeaponController WeaponController;
    public float AttackSpeedMultiplier;
    public float AttacksPerSecond;
    public float CriticalChance = 10;
    public float CriticalDamage = 50;

    [Header("Resources")]
    public ResourceController ResourceController;
    public Resources.ResourceType ResourceType;
    public float ResourceMax;
    public float ResourceCurrent;
    public float ResourceMin;
    public float ResourceRegen;

    public float WeightCurrent;
    public float WeightMax = 150f;

    public int GoldAmount = 500;
    public Action OnGoldAmountChanged;

    [Header("Equipment Requirements")]
    public bool CanEquipShields = false;
    public ArmorType armorType;

    [Header("Attack Masteries")]
    public bool UnarmedMastery = false;
    public bool OneHandedMastery = false;
    public bool TwoHandedMastery = false;
    public bool DualWieldMastery = false;
    public bool BowMastery = false;

    public string ClassName;

    private void Start()
    {
        UpdateHealthStats(0, null);
        UpdateMovementSpeed();
        UpdateAttackStats();
        UpdateResourceStats();
        UpdateArmorStats();
        PlayerStatsSO playerStats = PlayerController.EntityData as PlayerStatsSO;
        AttackSpeedMultiplier = playerStats.BaseAttackSpeed;
    }

    private void OnEnable()
    {
        PlayerController.HealthController.OnTakeDamage += UpdateHealthStats;
        WeaponController.OnWeaponUpdated += UpdateAttackStats;
        ResourceController.resource.OnResourceValueChanged += UpdateResourceStats;
        PlayerController.HealthController.OnMaximumHealthChanged += UpdateHealthStats;
        PlayerController.HealthController.OnArmorAmountChanged += UpdateArmorStats;
    }

    private void OnDisable()
    {
        PlayerController.HealthController.OnTakeDamage -= UpdateHealthStats;
        WeaponController.OnWeaponUpdated -= UpdateAttackStats;
        ResourceController.resource.OnResourceValueChanged -= UpdateResourceStats;
        PlayerController.HealthController.OnMaximumHealthChanged -= UpdateHealthStats;
        PlayerController.HealthController.OnArmorAmountChanged -= UpdateArmorStats;
    }

    private void UpdateHealthStats(int amount = 0, Entity controller = null)
    {
        if (PlayerController.HealthController == null) return;
        MaximumHealth = PlayerController.HealthController.MaximumHealth;
        CurrentHealth = PlayerController.HealthController.CurrentHealth;
        HealthRegen = PlayerController.HealthController.HealthRegenPerSecond;
    }

    private void UpdateHealthStats()
    {
        if (PlayerController.HealthController == null) return;
        MaximumHealth = PlayerController.HealthController.MaximumHealth;
        CurrentHealth = PlayerController.HealthController.CurrentHealth;
    }

    public void UpdateArmorStats()
    {
        if (PlayerController.HealthController == null) return;
        ArmorAmount = PlayerController.HealthController.ArmorAmount;
    }

    public void UpdateMovementSpeed()
    {
        if (PlayerController == null) return;
        MovementSpeed = PlayerController._maximumMovementSpeed;
        CurrentSpeed = PlayerController._movementSpeed;
    }

    public void AddGold(int amount)
    {
        GoldAmount += amount;
        PlayerController.PlayerContext.UserInterfaceController.UpdateGoldAmount(GoldAmount);
        OnGoldAmountChanged?.Invoke();
    }

    public void AddCurrentWeight(float amount)
    {
        WeightCurrent += amount;
        PlayerController.PlayerContext.UserInterfaceController.UpdateWeightAmount(WeightCurrent, WeightMax);
    }

    public void UpdateAttackStats()
    {
        float attacksPerSecond = 0.00f;
        int numWeapons = 0;

        if (WeaponController.instantiatedPrimaryWeapon == null)
        {
            AttacksPerSecond = 1.00f;
            return;
        }

        if (WeaponController.instantiatedPrimaryWeapon != null && WeaponController.instantiatedPrimaryWeapon.ItemData.ItemSO is WeaponSO)
        {
            WeaponSO weaponData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(WeaponController.instantiatedPrimaryWeapon.itemID).itemData.ItemSO as WeaponSO;
            attacksPerSecond += weaponData.AttacksPerSecond;
            numWeapons++;
        }

        if (WeaponController.instantiatedSecondaryWeapon != null && WeaponController.instantiatedSecondaryWeapon.ItemData.ItemSO is WeaponSO)
        {
            WeaponSO weaponData = SpawnedItemDataBase.Instance.GetSpawnedItemDataFromDataBase(WeaponController.instantiatedSecondaryWeapon.itemID).itemData.ItemSO as WeaponSO;
            attacksPerSecond += weaponData.AttacksPerSecond;
            numWeapons++;
        }

        if (numWeapons != 0)
        {
            attacksPerSecond /= numWeapons;
        }

        AttacksPerSecond = attacksPerSecond * AttackSpeedMultiplier;
    }

    private void UpdateResourceStats()
    {
        if (ResourceController == null) return;
        ResourceType = ResourceController.resource.resourceType;
        ResourceMax = ResourceController.resource.resourceMax;
        ResourceCurrent = ResourceController.resource.resourceCurrent;
        ResourceMin = ResourceController.resource.resourceMin;
        ResourceRegen = ResourceController.resource.RegenPerSecond;
    }

    public bool IsCritical()
    {
        int rand = UnityEngine.Random.Range(0, 101);
        if (rand <= CriticalChance)
        {
            return true;
        }
        return false;
    }

    public int CalculateCritical(int damage)
    {
        float newDamage = damage;
        newDamage = newDamage * (1 + CriticalDamage / 100);
        return (int)newDamage;
    }
}
