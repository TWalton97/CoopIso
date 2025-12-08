using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{

    public ItemStatus ItemStatus;
    public ItemData ItemData;
    public ItemSO ItemSO;
    public int Quantity;
    public ItemQuality Quality;

    protected Quaternion targetRotation;
    protected Vector3 targetPosition;
    protected bool _isInteractable = false;
    public InteractionType InteractionType;

    public virtual string interactableName { get => GetInteractableName(); set => ItemData.Name = value; }
    public virtual bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public virtual InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    protected bool itemCollected { get; set; }

    void Start()
    {
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        ItemStatus = new ItemStatus(ItemData.ItemID, targetPosition, targetRotation);
        StartCoroutine(RotateRandomly());
    }

    protected IEnumerator RotateRandomly()
    {
        _isInteractable = false;
        Vector3 startPos = targetPosition + Vector3.up * 2f;
        float elapsedTime = 0f;
        while (elapsedTime < 0.3f)
        {
            transform.rotation = Random.rotation;
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / 0.3f);
            elapsedTime += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        transform.rotation = targetRotation;
        transform.position = targetPosition;
        _isInteractable = true;
        yield return null;
    }

    protected virtual void CollectItem(PlayerContext playerContext)
    {
        if (ItemData.ItemSO != null)
        {
            if (playerContext.PlayerController.PlayerStatsBlackboard.WeightCurrent + ItemData.ItemSO.Weight > playerContext.PlayerController.PlayerStatsBlackboard.WeightMax)
            {
                StartCoroutine(RotateRandomly());
                return;
            }
        }
        else if (playerContext.PlayerController.PlayerStatsBlackboard.WeightCurrent + ItemSO.Weight > playerContext.PlayerController.PlayerStatsBlackboard.WeightMax)
        {
            StartCoroutine(RotateRandomly());
            return;
        }

        if (itemCollected) return;
        itemCollected = true;
        if (ItemData.ItemSO == null)
        {
            ItemData = SpawnedItemDataBase.Instance.CreateItemData(ItemSO, Quality, Quantity);
        }

        playerContext.InventoryController.AddItemToInventory(ItemData);
        Destroy(gameObject);
    }

    public virtual void OnInteract(PlayerContext context, int playerIndex)
    {
        CollectItem(context);
    }

    public ItemStatus ReturnItemStatus()
    {
        Debug.Log($"Returning ItemStatus for {interactableName}");
        if (ItemData.ItemID == null)
        {
            ItemData.ItemID = SpawnedItemDataBase.Instance.RegisterItemToDatabase(ItemData);
        }


        ItemStatus.GUID = ItemData.ItemID;
        ItemStatus.WorldPosition = targetPosition;
        return ItemStatus;
    }

    public virtual string GetInteractableName()
    {
        if (ItemSO != null)
        {
            return Quality.ToString() + " " + ItemSO.ItemName;
        }
        return ItemData.Quality.ToString() + " " + ItemData.ItemSO.ItemName;
    }
}

[System.Serializable]
public class ItemData
{
    public string ItemID;
    public ItemSO ItemSO;
    public string ItemSO_ID;
    public ItemQuality Quality;

    public string Name { get => ItemSO.ItemName; set => Name = value; }
    public Sprite Sprite => ItemSO.ItemSprite;
    public GameObject ItemPrefab => ItemSO.ItemPrefab;
    public GameObject GroundPrefab => ItemSO.GroundItemPrefab;
    public int GoldValue => Mathf.RoundToInt(ItemSO.GoldValue * LootCalculator.QualityFactor(Quality));
    public float Weight => ItemSO.BaseLootWeight;
    public int Quantity = 1;

    public int MinDamage => (ItemSO as WeaponSO)?.WeaponMinDamage != null ? Mathf.RoundToInt((ItemSO as WeaponSO).WeaponMinDamage * LootCalculator.QualityFactor(Quality)) : 0;
    public int MaxDamage => (ItemSO as WeaponSO)?.WeaponMaxDamage != null ? Mathf.RoundToInt((ItemSO as WeaponSO).WeaponMaxDamage * LootCalculator.QualityFactor(Quality)) : 0;
    public WeaponRangeType WeaponRangeType => (ItemSO as WeaponSO)?.WeaponRangeType != null ? (ItemSO as WeaponSO).WeaponRangeType : WeaponRangeType.None;

    public int ArmorAmount => (ItemSO as ArmorSO)?.ArmorAmount != null ? Mathf.FloorToInt((ItemSO as ArmorSO).ArmorAmount * LootCalculator.QualityFactor(Quality)) : 0;
    public ArmorType ArmorType => (ItemSO as ArmorSO)?.ArmorType != null ? (ItemSO as ArmorSO).ArmorType : ArmorType.None;

    public int ShieldArmorAmount => (ItemSO as ShieldSO)?.ArmorAmount != null ? Mathf.FloorToInt((ItemSO as ShieldSO).ArmorAmount * LootCalculator.QualityFactor(Quality)) : 0;

    public PlayerResource.ResourceType ResourceType => (ItemSO as PotionSO)?.ResourceToRestore ?? PlayerResource.ResourceType.Health;
    public int ResourceAmount => (ItemSO as PotionSO)?.AmountOfResourceToRestore ?? 0;
}

[System.Serializable]
public class ItemStatus
{
    public string GUID;
    public Vector3 WorldPosition;
    public Quaternion WorldRotation;

    public ItemStatus(string _guid, Vector3 _worldPosition, Quaternion _worldRotation)
    {
        GUID = _guid;
        WorldPosition = _worldPosition;
        WorldRotation = _worldRotation;
    }
}

public enum ItemDropType
{
    Equipment,
    Consumable,
    Gold
}
