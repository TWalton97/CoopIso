using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gem")]
public class GemSO : ScriptableObject
{
    public string GemName;
    public int LootBudgetCost;
    public Sprite GemSprite;

    public List<GemEffectEntry> GemEffects;

    public GemEffectEntry GetEffectForSlot(EquipmentSlotType Slot)
    {
        foreach (GemEffectEntry entry in GemEffects)
        {
            if (entry.Slot == Slot)
            {
                return entry;
            }
        }
        return null;
    }
}

[System.Serializable]
public class GemEffectEntry
{
    public EquipmentSlotType Slot;
    public string EffectClassName;
    public string EffectDescription;
    public GameObject HitVFX;
    public GameObject WeaponVFX;

    public string namePrefix = "";
    public string nameSuffix = "";
}

public interface IGemEffect
{
    void Initialize(ItemData item, NewPlayerController controller, GameObject hitVFX);

    void Activate();

    void Disable();
}
