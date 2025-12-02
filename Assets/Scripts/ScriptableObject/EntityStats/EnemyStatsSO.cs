using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Entity Data/Enemy Stats")]
[System.Serializable]
public class EnemyStatsSO : EntityStatsSO
{
    public float WanderSpeed;
    public float WanderRadius;
    public float AggroRange;
    public float ChaseSpeed;
    public float AttackRange;
    public float AttackCooldown;
    public float RotationSpeed;
}


