using System;
using System.Collections.Generic;

public static class AffixStatCalculator
{
    #region Weapon Stat Calculations
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

    #endregion

    #region Shield Stat Calculations

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

    public static float CalculateArmor(List<ShieldAffix> affixes)
    {
        float armor = 1;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == ShieldStatTypes.IncreasedArmor)
            {
                armor += affixes[i].statValue * 0.01f;
            }
        }
        return armor;
    }

    #endregion

    #region Bow Stat Calculations

    public static float CalculateAttackSpeed(List<BowAffix> affixes)
    {
        float attackSpeedMultiplier = 1;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == BowStatTypes.AttackSpeed)
            {
                attackSpeedMultiplier += affixes[i].statValue * 0.01f;
            }
        }
        return attackSpeedMultiplier;
    }

    public static int CalculateMinDamage(List<BowAffix> affixes)
    {
        int minDamage = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == BowStatTypes.MinimumDamage)
            {
                minDamage += (int)affixes[i].statValue;
            }
        }
        return minDamage;
    }

    public static int CalculateMaxDamage(List<BowAffix> affixes)
    {
        int maxDamage = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == BowStatTypes.MaximumDamage)
            {
                maxDamage += (int)affixes[i].statValue;
            }
        }
        return maxDamage;
    }
    public static int CalculateProjectileCount(List<BowAffix> affixes)
    {
        int projectileCount = 0;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == BowStatTypes.ProjectileCount)
            {
                projectileCount += (int)affixes[i].statValue;
            }
        }
        return projectileCount;
    }

    #endregion

    #region Armor Stat Calculations

    public static float CalculateArmor(List<ArmorAffix> affixes)
    {
        float armor = 1;
        for (int i = 0; i < affixes.Count; i++)
        {
            if (affixes[i].statType == ArmorStatTypes.IncreasedArmor)
            {
                armor += affixes[i].statValue * 0.01f;
            }
        }
        return armor;
    }

    #endregion
}

#region Affix Factory
[Serializable]
public class Affix
{
    public int statTier;
    public float statValue;
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
        int rand = UnityEngine.Random.Range(0, 3);
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
            case ShieldStatTypes.IncreasedArmor:
                statValue = ShieldPrefixes.IncreasedArmorAmount[rand];
                break;
        }

        Affix affix = new ShieldAffix(statType, statValue, rand);
        return affix;
    }

    public static Affix ReturnRandomBowAffix()
    {
        int rand = UnityEngine.Random.Range(0, 4);
        BowStatTypes statType = (BowStatTypes)rand;

        float statValue = 0;
        rand = UnityEngine.Random.Range(0, 3);
        switch (statType)
        {
            case BowStatTypes.MaximumDamage:
                statValue = BowPrefixes.IncreasedMaximumDamage[rand];
                break;
            case BowStatTypes.MinimumDamage:
                statValue = BowPrefixes.IncreasedMinimumDamage[rand];
                break;
            case BowStatTypes.AttackSpeed:
                statValue = BowPrefixes.IncreasedAttackSpeed[rand];
                break;
            case BowStatTypes.ProjectileCount:
                statValue = BowPrefixes.IncreasedProjectiles[rand];
                break;
        }

        Affix affix = new BowAffix(statType, statValue, rand);
        return affix;
    }

    public static Affix ReturnRandomArmorAffix()
    {
        int rand = UnityEngine.Random.Range(0, 1);
        ArmorStatTypes statType = (ArmorStatTypes)rand;

        float statValue = 0;
        rand = UnityEngine.Random.Range(0, 3);
        switch (statType)
        {
            case ArmorStatTypes.IncreasedArmor:
                statValue = ArmorPrefixes.IncreasedArmor[rand];
                break;
        }

        Affix affix = new ArmorAffix(statType, statValue, rand);
        return affix;
    }
}

#region Weapon Affixes
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
#endregion

