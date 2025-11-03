using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    public ItemData itemData;
    public InventoryManager inventoryManager;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private bool _isInteractable = false;
    public InteractionType InteractionType;

    public string interactableName { get => itemData.itemName; set => itemData.itemName = value; }
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        if (itemData.vfxPrefab != null)
            Instantiate(itemData.vfxPrefab, transform);
        targetRotation = transform.rotation;
        targetPosition = transform.position;
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
        _isInteractable = true;
        yield return null;
    }

    private void CollectItem(int playerIndex)
    {
        inventoryManager.AddItemToCorrectPlayerInventory(itemData, playerIndex);
        Destroy(gameObject);
    }

    public void OnInteract(int playerIndex)
    {
        CollectItem(playerIndex);
    }
}

[System.Serializable]
public class ItemData
{
    public string itemID;
    public string itemName;
    public int quantity;
    public Sprite sprite;
    [TextArea] public string itemDescription;
    public GameObject objectPrefab;
    public GameObject vfxPrefab;
    public ItemType itemType;
    public ItemSO data;
    public List<Affix> affixes;
}
