using System.Collections.Generic;
using UnityEngine;

public static class LootCalculator
{
    public static float BudgetScalingPower = 0.65f;
    public static float BudgetScalingFactor = 0.65f;

    public const float GemSpawnChance = 0.3f;

    private static List<(ItemQuality quality, float weight)> GetQualityTable(int enemyLevel)
    {
        if (QualityTables.EnemyLevelQualityTables.ContainsKey(enemyLevel))
            return QualityTables.EnemyLevelQualityTables[enemyLevel];

        return QualityTables.EnemyLevelQualityTables[4];
    }

    public static List<LootResult> RollLoot(int enemyLevel, int enemyMaxHP)
    {
        int budget = CalculateBudgetFromHP(enemyMaxHP);

        return RollItemsWithBudget(budget, enemyLevel);
    }

    private static int CalculateBudgetFromHP(int hp)
    {
        float scaled = Mathf.Pow(hp, BudgetScalingPower) * BudgetScalingFactor;
        return Mathf.FloorToInt(scaled);
    }

    private static List<LootResult> RollItemsWithBudget(int budget, int enemyLevel)
    {
        SpawnedItemDataBase db = SpawnedItemDataBase.Instance;
        List<LootResult> results = new();

        if (db.spawnableItems.Count == 0)
            return results;

        int cheapestCost = int.MaxValue;
        foreach (var item in db.spawnableItems)
            cheapestCost = Mathf.Min(cheapestCost, item.BaseLootBudget);

        int remaining = budget;

        int safety = 200;
        while (remaining >= cheapestCost && safety-- > 0)
        {
            ItemSO itemSO = WeightedRandomItem(db.spawnableItems);

            ItemQuality q = RollQualityForEnemyLevel(enemyLevel);

            float costMultiplier = (itemSO.ItemDropType == ItemDropType.Equipment)
                ? QualityTables.QualityStatMultiplier[q]
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

                if (itemSO.ItemDropType == ItemDropType.Equipment && Random.value < GemSpawnChance)
                {
                    int maxSockets = 3;
                    int socketsToRoll = Random.Range(1, maxSockets + 1);

                    for (int s = 0; s < socketsToRoll; s++)
                    {
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

    public static List<GemSO> RollGemSockets(int level)
    {
        List<GemSO> returnedGems = new();
        int socketsToRoll = GetMaxGems(level);

        for (int s = 0; s < socketsToRoll; s++)
        {
            if (Random.value < GetGemChance(level))
            {
                var affordableGems = SpawnedItemDataBase.Instance.spawnableGems;

                if (affordableGems.Count == 0)
                    break;

                GemSO gem = affordableGems[Random.Range(0, affordableGems.Count)];

                returnedGems.Add(gem);
            }
        }
        return returnedGems;
    }

    private static float GetGemChance(int enemyLevel)
    {
        return Mathf.Clamp01(0.05f + enemyLevel * 0.05f);
    }

    private static int GetMaxGems(int enemyLevel)
    {
        if (enemyLevel < 3) return 1;
        if (enemyLevel < 6) return 2;
        return 3;
    }

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

public class LootResult
{
    public ItemSO itemSO;
    public ItemQuality quality = ItemQuality.Normal;
    public int quantity = 1;

    public List<GemSO> socketedGems = new();
}
