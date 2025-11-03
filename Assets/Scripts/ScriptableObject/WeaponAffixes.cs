using System;
using System.Collections.Generic;
using UnityEngine;

public static class AffixStatCalculator
{
    public static float CalculateAttackSpeed(List<WeaponAffix> affixes)
    {
        float attackSpeedMultiplier = 1;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == WeaponStatTypes.AttackSpeed)
            {
                attackSpeedMultiplier += affixes[i].statValue * 0.01f;
            }
        }
        return attackSpeedMultiplier;
    }

    public static int CalculateMinDamage(List<WeaponAffix> affixes)
    {
        int minDamage = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == WeaponStatTypes.MinimumDamage)
            {
                minDamage += (int)affixes[i].statValue;
            }
        }
        return minDamage;
    }

    public static int CalculateMaxDamage(List<WeaponAffix> affixes)
    {
        int maxDamage = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == WeaponStatTypes.MaximumDamage)
            {
                maxDamage += (int)affixes[i].statValue;
            }
        }
        return maxDamage;
    }

    public static int CalculateBlockAngle(List<ShieldAffix> affixes)
    {
        int blockAngle = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == ShieldStatTypes.IncreasedBlockAngle)
            {
                blockAngle += (int)affixes[i].statValue;
            }
        }
        return blockAngle;
    }

    public static int CalculateBlockAmount(List<ShieldAffix> affixes)
    {
        int blockAmount = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == ShieldStatTypes.IncreasedBlockAmount)
            {
                blockAmount += (int)affixes[i].statValue;
            }
        }
        return blockAmount;
    }
}

public class WeaponAffixFactory
{
    public static Affix ReturnRandomWeaponAffix()
    {
        int rand = UnityEngine.Random.Range(0, 3);
        WeaponStatTypes statType = (WeaponStatTypes)rand;

        float statValue = 0;
        rand = UnityEngine.Random.Range(0, 3);
        switch (statType)
        {
            case WeaponStatTypes.MaximumDamage:
                statValue = WeaponPrefixes.IncreasedMaximumDamage[rand];
                break;
            case WeaponStatTypes.MinimumDamage:
                statValue = WeaponPrefixes.IncreasedMinimumDamage[rand];
                break;
            case WeaponStatTypes.AttackSpeed:
                statValue = WeaponPrefixes.IncreasedAttackSpeed[rand];
                break;
        }

        Affix affix = new WeaponAffix(statType, statValue, rand);
        return affix;
    }

    public static Affix ReturnRandomShieldAffix()
    {
        int rand = UnityEngine.Random.Range(0, 2);
        ShieldStatTypes statType = (ShieldStatTypes)rand;

        float statValue = 0;
        rand = UnityEngine.Random.Range(0, 3);
        switch (statType)
        {
            case ShieldStatTypes.IncreasedBlockAngle:
                statValue = ShieldPrefixes.IncreasedBlockAngle[rand];
                break;
            case ShieldStatTypes.IncreasedBlockAmount:
                statValue = ShieldPrefixes.IncreasedBlockAmount[rand];
                break;
        }

        Affix affix = new ShieldAffix(statType, statValue, rand);
        return affix;
    }
}

public static class WeaponPrefixes
{
    public static int[] IncreasedMaximumDamage = new int[] { 1, 2, 3 };
    public static int[] IncreasedMinimumDamage = new int[] { 1, 2, 3 };
    public static float[] IncreasedAttackSpeed = new float[] { 5, 10, 15 };
}
[Serializable]
public enum WeaponStatTypes
{
    MaximumDamage,
    MinimumDamage,
    AttackSpeed
}
public class WeaponAffix : Affix
{
    public WeaponStatTypes statType;
    public WeaponAffix(WeaponStatTypes _statType, float _statValue, int _statTier)
    {
        statType = _statType;
        statValue = _statValue;
        statTier = _statTier;
    }
}

public static class ShieldPrefixes
{
    public static int[] IncreasedBlockAngle = new int[] { 10, 20, 30 };
    public static int[] IncreasedBlockAmount = new int[] { 1, 2, 3 };
}
[Serializable]
public enum ShieldStatTypes
{
    IncreasedBlockAngle,
    IncreasedBlockAmount
}
public class ShieldAffix : Affix
{
    public ShieldStatTypes statType;
    public ShieldAffix(ShieldStatTypes _statType, float _statValue, int _statTier)
    {
        statType = _statType;
        statValue = _statValue;
        statTier = _statTier;
    }
}

[Serializable]
public class Affix
{
    public int statTier;
    public float statValue;
}



public static class AffixStringBuilder
{
    public static string BuildStringBasedOnAffix(Affix affix)
    {
        string s = "";
        if (affix.GetType() == typeof(WeaponAffix))
        {
            WeaponAffix weaponAffix = affix as WeaponAffix;
            switch (weaponAffix.statType)
            {
                case WeaponStatTypes.MaximumDamage:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases weapon maximum damage by " + affix.statValue + "</color>";
                    break;
                case WeaponStatTypes.MinimumDamage:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases weapon minimum damage by " + affix.statValue + "</color>";
                    break;
                case WeaponStatTypes.AttackSpeed:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases weapon attack speed by " + affix.statValue + "%" + "</color>";
                    break;
            }
        }
        else if (affix.GetType() == typeof(ShieldAffix))
        {
            ShieldAffix shieldAffix = affix as ShieldAffix;
            switch (shieldAffix.statType)
            {
                case ShieldStatTypes.IncreasedBlockAngle:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases block angle by " + affix.statValue + "</color>";
                    break;
                case ShieldStatTypes.IncreasedBlockAmount:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases block amount by " + affix.statValue + "</color>";
                    break;
            }
        }
        return s;
    }

    public static string ReturnColorCodeBasedOnTier(int tier)
    {
        switch (tier)
        {
            case 0:
                return "<color=#7FFF70>";
            case 1:
                return "<color=#368AE3>";
            case 2:
                return "<color=#BA55F2>";
        }
        return "";
    }
}

public static class AffixListConverter
{
    public static List<WeaponAffix> ConvertListIntoWeaponAffixes(List<Affix> affixes)
    {
        List<WeaponAffix> weaponAffixes = new List<WeaponAffix>();
        for (int i = 0; i < affixes.Count; i++)
        {
            WeaponAffix weaponAffix = affixes[i] as WeaponAffix;
            weaponAffixes.Add(weaponAffix);
        }

        return weaponAffixes;
    }

    public static List<ShieldAffix> ConvertListIntoShieldAffixes(List<Affix> affixes)
    {
        List<ShieldAffix> shieldAffixes = new List<ShieldAffix>();
        for (int i = 0; i < affixes.Count; i++)
        {
            ShieldAffix shieldAffix = affixes[i] as ShieldAffix;
            shieldAffixes.Add(shieldAffix);
        }

        return shieldAffixes;
    }
}




