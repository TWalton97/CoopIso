using UnityEngine;
using System;

[Serializable]
public class MovementData : ComponentData
{
    [field: SerializeField] public AttackMovement[] attackData { get; private set; }
}