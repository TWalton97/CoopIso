using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QualityTables
{
    public static readonly Dictionary<int, List<(ItemQuality quality, float weight)>> EnemyLevelQualityTables =
        new()
        {
            { 0, new() { (ItemQuality.Shoddy, 55f), (ItemQuality.Normal, 35f), (ItemQuality.Fine, 10f) } },
            { 1, new() { (ItemQuality.Shoddy, 55f), (ItemQuality.Normal, 35f), (ItemQuality.Fine, 10f) } },
            { 2, new() { (ItemQuality.Shoddy, 25f), (ItemQuality.Normal, 55f), (ItemQuality.Fine, 15f), (ItemQuality.Remarkable, 5f) } },
            { 3, new() { (ItemQuality.Shoddy, 10f), (ItemQuality.Normal, 60f), (ItemQuality.Fine, 20f), (ItemQuality.Remarkable, 10f) } },
            { 4, new() { (ItemQuality.Normal, 40f), (ItemQuality.Fine, 35f), (ItemQuality.Remarkable, 15f), (ItemQuality.Superior, 10f) } },
            { 5, new() { (ItemQuality.Normal, 40f), (ItemQuality.Fine, 35f), (ItemQuality.Remarkable, 15f), (ItemQuality.Superior, 10f) } },
            { 6, new() { (ItemQuality.Normal, 40f), (ItemQuality.Fine, 35f), (ItemQuality.Remarkable, 15f), (ItemQuality.Superior, 10f) } },
            { 7, new() { (ItemQuality.Normal, 40f), (ItemQuality.Fine, 35f), (ItemQuality.Remarkable, 15f), (ItemQuality.Superior, 10f) } },
            { 8, new() { (ItemQuality.Normal, 40f), (ItemQuality.Fine, 35f), (ItemQuality.Remarkable, 15f), (ItemQuality.Superior, 10f) } }
        };

    public static readonly Dictionary<ItemQuality, float> QualitySellMultiplier = new()
    {
        { ItemQuality.Shoddy,      0.35f },
        { ItemQuality.Normal,      0.5f },
        { ItemQuality.Fine,        0.65f },
        { ItemQuality.Remarkable,  0.8f },
        { ItemQuality.Superior,    0.95f },
        { ItemQuality.Grand,       1.1f },
        { ItemQuality.Imperial,    1.15f },
        { ItemQuality.Flawless,    1.3f }
    };

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

    public static readonly Dictionary<int, float> EnemyStatMultiplier = new()
    {
        { 0, 1 },
        { 1, 1 },
        { 2, 1.2f },
        { 3, 1.4f },
        { 4, 1.6f },
        { 5, 1.8f },
        { 6, 2.00f },
        { 7, 2.2f },
        { 8, 2.4f },
        { 9, 2.6f },
        { 10, 2.8f },
        { 11, 3.0f },

    };
}
