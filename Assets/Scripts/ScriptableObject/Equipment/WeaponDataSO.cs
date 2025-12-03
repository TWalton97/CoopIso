using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
[Serializable]
public class WeaponSO : ItemSO
{
    [field: SerializeField] public int WeaponMinDamage { get; private set; }
    [field: SerializeField] public int WeaponMaxDamage { get; private set; }
    [field: SerializeField] public float AttacksPerSecond { get; private set; }
    [field: SerializeField] public int NumberOfAttacksInCombo { get; private set; }
    [field: SerializeField] public WeaponRangeType WeaponRangeType;
}
