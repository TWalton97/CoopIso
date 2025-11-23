using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public int Rarity;
    public Transform spawnPosition;
    private Animator animator;

    public List<Item> itemsToSpawn;

    private string itemName = "Chest";
    public string interactableName { get => itemName; set => itemName = value; }

    public InteractionType InteractionType;
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    private bool _isInteractable = true;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        OpenChest(playerContext);
    }

    private void OpenChest(PlayerContext playerContext)
    {
        isInteractable = false;
        animator.SetTrigger("Open");
        StartCoroutine(SpawnItems(playerContext.SpawnedItemDatabase));
    }

    private IEnumerator SpawnItems(SpawnedItemDataBase spawnedItemDataBase)
    {
        int numItemsToSpawn = spawnedItemDataBase.GetAffixCount(Rarity);
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            Item instantiatedItem = spawnedItemDataBase.SpawnRandomItem(Rarity);
            instantiatedItem.transform.position = ReturnSpawnPositionInRadius();
            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < itemsToSpawn.Count; i++)
        {
            Item instantiatedItem = spawnedItemDataBase.SpawnRandomItem(Rarity, itemsToSpawn[i]);
            instantiatedItem.transform.position = ReturnSpawnPositionInRadius();
            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }

    private Vector3 ReturnSpawnPositionInRadius()
    {
        Vector3 insideUnitCircle = Random.insideUnitCircle;
        insideUnitCircle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        return spawnPosition.position + insideUnitCircle;
    }
}
