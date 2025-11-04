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

    public void OnInteract(int playerIndex)
    {
        OpenChest();
    }

    private void OpenChest()
    {
        isInteractable = false;
        animator.SetTrigger("Open");
        StartCoroutine(SpawnItems());
    }

    private IEnumerator SpawnItems()
    {
        int numItemsToSpawn = SpawnedItemDataBase.Instance.GetAffixCount(Rarity);
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            Item instantiatedItem = SpawnedItemDataBase.Instance.SpawnRandomItem(Rarity);
            instantiatedItem.transform.position = ReturnSpawnPositionInRadius();
            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < itemsToSpawn.Count; i++)
        {
            Item instantiatedItem = Instantiate(itemsToSpawn[i], ReturnSpawnPositionInRadius(), Quaternion.identity);
            instantiatedItem.itemData.itemID = SpawnedItemDataBase.Instance.RegisterItemToDatabase(instantiatedItem.itemData);
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
