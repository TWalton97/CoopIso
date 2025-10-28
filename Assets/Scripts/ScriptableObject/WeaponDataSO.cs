using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
[Serializable]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public int NumberOfAttacks { get; private set; }
    [field: SerializeField] public float MovementSpeedDuringAttack { get; private set; }
    [field: SerializeReference] public List<ComponentData> ComponentData;
    public T GetData<T>()
    {
        return ComponentData.OfType<T>().FirstOrDefault();
    }
}
