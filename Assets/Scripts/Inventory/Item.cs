using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int quantity;
    public Sprite sprite;
    public GameObject objectPrefab;
    public WeaponDataSO data;

    [TextArea] public string itemDescription;

    public InventoryManager inventoryManager;

    public ItemType itemType;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;    //TODO: fix this
        data = GetComponentInChildren<Weapon>().Data;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out PlayerInputController controller))
        {
            inventoryManager.AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, itemType, data, controller.playerIndex);
            Destroy(gameObject);
        }
    }
}
