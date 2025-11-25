using System;
using UnityEngine;

public class PlayerHealthController : HealthController
{
    public Action OnArmorAmountChanged;
    public int ArmorAmount;
    private NewPlayerController newPlayerController;

    private void Awake()
    {
        newPlayerController = GetComponent<NewPlayerController>();
    }

    public override void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck = false, bool isCritical = false)
    {
        if (!bypassBlockCheck)
        {
            if (newPlayerController.attackStateMachine.current.State == newPlayerController.blockState)
            {
                SpawnedItemDataBase.SpawnedShieldData spawnedItemData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(newPlayerController.WeaponController.instantiatedSecondaryWeapon.itemID) as SpawnedItemDataBase.SpawnedShieldData;
                if (CheckAngleToAttacker(controller.gameObject, spawnedItemData.blockAngle))
                {
                    Block(damageAmount, controller, spawnedItemData);
                    return;
                }
            }
        }

        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - ApplyArmorReduction(damageAmount), 0, MaximumHealth);
        OnTakeDamage?.Invoke(damageAmount, controller);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private bool CheckAngleToAttacker(GameObject attacker, float blockAngle)
    {
        var directionToPlayer = attacker.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        if (!(angleToPlayer < blockAngle / 2f))
        {
            return false;
        }
        return true;
    }


    //Calculates the new damage amount and returns it
    private void Block(int damageAmount, Entity controller, SpawnedItemDataBase.SpawnedShieldData shieldData)
    {
        int newDamageAmount = Mathf.Clamp(damageAmount - shieldData.blockAmount, 0, damageAmount);
        DamageNumberManager.Instance.SpawnText($"Blocked", transform.position + Vector3.up);
        TakeDamage(newDamageAmount, controller, true);
    }

    public void UpdateArmorAmount(int amount)
    {
        ArmorAmount += amount;
        OnArmorAmountChanged?.Invoke();
    }

    private int ApplyArmorReduction(int baseDamage, int K = 12)
    {
        if (baseDamage <= 0) return 0;
        if (ArmorAmount <= 0) return baseDamage;
        int damage = Mathf.Clamp(baseDamage * K / (K + ArmorAmount), 1, baseDamage);
        return damage;
    }
}
