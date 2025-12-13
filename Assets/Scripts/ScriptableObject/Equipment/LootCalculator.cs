using System.Collections.Generic;
using UnityEngine;

public static class LootCalculator
{
    // ------------------------------------------------------------
    // CONFIGURATION
    // ------------------------------------------------------------

    // How much budget an enemy has based on HP
    public static float BudgetScalingPower = 0.65f;
    public static float BudgetScalingFactor = 0.65f;

    // Quality multipliers for stats (DA2 style)
    public static readonly Dictionary<ItemQuality, float> QualityStatMultiplier = new()
    {
        { ItemQuality.Shoddy,      0.50f },
        { ItemQuality.Normal,      1.00f },
        { ItemQuality.Fine,        1.50f },
        { ItemQuality.Remarkable,  2.00f },
        { ItemQuality.Superior,    2.50f },
        { ItemQuality.Grand,       3.00f },
        { ItemQuality.Imperial,    3.50f },
        { ItemQuality.Flawless,    4.00f }
    };

    // Sell multipliers (vendor uses this)
    public static readonly Dictionary<ItemQuality, float> QualitySellMultiplier = new()
    {
        { ItemQuality.Shoddy,      0.15f },
        { ItemQuality.Normal,      0.25f },
        { ItemQuality.Fine,        0.40f },
        { ItemQuality.Remarkable,  0.55f },
        { ItemQuality.Superior,    0.70f },
        { ItemQuality.Grand,       0.85f },
        { ItemQuality.Imperial,    1.00f },
        { ItemQuality.Flawless,    1.20f }
    };

    // ------------------------------------------------------------
    // ENEMY LEVEL QUALITY TABLES
    // ------------------------------------------------------------
    private static readonly Dictionary<int, List<(ItemQuality quality, float weight)>> EnemyLevelQualityTables =
        new()
        {
            { 0, new() { (ItemQuality.Shoddy, 60f), (ItemQuality.Normal, 35f), (ItemQuality.Fine, 5f) } },
            // Level 1 enemies
            { 1, new() { (ItemQuality.Shoddy, 60f), (ItemQuality.Normal, 35f), (ItemQuality.Fine, 5f) } },

            // Level 2 enemies
            { 2, new() { (ItemQuality.Shoddy, 30f), (ItemQuality.Normal, 55f), (ItemQuality.Fine, 12f), (ItemQuality.Remarkable, 3f) } },

            // Level 3 enemies
            { 3, new() { (ItemQuality.Shoddy, 10f), (ItemQuality.Normal, 55f), (ItemQuality.Fine, 25f), (ItemQuality.Remarkable, 10f) } },

            // Level 4+ enemies (default)
            { 4, new() { (ItemQuality.Normal, 40f), (ItemQuality.Fine, 35f), (ItemQuality.Remarkable, 15f), (ItemQuality.Superior, 10f) } }
        };

    const float GemSpawnChance = 0.3f;

    private static List<(ItemQuality quality, float weight)> GetQualityTable(int enemyLevel)
    {
        if (EnemyLevelQualityTables.ContainsKey(enemyLevel))
            return EnemyLevelQualityTables[enemyLevel];

        // fallback to the last defined table
        return EnemyLevelQualityTables[4];
    }

    // ------------------------------------------------------------
    // PUBLIC ENTRY POINT
    // ------------------------------------------------------------
    public static List<LootResult> RollLoot(int enemyLevel, int enemyMaxHP)
    {
        int budget = CalculateBudgetFromHP(enemyMaxHP);

        return RollItemsWithBudget(budget, enemyLevel);
    }

    // ------------------------------------------------------------
    // BUDGET CALCULATION
    // ------------------------------------------------------------
    private static int CalculateBudgetFromHP(int hp)
    {
        float scaled = Mathf.Pow(hp, BudgetScalingPower) * BudgetScalingFactor;
        return Mathf.FloorToInt(scaled);
    }

