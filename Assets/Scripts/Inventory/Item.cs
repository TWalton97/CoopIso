using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;

    public InventoryManager inventoryManager;
    private float x, y, z;
    private Quaternion targetRotation;
    private Vector3 targetPosition;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;    //TODO: fix this
        itemData.data = GetComponentInChildren<Weapon>().Data;
        Instantiate(itemData.vfxPrefab, transform);
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        StartCoroutine(RotateRandomly());
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out PlayerInputController controller))
        {
            inventoryManager.AddItemToCorrectPlayerInventory(itemData, controller.playerIndex);
            Destroy(gameObject);
        }
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
        yield return null;
    }
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;
    public Sprite sprite;
    [TextArea] public string itemDescription;
    public GameObject objectPrefab;
    public GameObject vfxPrefab;
    public ItemType itemType;
    public WeaponDataSO data;
}
