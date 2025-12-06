using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class LootCalculator
{
    public static int RollBudget(int minBudget, int maxBudget)
    {
        return Random.Range(minBudget, maxBudget + 1);
    }


    //This just returns a type of item, its quality (if applicable), and quantity (if applicable)
    //The item isn't turned into itemData, or assigned an ID
    public static List<LootResult> RollItemsWithBudget(int budget, float qualityBias = 0f)
    {
        budget = Random.Range(0, budget);

        List<LootResult> results = new List<LootResult>();
        // List<ItemData> results = new List<ItemData>();
        SpawnedItemDataBase database = SpawnedItemDataBase.Instance;

        int cheapestCost = int.MaxValue;
        foreach (var li in database.spawnableItems)
            cheapestCost = Mathf.Min(cheapestCost, li.BaseLootBudget);

        if (budget < cheapestCost)
            return results;

        int remaining = budget;

        int safetyCounter = 200;

        while (remaining >= cheapestCost && safetyCounter-- > 0)
        {
            ItemSO itemSO = WeightedRandom(database.spawnableItems);
            ItemQuality q = 0;
            float multiplier = 1;
            if (itemSO.ItemDropType == ItemDropType.Equipment)
            {
                q = RollQuality(qualityBias);
                multiplier = QualityFactor(q);
            }

            int cost = Mathf.FloorToInt(itemSO.BaseLootBudget * multiplier);

            if (cost <= remaining)
            {
                LootResult loot = new LootResult();
                loot.itemSO = itemSO;
                loot.quality = q;
                loot.quantity = ResolveAmount(itemSO);
                results.Add(loot);
                remaining -= cost;
            }
            else
            {
                continue;
            }
        }

        return CombineGold(results);
    }

    public static List<LootResult> CombineGold(List<LootResult> Items)
    {
        int totalQuantity = 0;
        ItemSO goldSO = null;

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            LootResult item = Items[i];
            if (item.itemSO.ItemDropType == ItemDropType.Gold)
            {
                totalQuantity += item.quantity;
                Items.RemoveAt(i);
                goldSO = item.itemSO;
            }
        }

        if (totalQuantity <= 0)
            return Items;

        LootResult loot = new LootResult();
        loot.itemSO = goldSO;
        loot.quantity = totalQuantity;

        Items.Add(loot);
        return Items;
    }

    private static int ResolveAmount(ItemSO item)
    {
        switch (item.ItemDropType)   // depends on your ItemSO
        {
            case ItemDropType.Gold:
                return Mathf.RoundToInt(item.GoldValue * 12); // example conversion
            case ItemDropType.Consumable:
                return 1; // or use cost scaling for stacks
            default:
                return 1;
        }
    }

    private static ItemSO WeightedRandom(List<ItemSO> list)
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

        return list[list.Count - 1]; // fallback
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

    public static float QualityFactor(ItemQuality q)
    {
        return Mathf.Pow(1.4f, (int)q);
    }

    public static int CalculateQualityModifiedStat(int stat, ItemQuality quality)
    {
        float newStat = Mathf.Clamp(stat * QualityFactor(quality), 1, Mathf.Infinity);
        return (int)newStat;
    }

    public static int CalculateCost(ItemSO itemSO, ItemQuality quality)
    {
        return Mathf.RoundToInt(itemSO.BaseLootBudget * QualityFactor(quality));
    }
}

public class LootResult
{
    public ItemSO itemSO;
    public ItemQuality quality = ItemQuality.Normal;
    public int quantity = 1;
}
