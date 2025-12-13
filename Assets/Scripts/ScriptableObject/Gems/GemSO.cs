using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gem")]
public class GemSO : ScriptableObject
{
    public string GemName;
    public int LootBudgetCost;
    public Sprite GemSprite;
    public Color GemColor;

    public List<GemEffectSO> effects;
    public List<GemDescription> GemDescriptions;

    public string GetDescriptionForSlot(EquipmentSlotType slotType)
    {
        string description = "";
        foreach (GemDescription gemDescription in GemDescriptions)
        {
            if (gemDescription.AppliesTo == slotType)
                return gemDescription.description;
        }

        return description;
    }

    public GemAffix GetGemAFfixForSlot(EquipmentSlotType slotType)
    {
        GemAffix affix = new GemAffix();
        affix.affixText = "";
        foreach (GemDescription gemDescription in GemDescriptions)
        {
            if (gemDescription.AppliesTo == slotType)
                return gemDescription.gemAffix;
        }
        return affix;
    }
}

public abstract class GemEffectSO : ScriptableObject
{
    public EquipmentSlotType AppliesTo;
    public abstract void Apply(GemContext context);
    public abstract void ApplyOnce(GemContext context, Entity target);
    public abstract void Deregister();
}

public struct GemContext
{
    public PlayerContext playerContext;
    public ItemData itemData;
    public EquipmentSlotType slotType;
}

[System.Serializable]
public class GemDescription
{
    public EquipmentSlotType AppliesTo;
    [TextArea] public string description;
    public GemAffix gemAffix;
}

[System.Serializable]
public class GemAffix
{
    public enum AffixType
    {
        Prefix,
        Suffix
    }
    public AffixType affixType;
    public string affixText;
}


