using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class Data", menuName = "Data/Class Preset")]
[System.Serializable]
public class ClassPresetSO : ScriptableObject
{
    public string PresetName;

    public ItemSO StartingMainHandWeapon;
    public ItemSO StartingOffhandWeapon;

    public ItemSO StartingHelmet;
    public ItemSO StartingBodyArmor;
    public ItemSO StartingLegArmor;

    public List<ItemSO> StartingConsumables;

    [Tooltip("List of feats that will be added to this class's Feats Menu")] public ClassFeatConfig classFeatConfig;
    public PlayerStatsSO PlayerStatsSO;
}
