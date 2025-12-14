using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
[Serializable]
public class WeaponSO : ItemSO
{
    [field: SerializeField] public int WeaponMinDamage { get; private set; }
    [field: SerializeField] public int WeaponMaxDamage { get; private set; }
    [field: SerializeField] public int NumberOfAttacksInCombo { get; private set; }
    public WeaponAttackSpeed WeaponAttackSpeed;
    [field: SerializeField] public WeaponRangeType WeaponRangeType;

    public float GetAttackSpeedMultiplier()
    {
        switch (WeaponAttackSpeed)
        {
            case WeaponAttackSpeed.IncrediblySlow:
                return 0.55f;
            case WeaponAttackSpeed.VerySlow:
                return 0.7f;
            case WeaponAttackSpeed.Slow:
                return 0.85f;
            case WeaponAttackSpeed.Fast:
                return 1.15f;
            case WeaponAttackSpeed.VeryFast:
                return 1.3f;
            case WeaponAttackSpeed.IncrediblyFast:
                return 1.45f;
        }
        return 1;
    }
}

public enum WeaponAttackSpeed
{
    IncrediblySlow,
    VerySlow,
    Slow,
    Normal,
    Fast,
    VeryFast,
    IncrediblyFast
}

