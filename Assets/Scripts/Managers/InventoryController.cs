using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    //This is the controller for an individual isntance of an inventory
    //It stores references to all item slots
    //

    public int playerIndex;

    public EquipmentSlot[] equipmentSlot;
    public EquippedSlot[] equippedSlot;

    [SerializeField] private Image previewImage;
    [SerializeField] private TMP_Text itemNamePreText;
    [SerializeField] private TMP_Text itemTypePreText;
    [SerializeField] private TMP_Text attackPreText;
    [SerializeField] private TMP_Text movementSpeedPreText;

    public void AddItem(string itemName, int quantity, Sprite sprite, string itemDescription, GameObject objectPrefab, ItemType itemType, WeaponDataSO weaponDataSO)
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isFull == false)
            {
                equipmentSlot[i].AddItem(itemName, quantity, sprite, itemDescription, objectPrefab, itemType, weaponDataSO);
                return;
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            if (equipmentSlot[i].isSelected)
            {
                equipmentSlot[i].isSelected = false;
            }
        }

        for (int i = 0; i < equippedSlot.Length; i++)
        {
            if (equippedSlot[i].isSelected)
            {
                equippedSlot[i].isSelected = false;
            }
        }
    }

    public void UpdatePreviewWindow(Sprite sprite, string itemName, ItemType itemType, WeaponDataSO weaponDataSO)
    {
        previewImage.sprite = sprite;
        itemNamePreText.text = itemName;
        itemTypePreText.text = itemType.ToString();
        attackPreText.text = weaponDataSO.WeaponDamage.ToString();
        movementSpeedPreText.text = weaponDataSO.MovementSpeedDuringAttack.ToString();
    }

    public void ClearPreviewWindow()
    {
        previewImage.sprite = null;
        itemNamePreText.text = "";
        itemTypePreText.text = "";
        attackPreText.text = "";
        movementSpeedPreText.text = "";
    }
}