#region Shield Affixes
public static class ShieldPrefixes
{
    public static int[] IncreasedBlockAngle = new int[] { 10, 20, 30 };
    public static int[] IncreasedBlockAmount = new int[] { 1, 2, 3 };
    public static int[] IncreasedArmorAmount = new int[] { 25, 50, 100 };
}
[Serializable]
public enum ShieldStatTypes
{
    IncreasedBlockAngle,
    IncreasedBlockAmount,
    IncreasedArmor
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

#endregion

#region Bow Affixes

public static class BowPrefixes
{
    public static int[] IncreasedMaximumDamage = new int[] { 1, 2, 3 };
    public static int[] IncreasedMinimumDamage = new int[] { 1, 2, 3 };
    public static float[] IncreasedAttackSpeed = new float[] { 5, 10, 15 };
    public static int[] IncreasedProjectiles = new int[] { 1, 2, 3 };
}
[Serializable]
public enum BowStatTypes
{
    MaximumDamage,
    MinimumDamage,
    AttackSpeed,
    ProjectileCount,
}
public class BowAffix : Affix
{
    public BowStatTypes statType;
    public BowAffix(BowStatTypes _statType, float _statValue, int _statTier)
    {
        statType = _statType;
        statValue = _statValue;
        statTier = _statTier;
    }
}

#endregion

#region Armor Affixes

public static class ArmorPrefixes
{
    public static int[] IncreasedArmor = new int[] { 50, 75, 100 };
}
[Serializable]
public enum ArmorStatTypes
{
    IncreasedArmor
}
public class ArmorAffix : Affix
{
    public ArmorStatTypes statType;
    public ArmorAffix(ArmorStatTypes _statType, float _statValue, int _statTier)
    {
        statType = _statType;
        statValue = _statValue;
        statTier = _statTier;
    }
}

#endregion
#endregion

#region Affix String Factory
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
                case ShieldStatTypes.IncreasedArmor:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases armor by " + affix.statValue + "%" + "</color>";
                    break;
            }
        }
        else if (affix.GetType() == typeof(BowAffix))
        {
            BowAffix bowAffix = affix as BowAffix;
            switch (bowAffix.statType)
            {
                case BowStatTypes.MaximumDamage:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases weapon maximum damage by " + affix.statValue + "</color>";
                    break;
                case BowStatTypes.MinimumDamage:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases weapon minimum damage by " + affix.statValue + "</color>";
                    break;
                case BowStatTypes.AttackSpeed:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases weapon attack speed by " + affix.statValue + "%" + "</color>";
                    break;
                case BowStatTypes.ProjectileCount:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases number of projectiles by " + affix.statValue + "</color>";
                    break;
            }
        }
        else if (affix.GetType() == typeof(ArmorAffix))
        {
            ArmorAffix armorAffix = affix as ArmorAffix;
            switch (armorAffix.statType)
            {
                case ArmorStatTypes.IncreasedArmor:
                    s = ReturnColorCodeBasedOnTier(affix.statTier) + "Increases armor by " + affix.statValue + "%" + "</color>";
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

#endregion

#region Affix List Converter
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

    public static List<BowAffix> ConvertListIntoBowAffixes(List<Affix> affixes)
    {
        List<BowAffix> bowAffixes = new List<BowAffix>();
        for (int i = 0; i < affixes.Count; i++)
        {
            BowAffix bowAffix = affixes[i] as BowAffix;
            bowAffixes.Add(bowAffix);
        }

        return bowAffixes;
    }

    public static List<ArmorAffix> ConvertListIntoArmorAffixes(List<Affix> affixes)
    {
        List<ArmorAffix> armorAffixes = new List<ArmorAffix>();
        for (int i = 0; i < affixes.Count; i++)
        {
            ArmorAffix armorAffix = affixes[i] as ArmorAffix;
            armorAffixes.Add(armorAffix);
        }

        return armorAffixes;
    }
}
#endregion




