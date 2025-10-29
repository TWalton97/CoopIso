using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
public class ItemSO : ScriptableObject
{
    public string itemName;

    public void UseItem()
    {
        Debug.Log("Using " + itemName);
    }
}
