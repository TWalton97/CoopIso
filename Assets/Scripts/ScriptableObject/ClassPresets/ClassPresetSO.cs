using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class Data", menuName = "Data/Class Preset")]
[System.Serializable]
public class ClassPresetSO : ScriptableObject
{
    public string PresetName;

    public Item StartingMainHandWeapon;
    public Item StartingOffhandWeapon;

    public Item StartingHelmet;
    public Item StartingBodyArmor;
    public Item StartingLegArmor;

    public List<ConsumableDrop> StartingConsumables;

    [Tooltip("List of feats that will be added to this class's Feats Menu")] public ClassFeatConfig classFeatConfig;
    public PlayerStatsSO PlayerStatsSO;
}
