using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Bow Data")]
[Serializable]
public class BowSO : ItemSO
{
    [field: SerializeField] public int WeaponMinDamage { get; private set; }
    [field: SerializeField] public int WeaponMaxDamage { get; private set; }
    [field: SerializeField] public float AttacksPerSecond { get; private set; }
    [field: SerializeField] public int NumberOfAttacksInCombo { get; private set; }
}
