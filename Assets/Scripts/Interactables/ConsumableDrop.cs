using System.Collections;
using UnityEngine;

public class ConsumableDrop : MonoBehaviour, IInteractable
{
    //Consumables will just store PotionSO
    public string itemID;
    public ItemStatus ItemStatus;
    public PotionSO potionData;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private bool _isInteractable = false;
    public InteractionType InteractionType;

    public string interactableName { get => potionData.PotionName; set => potionData.PotionName = value; }
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    public bool itemCollected { get; private set; }

    void Start()
    {
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        ItemStatus = new ItemStatus(itemID, targetPosition, targetRotation);
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
        playerContext.InventoryController.AddItemToInventory(potionData);
        Destroy(gameObject);
    }

    public void OnInteract(PlayerContext context, int playerIndex)
    {
        if (itemCollected) return;
        itemCollected = true;
        CollectItem(context);
    }

    public ItemStatus ReturnItemStatus()
    {
        ItemStatus.WorldPosition = targetPosition;
        return ItemStatus;
    }
}
