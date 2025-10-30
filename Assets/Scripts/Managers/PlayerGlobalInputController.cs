using UnityEngine;
using UnityEngine.UI;

public class PlayerGlobalInputController : MonoBehaviour
{
    public void OnOpenEquipmentMenu()
    {
        InventoryManager.Instance.Equipment();
    }
}
