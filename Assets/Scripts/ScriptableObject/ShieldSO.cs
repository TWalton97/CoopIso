
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Shield Data")]
public class ShieldSO : ItemSO
{
    public int BlockAngle;
    public int BlockAmount;

    public override bool CheckItemRequirements(PlayerStatsBlackboard playerStatsBlackboard)
    {
        return playerStatsBlackboard.CanEquipShields;
    }
}