    // ------------------------------------------------------------
    // MAIN LOOT FUNCTION
    // ------------------------------------------------------------
    private static List<LootResult> RollItemsWithBudget(int budget, int enemyLevel)
    {
        SpawnedItemDataBase db = SpawnedItemDataBase.Instance;
        List<LootResult> results = new();

        if (db.spawnableItems.Count == 0)
            return results;

        // Determine cheapest item so we know when to stop
        int cheapestCost = int.MaxValue;
        foreach (var item in db.spawnableItems)
            cheapestCost = Mathf.Min(cheapestCost, item.BaseLootBudget);

        int remaining = budget;

        int safety = 200;
        while (remaining >= cheapestCost && safety-- > 0)
        {
            ItemSO itemSO = WeightedRandomItem(db.spawnableItems);

            // Roll quality based on enemy level
            ItemQuality q = RollQualityForEnemyLevel(enemyLevel);

            // Equipment uses quality multiplier for cost
            float costMultiplier = (itemSO.ItemDropType == ItemDropType.Equipment)
                ? QualityStatMultiplier[q]
                : 1f;

            int cost = Mathf.FloorToInt(itemSO.BaseLootBudget * costMultiplier);

            if (cost <= remaining)
            {
                var loot = new LootResult
                {
                    itemSO = itemSO,
                    quality = q,
                    quantity = ResolveAmount(itemSO)
                };

                remaining -= cost;

                // Roll gems for equipment
                if (itemSO.ItemDropType == ItemDropType.Equipment && Random.value < GemSpawnChance)
                {
                    int maxSockets = 3;
                    int socketsToRoll = Random.Range(1, maxSockets + 1);

                    for (int s = 0; s < socketsToRoll; s++)
                    {
                        Debug.Log($"Remaining budget for gems is {remaining}");
                        // Get all affordable gems
                        var affordableGems = db.spawnableGems
                            .FindAll(g => g.LootBudgetCost <= remaining);

                        if (affordableGems.Count == 0)
                            break;

                        GemSO gem = affordableGems[Random.Range(0, affordableGems.Count)];

                        loot.socketedGems.Add(gem);
                        remaining -= gem.LootBudgetCost;
                    }
                }

                results.Add(loot);
            }
        }

        return CombineGold(results);
    }

    // ------------------------------------------------------------
    // QUALITY ROLLING
    // ------------------------------------------------------------
    public static ItemQuality RollQualityForEnemyLevel(int enemyLevel)
    {
        var table = GetQualityTable(enemyLevel);

        float totalWeight = 0;
        foreach (var entry in table)
            totalWeight += entry.weight;

        float r = Random.value * totalWeight;

        foreach (var entry in table)
        {
            r -= entry.weight;
            if (r <= 0f)
                return entry.quality;
        }

        return table[^1].quality; // fallback
    }

    // ------------------------------------------------------------
    // ITEM SELECTION
    // ------------------------------------------------------------
    private static ItemSO WeightedRandomItem(List<ItemSO> list)
    {
        float total = 0f;
        foreach (var li in list)
            total += li.BaseLootWeight;

        float r = Random.value * total;

        foreach (var li in list)
        {
            r -= li.BaseLootWeight;
            if (r <= 0f)
                return li;
        }

        return list[list.Count - 1];
    }

    // ------------------------------------------------------------
    // GOLD / STACKING
    // ------------------------------------------------------------
    private static int ResolveAmount(ItemSO item)
    {
        switch (item.ItemDropType)
        {
            case ItemDropType.Gold:
                return item.GoldValue;
            case ItemDropType.Consumable:
                return 1;
            default:
                return 1;
        }
    }

    private static List<LootResult> CombineGold(List<LootResult> items)
    {
        int goldTotal = 0;
        ItemSO goldSO = null;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            var lr = items[i];
            if (lr.itemSO.ItemDropType == ItemDropType.Gold)
            {
                goldTotal += lr.quantity;
                goldSO = lr.itemSO;
                items.RemoveAt(i);
            }
        }

        if (goldSO != null)
        {
            items.Add(new LootResult
            {
                itemSO = goldSO,
                quantity = goldTotal,
                quality = ItemQuality.Normal
            });
        }

        return items;
    }

    public static ItemQuality RollQuality(float qualityBias = 0f)
    {
        float r = Mathf.Clamp01(Random.value + qualityBias);

        if (r < 0.4f) return ItemQuality.Shoddy;
        if (r < 0.60f) return ItemQuality.Normal;
        if (r < 0.7f) return ItemQuality.Fine;
        if (r < 0.8f) return ItemQuality.Remarkable;
        if (r < 0.85f) return ItemQuality.Superior;
        if (r < 0.9f) return ItemQuality.Grand;
        if (r < 0.95f) return ItemQuality.Imperial;
        return ItemQuality.Flawless;
    }
}

// ------------------------------------------------------------
// RESULT CLASS
// ------------------------------------------------------------
public class LootResult
{
    public ItemSO itemSO;
    public ItemQuality quality = ItemQuality.Normal;
    public int quantity = 1;

    public List<GemSO> socketedGems = new();
}
