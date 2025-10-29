using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquippedSlot : MonoBehaviour
{
    //Slot appearance
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text slotName;

    //Slot data
    [SerializeField] private ItemType itemType = new ItemType();

    private Sprite itemSprite;
    private string itemName;
    private string itemDescription;

    //Other variables
    private bool slotInUse;


    public void EquipGear(Sprite sprite, string itemName, string itemDescription, GameObject weapon)
    {
        this.itemSprite = sprite;
        slotImage.sprite = this.itemSprite;
        slotName.enabled = false;

        this.itemName = itemName;
        this.itemDescription = itemDescription;

        slotInUse = true;

        NewWeaponController.Instance.EquipWeapon(NewWeaponController.WeaponAttackTypes.OneHandedAndShield, weapon);
    }
}
