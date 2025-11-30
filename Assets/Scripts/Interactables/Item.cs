using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public int baseLootBudget;
    public float baseLootWeight;
    public ItemStatus ItemStatus;
    public ItemData itemData;
    protected Quaternion targetRotation;
    protected Vector3 targetPosition;
    protected bool _isInteractable = false;
    public InteractionType InteractionType;

    public virtual string interactableName { get => itemData.itemName; set => itemData.itemName = value; }
    public virtual bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public virtual InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    public bool itemCollected { get; private set; }

    void Start()
    {
        if (itemData.vfxPrefab != null)
            Instantiate(itemData.vfxPrefab, transform);
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        ItemStatus = new ItemStatus(itemData.itemID, targetPosition, targetRotation);
        StartCoroutine(RotateRandomly());
    }

    private IEnumerator RotateRandomly()
    {
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

    private void CollectItem(PlayerContext playerContext)
    {
        if (itemCollected) return;
        itemCollected = true;
        playerContext.InventoryController.AddItemToInventory(itemData);
        Destroy(gameObject);
    }

    public virtual void OnInteract(PlayerContext context, int playerIndex)
    {
        CollectItem(context);
    }

    public ItemStatus ReturnItemStatus()
    {
        ItemStatus.WorldPosition = targetPosition;
        return ItemStatus;
    }

    public virtual string GetInteractableName()
    {
        return itemData.itemQuality.ToString() + " " + interactableName;
    }
}

[System.Serializable]
public class ItemData
{
    public string itemID;
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public int itemValue;
    public float itemWeight;
    [TextArea] public string itemDescription;
    public GameObject objectPrefab;
    public GameObject floorObjectPrefab;
    public GameObject vfxPrefab;
    public ItemType itemType;
    public WeaponRangeType weaponRangeType;
    public ItemSO data;
    public ItemQuality itemQuality;
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
