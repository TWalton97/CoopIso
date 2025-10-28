using UnityEngine;
using System;

[Serializable]
public class AttackMovement
{
    [field: SerializeField] public Vector3 Direction { get; private set; }
    [field: SerializeField] public float Velocity { get; private set; }
}
