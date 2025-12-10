using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable, ISaveable
{
    public int minBudget;
    public int maxBudget;

    private EntityIdentity entityIdentity;
    public ChestStatus ChestStatus;

    public int Rarity;
    public Transform spawnPosition;
    public Animator animator;

    public int minItems = 1;

    public List<Item> itemsToSpawn;
    public List<ConsumableDrop> consumablesToSpawn;

    private string itemName = "Chest";
    public string interactableName { get => itemName; set => itemName = value; }

    public InteractionType InteractionType;
    public InteractionType interactionType { get => InteractionType; set => InteractionType = value; }

    private bool _isInteractable = true;
    public bool isInteractable { get => _isInteractable; set => _isInteractable = value; }

    public int Level;

    private void Awake()
    {
        SaveRegistry.Register(this);
        animator = GetComponent<Animator>();
        entityIdentity = GetComponent<EntityIdentity>();
        ChestStatus = new ChestStatus(entityIdentity.GUID, !_isInteractable);
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
        var loot = LootCalculator.RollLoot(Level, maxBudget);

        foreach (var item in loot)
        {
            spawnedItemDataBase.SpawnWorldDropAtPosition(item, ReturnSpawnPositionInRadius());
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

    public ChestStatus ReturnChestStatus()
    {
        ChestStatus.IsOpened = !_isInteractable;
        return ChestStatus;
    }

    public string GetInteractableName()
    {
        return interactableName;
    }

    public void Save(GameStateData data)
    {

    }

    public void Load(GameStateData data)
    {

    }
}

[System.Serializable]
public class ChestStatus
{
    public string GUID;
    public bool IsOpened;

    public ChestStatus(string _guid, bool _isOpened)
    {
        GUID = _guid;
        IsOpened = _isOpened;
    }
}
