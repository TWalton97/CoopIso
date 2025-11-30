using System.Collections.Generic;
using UnityEngine;

public static class LootCalculator
{
    public struct LootResult
    {
        public Item item;
        public ItemQuality quality;
        public int amount;
        public int budgetCost;

        public LootResult(Item _item, ItemQuality _quality, int _amount, int _budgetCost)
        {
            this.item = _item;
            this.quality = _quality;
            this.amount = _amount;
            this.budgetCost = _budgetCost;
        }
    }

    public static int RollBudget(int minBudget, int maxBudget)
    {
        return Random.Range(minBudget, maxBudget + 1);
    }

    public static List<LootResult> RollItemsWithBudget(int budget, float qualityBias = 0f)
    {
        List<LootResult> results = new List<LootResult>();
        SpawnedItemDataBase database = SpawnedItemDataBase.Instance;

        int cheapestCost = int.MaxValue;
        foreach (var li in database.spawnableItems)
            cheapestCost = Mathf.Min(cheapestCost, li.baseLootBudget);

        if (budget < cheapestCost)
            return results;

        int remaining = budget;

        int safetyCounter = 200;

        while (remaining >= cheapestCost && safetyCounter-- > 0)
        {
            Item chosen = WeightedRandom(database.spawnableItems);

            ItemQuality q = RollQuality(qualityBias);
            chosen.itemData.itemQuality = q;

            float multiplier = QualityFactor(q);

            int cost = Mathf.RoundToInt(chosen.baseLootBudget * multiplier);

            if (cost <= remaining)
            {
                LootResult lr = new LootResult(chosen, q, cost, 1);
                results.Add(lr);
                remaining -= cost;
            }
            else
            {
                continue;
            }
        }
        return results;
    }

    private static Item WeightedRandom(List<Item> list)
    {
        float total = 0f;
        foreach (var li in list)
            total += li.baseLootWeight;

        float r = Random.value * total;

        foreach (var li in list)
        {
            r -= li.baseLootWeight;
            if (r <= 0f)
                return li;
        }

        return list[list.Count - 1]; // fallback
    }

    public static ItemQuality RollQuality(float qualityBias = 0f)
    {
        float r = Mathf.Clamp01(Random.value + qualityBias);

        if (r < 0.2f) return ItemQuality.Shoddy;
        if (r < 0.50f) return ItemQuality.Normal;
        if (r < 0.7f) return ItemQuality.Fine;
        if (r < 0.8f) return ItemQuality.Remarkable;
        if (r < 0.85f) return ItemQuality.Superior;
        if (r < 0.9f) return ItemQuality.Grand;
        if (r < 0.95f) return ItemQuality.Imperial;
        return ItemQuality.Flawless;
    }

    public static float QualityFactor(ItemQuality q)
    {
        return 1f + ((int)q * 0.25f);
    }

    public static int CalculateQualityModifiedStat(int stat, ItemQuality quality)
    {
        float newStat = Mathf.Clamp(stat * QualityFactor(quality), 1, Mathf.Infinity);
        return (int)newStat;
    }
}
