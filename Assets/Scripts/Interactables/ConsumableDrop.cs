using System.Collections;
using UnityEngine;

public class ConsumableDrop : Item, IInteractable
{
    //Consumables will just store PotionSO
    public string itemID;
    public PotionSO potionData;

    public override string interactableName { get => potionData.PotionName; set => potionData.PotionName = value; }
    public override InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

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
        if (playerContext.PlayerController.PlayerStatsBlackboard.WeightCurrent + itemData.itemWeight > playerContext.PlayerController.PlayerStatsBlackboard.WeightMax)
        {
            StartCoroutine(RotateRandomly());
            return;
        }
        playerContext.InventoryController.AddItemToInventory(potionData);
        Destroy(gameObject);
    }

    public override void OnInteract(PlayerContext context, int playerIndex)
    {
        CollectItem(context);
    }

    public override string GetInteractableName()
    {
        return interactableName;
    }

}
