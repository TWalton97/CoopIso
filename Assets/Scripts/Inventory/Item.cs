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
    public GameObject vfxPrefab;

    [TextArea] public string itemDescription;

    public InventoryManager inventoryManager;

    public ItemType itemType;

    private float x, y, z;
    private Quaternion targetRotation;
    private Vector3 targetPosition;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;    //TODO: fix this
        data = GetComponentInChildren<Weapon>().Data;
        Instantiate(vfxPrefab, transform);
        targetRotation = transform.rotation;
        targetPosition = transform.position;
        StartCoroutine(RotateRandomly());
    }


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.TryGetComponent(out PlayerInputController controller))
        {
            inventoryManager.AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, vfxPrefab, itemType, data, controller.playerIndex);
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
