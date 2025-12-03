using UnityEngine;

public class ItemSO : ScriptableObject
{
    public int GoldValue;
    public float Weight;
    public int BaseLootBudget;
    public int BaseLootWeight;
    public virtual bool CheckItemRequirements(PlayerStatsBlackboard playerStatsBlackboard)
    {
        return true;
    }
}
