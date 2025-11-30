using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Potion Data")]
public class PotionSO : ItemSO
{
    public string PotionName;
    public Sprite PotionSprite;
    public GameObject PotionPrefab;
    public Resources.ResourceType ResourceToRestore;
    public int AmountOfResourceToRestore;
    public int RestoreDuration;
}
