using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Potion Data")]
public class PotionSO : ItemSO
{
    public ItemData itemData;
    public List<PotionData> PotionData;

    public virtual void UsePotion()
    {

    }
}

[System.Serializable]
public class PotionData
{
    public Resources.ResourceType ResourceToRestore;
    public int AmountOfResourceToRestore;
    public int RestoreDuration;
}
