using UnityEngine;

public class ItemSO : ScriptableObject
{
    public virtual bool CheckItemRequirements(PlayerStatsBlackboard playerStatsBlackboard)
    {
        return true;
    }
}
