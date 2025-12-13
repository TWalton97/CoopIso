using UnityEngine;

public class ItemSO : ScriptableObject
{
    public string ItemName;
    public Sprite ItemSprite;
    public GameObject ItemPrefab;
    public GameObject GroundItemPrefab;

    public ItemType ItemType;
    public ItemDropType ItemDropType;
    public EquipmentSlotType EquipmentSlotType;

    public int BaseLootBudget;
    public int BaseLootWeight;

    public int GoldValue;
    public float Weight;


    public virtual bool CheckItemRequirements(PlayerStatsBlackboard playerStatsBlackboard)
    {
        return true;
    }
}
