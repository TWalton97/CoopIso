using System;
using UnityEngine;
using System.Collections.Generic;

public class SpawnedItemDataBase : Singleton<SpawnedItemDataBase>
{
    public Dictionary<string, SpawnedItemData> spawnedItemData = new Dictionary<string, SpawnedItemData>();

    public List<Item> spawnableItems;

    public Item SpawnRandomItem(int rarity)
    {
        Item itemBase = Instantiate(spawnableItems[UnityEngine.Random.Range(0, spawnableItems.Count - 1)]);
        int numAffixes = GetAffixCount(rarity);
        if (itemBase.itemData.data.GetType() == typeof(WeaponDataSO))
        {
            for (int i = 0; i < numAffixes; i++)
            {
                itemBase.itemData.affixes.Add(WeaponAffixFactory.ReturnRandomWeaponAffix());
            }
        }
        else if (itemBase.itemData.data.GetType() == typeof(ShieldSO))
        {
            for (int i = 0; i < numAffixes; i++)
            {
                itemBase.itemData.affixes.Add(WeaponAffixFactory.ReturnRandomShieldAffix());
            }
        }
        itemBase.itemData.vfxPrefab = AffixManager.Instance.ReturnVFX(numAffixes);
        itemBase.itemData.itemID = RegisterItemToDatabase(itemBase.itemData);

        return itemBase;
    }

    public int GetAffixCount(int rarityValue)
    {
        if (rarityValue == -1)
            return 0;

        rarityValue = Mathf.Clamp(rarityValue, 0, 100);

        float roll = UnityEngine.Random.Range(0f, 100f);


        if (roll < rarityValue * 0.5f)
            return 4;
        else if (roll < rarityValue * 0.7f)
            return 3;
        else if (roll < rarityValue * 0.9f)
            return 2;
        else
            return 1;
    }

    public string RegisterItemToDatabase(ItemData itemData)
    {
        string id = Guid.NewGuid().ToString();

        if (itemData.data.GetType() == typeof(WeaponDataSO))
        {
            WeaponDataSO weaponData = itemData.data as WeaponDataSO;
            List<WeaponAffix> weaponAffixes = AffixListConverter.ConvertListIntoWeaponAffixes(itemData.affixes);
            int weaponMinDamage = weaponData.WeaponMinDamage + AffixStatCalculator.CalculateMinDamage(weaponAffixes);
            int weaponMaxDamage = weaponData.WeaponMaxDamage + AffixStatCalculator.CalculateMaxDamage(weaponAffixes);
            float attacksPerSecond = weaponData.AttacksPerSecond * AffixStatCalculator.CalculateAttackSpeed(weaponAffixes);
            int numberOfAttacksInCombo = weaponData.NumberOfAttacksInCombo;
            float MovementSpeedDuringAttack = weaponData.MovementSpeedMultiplierDuringAttack;

            SpawnedWeaponsData data = new SpawnedWeaponsData(id, weaponMinDamage, weaponMaxDamage, attacksPerSecond, numberOfAttacksInCombo, MovementSpeedDuringAttack);
            spawnedItemData.Add(id, data);
        }
        else if (itemData.data.GetType() == typeof(ShieldSO))
        {
            ShieldSO shieldData = itemData.data as ShieldSO;
            List<ShieldAffix> shieldAffixes = AffixListConverter.ConvertListIntoShieldAffixes(itemData.affixes);
            int blockAngle = shieldData.BlockAngle + AffixStatCalculator.CalculateBlockAngle(shieldAffixes);
            int blockAmount = shieldData.BlockAmount + AffixStatCalculator.CalculateBlockAmount(shieldAffixes);

            SpawnedShieldData data = new SpawnedShieldData(id, blockAngle, blockAmount);
            spawnedItemData.Add(id, data);
        }

        return id;
    }

    public SpawnedItemData GetSpawnedItemDataFromDataBase(string id)
    {
        SpawnedItemData data = null;
        if (spawnedItemData.ContainsKey(id))
        {
            data = spawnedItemData[id];
        }
        return data;
    }

    public class SpawnedItemData
    {
        public string uniqueID;
    }
    public class SpawnedWeaponsData : SpawnedItemData
    {
        public int weaponMinDamage;
        public int weaponMaxDamage;
        public float attacksPerSecond;
        public int numberOfAttacksInCombo;
        public float novementSpeedDuringAttack;

        public SpawnedWeaponsData(string _uniqueID, int _weaponMinDamage, int _weaponMaxDamage, float _attacksPerSecond, int _numberofAttacksInCombo, float _movementSpeedDuringAttack)
        {
            uniqueID = _uniqueID;
            weaponMinDamage = _weaponMinDamage;
            weaponMaxDamage = _weaponMaxDamage;
            attacksPerSecond = _attacksPerSecond;
            novementSpeedDuringAttack = _movementSpeedDuringAttack;
        }
    }

    public class SpawnedShieldData : SpawnedItemData
    {
        public int blockAngle;
        public int blockAmount;

        public SpawnedShieldData(string _uniqueID, int _blockAngle, int _blockAmount)
        {
            uniqueID = _uniqueID;
            blockAngle = _blockAngle;
            blockAmount = _blockAmount;
        }
    }
}

[System.Serializable]
public enum ItemStatus
{
    OnGround,
    InInventory,
    Equipped
}