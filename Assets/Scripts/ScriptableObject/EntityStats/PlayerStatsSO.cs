using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Entity Data/Player Stats")]
[System.Serializable]
public class PlayerStatsSO : EntityStatsSO
{
    public int BaseArmor;
    public int BaseAttackSpeed;
}


