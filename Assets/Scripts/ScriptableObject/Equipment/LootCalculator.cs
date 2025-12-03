using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class LootCalculator
{
    public struct LootResult
    {
        public Item item;
        public ItemData itemData;
        public ItemQuality quality;
        public int amount;
        public int budgetCost;

        public LootResult(Item _item, ItemData _itemData, ItemQuality _quality, int _amount, int _budgetCost)
        {
            this.item = _item;
            this.itemData = _itemData;
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
        Debug.Log($"total loot budget is {budget}");
        List<LootResult> results = new List<LootResult>();
        SpawnedItemDataBase database = SpawnedItemDataBase.Instance;

        int cheapestCost = int.MaxValue;
        foreach (var li in database.spawnableItems)
            cheapestCost = Mathf.Min(cheapestCost, li.itemData.data.BaseLootBudget);

        if (budget < cheapestCost)
            return results;

        int remaining = budget;

        int safetyCounter = 200;

        while (remaining >= cheapestCost && safetyCounter-- > 0)
        {
            Item chosen = WeightedRandom(database.spawnableItems);
            ItemData data = chosen.itemData.Clone();
            ItemQuality q = 0;
            float multiplier = 1;
            if (chosen.itemDropType == Item.ItemDropType.Equipment)
            {
                q = RollQuality(qualityBias);
                data.itemQuality = q;
                multiplier = QualityFactor(q);
            }

            int cost = Mathf.FloorToInt(chosen.itemData.data.BaseLootBudget * multiplier);

            Debug.Log($"Trying to spawn a {q} {chosen.itemData.itemName} which costs {cost} with a remaining budget of {remaining}");

            if (cost <= remaining)
            {
                LootResult lr = new LootResult(chosen, data, q, cost, ResolveAmount(chosen, cost));
                results.Add(lr);
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
        int totalGold = 0;

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            Item item = Items[i].item;
            if (item is GoldDrop)
            {
                totalGold += Items[i].amount;
                Items.RemoveAt(i);

            }
        }

        if (totalGold <= 0)
            return Items;

        GoldDrop combinedGoldDrop = SpawnedItemDataBase.Instance.spawnableItems[0] as GoldDrop;
        LootResult lr = new LootResult(combinedGoldDrop, combinedGoldDrop.itemData, 0, totalGold, 0);
        Items.Add(lr);
        return Items;
    }

    private static int ResolveAmount(Item item, int cost)
    {
        switch (item.itemDropType)   // depends on your ItemSO
        {
            case Item.ItemDropType.Gold:
                return Mathf.RoundToInt(cost * 12); // example conversion
            case Item.ItemDropType.Consumable:
                return 1; // or use cost scaling for stacks
            default:
                return 1;
        }
    }

    private static Item WeightedRandom(List<Item> list)
    {
        float total = 0f;
        foreach (var li in list)
            total += li.itemData.data.BaseLootWeight;

        float r = Random.value * total;

        foreach (var li in list)
        {
            r -= li.itemData.data.BaseLootWeight;
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
}
