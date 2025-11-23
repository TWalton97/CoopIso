using System;
using UnityEngine;
using System.Collections.Generic;

public class SpawnedItemDataBase : MonoBehaviour
{
    public Dictionary<string, SpawnedItemData> spawnedItemData = new Dictionary<string, SpawnedItemData>();

    public List<Item> spawnableItems;

    public Item SpawnRandomItem(int rarity, Item itemToSpawn = null)
    {
        Item itemBase;
        if (itemToSpawn == null)
        {
            itemBase = Instantiate(spawnableItems[UnityEngine.Random.Range(0, spawnableItems.Count - 1)]);
        }
        else
        {
            itemBase = Instantiate(itemToSpawn);
        }

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
        else if (itemBase.itemData.data.GetType() == typeof(BowSO))
        {
            for (int i = 0; i < numAffixes; i++)
            {
                itemBase.itemData.affixes.Add(WeaponAffixFactory.ReturnRandomBowAffix());
            }
        }
        else if (itemBase.itemData.data.GetType() == typeof(ArmorSO))
        {
            for (int i = 0; i < numAffixes; i++)
            {
                itemBase.itemData.affixes.Add(WeaponAffixFactory.ReturnRandomArmorAffix());
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


        if (roll < rarityValue * 0.05f)
            return 4;
        else if (roll < rarityValue * 0.2f)
            return 3;
        else if (roll < rarityValue * 0.4f)
            return 2;
        else if (roll < rarityValue * 0.9f)
            return 1;
        else
            return 0;
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

            SpawnedWeaponsData data = new SpawnedWeaponsData(id, weaponMinDamage, weaponMaxDamage, attacksPerSecond);
            spawnedItemData.Add(id, data);
        }
        else if (itemData.data.GetType() == typeof(ShieldSO))
        {
            ShieldSO shieldData = itemData.data as ShieldSO;
            List<ShieldAffix> shieldAffixes = AffixListConverter.ConvertListIntoShieldAffixes(itemData.affixes);
            int blockAngle = shieldData.BlockAngle + AffixStatCalculator.CalculateBlockAngle(shieldAffixes);
            int blockAmount = shieldData.BlockAmount + AffixStatCalculator.CalculateBlockAmount(shieldAffixes);
            int armorAmount = Mathf.CeilToInt(shieldData.ArmorAmount * AffixStatCalculator.CalculateArmor(shieldAffixes));

            SpawnedShieldData data = new SpawnedShieldData(id, blockAngle, blockAmount, armorAmount);
            spawnedItemData.Add(id, data);
        }
        else if (itemData.data.GetType() == typeof(BowSO))
        {
            BowSO weaponData = itemData.data as BowSO;
            List<BowAffix> weaponAffixes = AffixListConverter.ConvertListIntoBowAffixes(itemData.affixes);
            int weaponMinDamage = weaponData.WeaponMinDamage + AffixStatCalculator.CalculateMinDamage(weaponAffixes);
            int weaponMaxDamage = weaponData.WeaponMaxDamage + AffixStatCalculator.CalculateMaxDamage(weaponAffixes);
            float attacksPerSecond = weaponData.AttacksPerSecond * AffixStatCalculator.CalculateAttackSpeed(weaponAffixes);
            float MovementSpeedDuringAttack = weaponData.MovementSpeedMultiplierDuringAttack;
            int numberOfProjectiles = weaponData.NumberOfProjectiles + AffixStatCalculator.CalculateProjectileCount(weaponAffixes);

            SpawnedBowData data = new SpawnedBowData(id, weaponMinDamage, weaponMaxDamage, attacksPerSecond, MovementSpeedDuringAttack, numberOfProjectiles);
            spawnedItemData.Add(id, data);
        }
        else if (itemData.data.GetType() == typeof(ArmorSO))
        {
            ArmorSO armorData = itemData.data as ArmorSO;
            List<ArmorAffix> armorAffixes = AffixListConverter.ConvertListIntoArmorAffixes(itemData.affixes);
            int increasedArmor = Mathf.CeilToInt(armorData.ArmorAmount * AffixStatCalculator.CalculateArmor(armorAffixes));
            SpawnedArmorData data = new SpawnedArmorData(id, armorData.ArmorType, increasedArmor);
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

        if (spawnedItemData[id].GetType() == typeof(SpawnedWeaponsData))
        {
            return data as SpawnedWeaponsData;
        }
        else if (spawnedItemData[id].GetType() == typeof(SpawnedBowData))
        {
            return data as SpawnedBowData;
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
        public float novementSpeedDuringAttack;

        public SpawnedWeaponsData(string _uniqueID, int _weaponMinDamage, int _weaponMaxDamage, float _attacksPerSecond)
        {
            uniqueID = _uniqueID;
            weaponMinDamage = _weaponMinDamage;
            weaponMaxDamage = _weaponMaxDamage;
            attacksPerSecond = _attacksPerSecond;
        }
    }

    public class SpawnedShieldData : SpawnedItemData
    {
        public int blockAngle;
        public int blockAmount;
        public int armorAmount;

        public SpawnedShieldData(string _uniqueID, int _blockAngle, int _blockAmount, int _armorAmount)
        {
            uniqueID = _uniqueID;
            blockAngle = _blockAngle;
            blockAmount = _blockAmount;
            armorAmount = _armorAmount;
        }
    }

    public class SpawnedBowData : SpawnedItemData
    {
        public int weaponMinDamage;
        public int weaponMaxDamage;
        public float attacksPerSecond;
        public float novementSpeedDuringAttack;
        public int numberOfProjectiles;

        public SpawnedBowData(string _uniqueID, int _weaponMinDamage, int _weaponMaxDamage, float _attacksPerSecond, float _movementSpeedDuringAttack, int _numberOfProjectiles)
        {
            uniqueID = _uniqueID;
            weaponMinDamage = _weaponMinDamage;
            weaponMaxDamage = _weaponMaxDamage;
            attacksPerSecond = _attacksPerSecond;
            novementSpeedDuringAttack = _movementSpeedDuringAttack;
            numberOfProjectiles = _numberOfProjectiles;
        }
    }

    public class SpawnedArmorData : SpawnedItemData
    {
        public ArmorType armorType;
        public int armorAmount;

        public SpawnedArmorData(string _uniqueID, ArmorType _armorType, int _armorAmount)
        {
            uniqueID = _uniqueID;
            armorType = _armorType;
            armorAmount = _armorAmount;
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