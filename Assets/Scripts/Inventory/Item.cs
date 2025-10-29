using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public GameObject objectPrefab;

    [TextArea] public string itemDescription;

    private InventoryManager inventoryManager;

    public ItemType itemType;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;    //TODO: fix this
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            inventoryManager.AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, itemType);
            Destroy(gameObject);
        }
    }
}
