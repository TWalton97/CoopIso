using System;
using UnityEngine;

public abstract class AttackSO : ScriptableObject
{
    public float attackCooldown;
    public abstract void Attack(Transform transform, Vector3 attackDir);
}
