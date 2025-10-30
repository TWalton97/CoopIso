using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class EquippedSlot : MonoBehaviour, IPointerClickHandler
{
    //Slot appearance
    [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text slotName;

    //Slot data
    [SerializeField] private ItemType itemType = new ItemType();
    [SerializeField] private Slot slotType;
    public ItemType equippedWeaponType { get; private set; }

    private Sprite itemSprite;
    private string itemName;
    private string itemDescription;
    private GameObject itemPrefab;
    private WeaponDataSO data;

    private InventoryManager inventoryManager;
    private InventoryController inventoryController;
    private EquipmentSOLibrary equipmentSOLibrary;

    //Other variables
    public bool slotInUse;
    public GameObject selectedShader;
    public bool isSelected;

    [SerializeField] private Sprite emptySprite;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        inventoryController = GetComponentInParent<InventoryController>();
        equipmentSOLibrary = EquipmentSOLibrary.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    void OnLeftClick()
    {
        if (isSelected && slotInUse)
        {
            inventoryManager.ClearPreviewWindow();
            UnequipGear();
        }
        else
        {
            inventoryManager.DeselectAllSlots();
            inventoryManager.UpdatePreviewWindow(itemSprite, itemName, itemType, data);
            isSelected = true;
        }
    }

    void OnRightClick()
    {
        UnequipGear();
    }

    public void EquipGear(Sprite sprite, string itemName, string itemDescription, GameObject weapon, WeaponDataSO weaponDataSO, ItemType itemType = ItemType.Head)
    {
        if (slotInUse)
            UnequipGear();

        this.itemSprite = sprite;
        slotImage.sprite = this.itemSprite;
        slotImage.enabled = true;
        slotName.enabled = false;
        itemPrefab = weapon.gameObject;
        this.itemType = itemType;
        this.equippedWeaponType = itemType;
        this.data = weaponDataSO;

        this.itemName = itemName;
        this.itemDescription = itemDescription;

        if (itemType == ItemType.OneHanded)
        {
            PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.EquipOneHandedWeapon(weapon);
            //NewWeaponController.Instance.EquipOneHandedWeapon(weapon);
        }
        else if (itemType == ItemType.Offhand)
        {
            PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.EquipOffhand(weapon);
            //NewWeaponController.Instance.EquipOffhand(weapon);
        }
        else if (itemType == ItemType.TwoHanded)
        {
            PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.EquipTwoHandedWeapon(weapon);
            //NewWeaponController.Instance.EquipTwoHandedWeapon(weapon);
        }

        slotInUse = true;
    }

    public void UnequipGear()
    {
        inventoryManager.DeselectAllSlots();
        if (slotInUse)
        {
            inventoryManager.AddItem(itemName, 1, itemSprite, itemDescription, itemPrefab, itemType, data, inventoryController.playerIndex);
        }


        this.itemSprite = emptySprite;
        slotImage.sprite = emptySprite;
        slotImage.enabled = false;
        slotName.enabled = true;
        selectedShader.SetActive(false);
        slotInUse = false;

        switch (slotType)
        {
            case Slot.MainHand:
                PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.UnequipWeapon(Weapon.WeaponHand.MainHand);
                //NewWeaponController.Instance.UnequipWeapon(Weapon.WeaponHand.MainHand);
                break;
            case Slot.OffHand:
                PlayerJoinManager.Instance.GetPlayerControllerByIndex(inventoryController.playerIndex).WeaponController.UnequipWeapon(Weapon.WeaponHand.OffHand);
                //NewWeaponController.Instance.UnequipWeapon(Weapon.WeaponHand.OffHand);
                break;
        }
    }

}

public enum Slot
{
    Potion,
    Head,
    Body,
    Legs,
    MainHand,
    OffHand,
    Trinket
}
