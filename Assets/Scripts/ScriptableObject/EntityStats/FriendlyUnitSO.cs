using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Entity Data/Friendly Unit Stats")]
[System.Serializable]
public class FriendlyUnitSO : EnemyStatsSO
{
    public float LeashRange;
    public float TeleportRange;
}


