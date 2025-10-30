using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
[Serializable]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public int WeaponDamage { get; private set; }
    [field: SerializeField] public float MovementSpeedDuringAttack { get; private set; }
}
